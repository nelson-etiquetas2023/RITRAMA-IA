using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ProductsService;

/// <summary>
/// Importación de productos desde Excel (ClosedXML, misma librería que la exportación).
/// Plantilla mínima: product_id (el código Ritrama único que teclea el usuario),
/// product_name y categoria. Se aceptan también las cabeceras CodigoRitrama, Nombre y
/// Tipo. El codigo tecleado en esa columna ES el Product_ID (clave primaria del
/// producto); el consecutivo del sistema lo asigna el importador en IdConsec (PROD,
/// arranca en 1) y el resto de columnas nacen por defecto.
/// </summary>
public sealed class ProductsImportService : IProductsImportService
{
    private readonly IProductsService _products;
    private readonly IConsecutivosService _consecutivos;
    private readonly string _connectionString;

    public ProductsImportService(IProductsService products, IConsecutivosService consecutivos, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(consecutivos);
        ArgumentNullException.ThrowIfNull(configuration);

        _products = products;
        _consecutivos = consecutivos;
        _connectionString = ConexionResolver.Resolver(configuration);
    }

    /// <summary>
    /// Encabezados de la hoja, normalizados (sin espacios ni acentos, en minúscula):
    /// "CodigoRitrama", "codigo ritrama" o "CODIGO_RITRAMA" valen lo mismo.
    /// </summary>
    private static string NormalizarEncabezado(string texto) =>
        new string((texto ?? string.Empty).Trim().ToLowerInvariant()
            .Where(c => c != ' ' && c != '_' && c != '-' && !char.IsPunctuation(c))
            .ToArray());

    public Result<List<ProductoImportFila>> LeerYValidarExcel(string pathFileName)
    {
        if (string.IsNullOrWhiteSpace(pathFileName) || !File.Exists(pathFileName))
        {
            return Result<List<ProductoImportFila>>.Failure("No se encontró el archivo de Excel indicado.");
        }

        try
        {
            List<ProductoImportFila> filas = [];

            using XLWorkbook workbook = new(pathFileName);
            IXLWorksheet worksheet = workbook.Worksheet(1);

            // Localizar columnas por encabezado (no por posición: el orden de la hoja
            // lo decide el usuario).
            int colCodigo = -1, colNombre = -1, colTipo = -1;
            IXLRow? encabezado = worksheet.FirstRowUsed();
            if (encabezado is null)
            {
                return Result<List<ProductoImportFila>>.Failure("La hoja está vacía.");
            }

            foreach (IXLCell celda in encabezado.CellsUsed())
            {
                string clave = NormalizarEncabezado(celda.GetString());

                // product_id es la cabecera de la plantilla descargable: lo que se teclea
                // ahi es el codigo Ritrama del usuario, que ya es el Product_ID del sistema
                // (el consecutivo va aparte, en la columna IdConsec).
                if (clave is "productid" or "codigoritrama" or "codigointerno") { colCodigo = celda.Address.ColumnNumber; }
                else if (clave is "nombre" or "productname" or "producto") { colNombre = celda.Address.ColumnNumber; }
                else if (clave is "tipo" or "categoria" or "tipoproducto") { colTipo = celda.Address.ColumnNumber; }
            }

            if (colCodigo < 0 || colNombre < 0 || colTipo < 0)
            {
                return Result<List<ProductoImportFila>>.Failure(
                    "La hoja debe tener las columnas: product_id (o CodigoRitrama), "
                    + "product_name (o Nombre) y categoria (o Tipo). "
                    + $"Encontradas: [{string.Join(", ", encabezado.CellsUsed().Select(c => c.GetString()))}].");
            }

            // Duplicados dentro de la propia hoja.
            HashSet<string> vistosEnHoja = new(StringComparer.OrdinalIgnoreCase);

            // Codigos ya existentes en la base (son el product_id): una sola consulta para todo el lote.
            HashSet<string> existentes = CargarCodigosExistentes();

            foreach (IXLRow fila in worksheet.RowsUsed().Skip(1))
            {
                string codigo = fila.Cell(colCodigo).GetString().Trim();
                string nombre = fila.Cell(colNombre).GetString().Trim();
                string tipo = fila.Cell(colTipo).GetString().Trim();

                ProductoImportFila item = new()
                {
                    FilaExcel = fila.RowNumber(),
                    CodigoRitrama = codigo,
                    Nombre = nombre,
                    Tipo = tipo
                };

                item.Error = ValidarFila(item, vistosEnHoja, existentes);
                filas.Add(item);
            }

            if (filas.Count == 0)
            {
                return Result<List<ProductoImportFila>>.Failure("La hoja no tiene filas de datos.");
            }

            return Result<List<ProductoImportFila>>.Success(filas);
        }
        catch (Exception ex)
        {
            ServiceLogger.LogError("ProductsImportService.LeerYValidarExcel", ex);
            return Result<List<ProductoImportFila>>.Failure("No se pudo leer el archivo de Excel: " + ex.Message);
        }
    }

    public Result CrearPlantilla(string pathFileName)
    {
        if (string.IsNullOrWhiteSpace(pathFileName))
        {
            return Result.Failure("No se indicó dónde guardar la plantilla.");
        }

        try
        {
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.AddWorksheet("Productos");

            // Las tres cabeceras que el importador acepta tal cual: el valor de product_id
            // es el codigo Ritrama que teclea el usuario y ya es el Product_ID del
            // producto; el consecutivo del sistema queda en IdConsec (lo pone el importador).
            worksheet.Cell(1, 1).Value = "product_id";
            worksheet.Cell(1, 2).Value = "product_name";
            worksheet.Cell(1, 3).Value = "categoria";
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 40;
            worksheet.Column(3).Width = 20;
            worksheet.SheetView.FreezeRows(1);

            workbook.SaveAs(pathFileName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            ServiceLogger.LogError("ProductsImportService.CrearPlantilla", ex);
            return Result.Failure("No se pudo guardar la plantilla: " + ex.Message);
        }
    }

    private static string ValidarFila(ProductoImportFila item, HashSet<string> vistosEnHoja, HashSet<string> existentes)
    {
        if (item.CodigoRitrama.Length == 0)
        {
            return "El código Ritrama es obligatorio.";
        }

        if (item.CodigoRitrama.Length > ProductValidator.MaxCodigoRitrama)
        {
            return $"El código Ritrama excede {ProductValidator.MaxCodigoRitrama} caracteres.";
        }

        if (!vistosEnHoja.Add(item.CodigoRitrama))
        {
            return $"El código Ritrama '{item.CodigoRitrama}' está repetido en la hoja.";
        }

        if (existentes.Contains(item.CodigoRitrama))
        {
            return $"Ya existe un producto con el código Ritrama '{item.CodigoRitrama}' en la base.";
        }

        if (item.Nombre.Length == 0)
        {
            return "El nombre del producto es obligatorio.";
        }

        if (item.Nombre.Length > 200)
        {
            return "El nombre del producto excede 200 caracteres.";
        }

        if (MapearTipo(item.Tipo) is null)
        {
            return $"Tipo '{item.Tipo}' no reconocido. Use: Master, Rollo Cortado, Resma o Graphics.";
        }

        return string.Empty;
    }

    /// <summary>Texto libre del Excel → bits de categoria. Null si el tipo no es valido.</summary>
    private static (bool Master, bool RolloCortado, bool Hoja, bool Graphics)? MapearTipo(string texto)
    {
        string clave = texto.Trim().ToLowerInvariant();
        return clave switch
        {
            "master" => (true, false, false, false),
            "rollo cortado" or "rollos cortados" => (false, true, false, false),
            "resma" or "hoja" or "hojas" => (false, false, true, false),
            "graphics" => (false, false, false, true),
            _ => null
        };
    }

    private HashSet<string> CargarCodigosExistentes()
    {
        HashSet<string> codigos = new(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conn = new(_connectionString);
        conn.Open();
        using SqlCommand cmd = new()
        {
            Connection = conn,
            CommandType = System.Data.CommandType.Text,
            CommandText = "SELECT product_id FROM producto"
        };
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            codigos.Add(reader.GetString(0).Trim());
        }
        return codigos;
    }

    public async Task<Result<ProductImportResumen>> ImportarFilasAsync(
        IReadOnlyList<ProductoImportFila> filasValidas,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filasValidas);

        ProductImportResumen resumen = new();

        foreach (ProductoImportFila fila in filasValidas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            (bool master, bool rollo, bool hoja, bool graphics) = MapearTipo(fila.Tipo)
                ?? (false, false, false, false); // ya validado antes; el default nunca se usa

            Product producto = new()
            {
                // El codigo tecleado en la hoja ES el product_id (clave primaria). El
                // consecutivo del sistema se guarda aparte, en IdConsec: misma via que el
                // alta manual (Nuevo → GetAndIncrementConsecProducto). Si el INSERT falla,
                // el numero se "gasta" igual que en el formulario.
                Product_id = fila.CodigoRitrama.Trim(),
                IdConsec = _consecutivos.GetAndIncrementConsecProducto(),
                Product_Name = fila.Nombre,
                Product_Description = string.Empty,
                Referencia = string.Empty,
                Codigo_Barra = string.Empty,
                Precio = 0m,
                Costo = 0m,
                Ratio = 0m,
                Anulado = false,
                Master = master,
                RolloCortado = rollo,
                Hoja = hoja,
                Graphics = graphics
            };

            Result<bool> resultado = await _products.AddValidatedAsync(producto, cancellationToken).ConfigureAwait(false);
            if (resultado.IsSuccess && resultado.Value)
            {
                resumen.Insertadas++;
            }
            else
            {
                resumen.Fallos.Add($"Fila {fila.FilaExcel} [{fila.CodigoRitrama}]: {resultado.Error ?? "error desconocido"}");
            }
        }

        return Result<ProductImportResumen>.Success(resumen);
    }
}
