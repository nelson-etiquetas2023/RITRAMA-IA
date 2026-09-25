using System.Data;
using System.Diagnostics;
using System.Reflection;
using ClosedXML.Excel;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ExportData
{
    public class ExportDataService : IExportDataService
    {

        public bool ExportToExcelProducts<T>(List<T> data, string FileName)
        {
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("La coleccion de datos no puede ser vacia para exportar a excel.", nameof(data));
            }
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add(typeof(T).Name);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            // 1. Escribir encabezados
            for (int col = 0; col < properties.Length; col++)
            {
                worksheet.Cell(1, col + 1).Value = properties[col].Name;
                // Opcional: dar formato de negrita
                worksheet.Cell(1, col + 1).Style.Font.Bold = true;
            }

            // 2. Rellenar filas con los valores de cada entidad
            int row = 2;
            foreach (T? item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    object? value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = XLCellValue.FromObject(value); // Conversión explícita
                }
                row++;
            }

            // 3. Autoajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            //Definir las columnas de forma personalizada para el inventario de master.
            if (FileName == "Products.xlsx")
            {
                worksheet.Cell(1, 1).Value = "product id.";
                worksheet.Cell(1, 2).Value = "Product Name";
                worksheet.Cell(1, 3).Value = "prodcut type";
            }

            string filePath = Path.Combine(Environment.CurrentDirectory, FileName);

            // 4. Guardar el archivo
            try
            {
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("error al guardar la hoja de excel: " + ex.Message);
                return false;
            }
            // 5) Lanzar Excel automáticamente
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filePath,      // Abre con la app por defecto (.xlsx → Excel)
                UseShellExecute = true     // Necesario en .NET Core/5+ para usar la asociación de ficheros
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report($"No se pudo abrir el archivo automáticamente: {ex.Message}");
            }
            return true;
        }
        public bool ExportToExcel<T>(List<T> data, string FileName)
        {
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("La coleccion de datos no puede ser vacia para exportar a excel.", nameof(data));
            }
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add(typeof(T).Name);

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            // 1. Escribir encabezados
            for (int col = 0; col < properties.Length; col++)
            {

                worksheet.Cell(1, col + 1).Value = properties[col].Name;

                // Opcional: dar formato de negrita
                worksheet.Cell(1, col + 1).Style.Font.Bold = true;
            }

            // 2. Rellenar filas con los valores de cada entidad
            int row = 2;
            foreach (T? item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    object? value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = XLCellValue.FromObject(value); // Conversión explícita
                }
                row++;
            }

            // 3. Autoajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            //Definir las columnas de forma personalizada para el inventario de master.
            if (FileName == "InventarioMaster.xlsx")
            {
                worksheet.Cell(1, 1).Value = "It.";
                worksheet.Cell(1, 2).Value = "Product Id.";
                worksheet.Cell(1, 3).Value = "Nombre del Producto";
                worksheet.Cell(1, 5).Value = "Width [Inch]";
                worksheet.Cell(1, 6).Value = "Length [Pies]";
                worksheet.Cell(1, 7).Value = "Consumido [Pies]";
                worksheet.Cell(1, 8).Value = "Restante [Pies]";
                worksheet.Cell(1, 14).Value = "Ubicación";
                //worksheet.Cell(1, 16).Value = "Fecha Producción";
                //worksheet.Cell(1, 17).Value = "Fecha Ingreso";



                worksheet.Column(5).AdjustToContents();
                worksheet.Column(6).AdjustToContents();

                // Fila con format condicional.
                IXLRange rango = worksheet.Range("A2:O100");
                rango.AddConditionalFormat()
                    .WhenIsTrue("=$I2=\"Desperdicio\"")
                    .Fill.SetBackgroundColor(XLColor.Red)
                    .Font.SetFontColor(XLColor.Black)
                    .Font.SetBold(true);

                worksheet.Column(4).Hide();
                worksheet.Column(15).Hide();
                worksheet.Column(16).Hide();
                worksheet.Column(17).Hide();

                worksheet.Cell(1, 18).Value = "Fecha Produción";
                worksheet.Cell(1, 19).Value = "Fecha Llegada";

            }

            string filePath = Path.Combine(Environment.CurrentDirectory, FileName);

            // 4. Guardar el archivo
            try
            {
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("error al guardar la hoja de excel: " + ex.Message);
                return false;
            }



            // 5) Lanzar Excel automáticamente
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filePath,      // Abre con la app por defecto (.xlsx → Excel)
                UseShellExecute = true     // Necesario en .NET Core/5+ para usar la asociación de ficheros
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report($"No se pudo abrir el archivo automáticamente: {ex.Message}");
            }
            return true;
        }



        public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad)
        {
            string carpetaDestino = Path.Combine(Application.StartupPath, "Archivos");

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string LabelPath = Path.Combine(carpetaDestino, "FormatDataMaster.txt");

            using (StreamWriter sr = new(LabelPath))
            {
                string linea = $"{master.Product_Id},{master.Product_Name},{master.Rollid},{master.Width},{master.Length},{master.Msi},{master.Length_Consumido},{master.Length_Restante},{master.Estado},{master.Core},{master.Splice},{master.Fecha_Impresion},{master.Fecha_Fabricacion}";
                sr.WriteLine(linea);
            }

            if (openNotePad)
            {
                //abri el archivo con el programa predeterminado.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = LabelPath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }

            return true;
        }

        public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solorc, string? fecha_produccion, string? fecha_ingreso, bool openNotePad)
        {
            try
            {
                string carpetaDestino = Path.Combine(Application.StartupPath, "Archivos");
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }
                string ArchivoPath = Path.Combine(carpetaDestino, "Datos.txt");
                using (StreamWriter sr = new(ArchivoPath))
                {
                    foreach (DataRow item in rollos)
                    {
                        if (solorc)
                        {
                            string codeperson = item["unique_code"].ToString()!.Trim();
                            string linea = $"{item["unique_code"]}";
                            sr.WriteLine(linea);

                        }
                        else
                        {
                            string productid = item["product_id"].ToString()!.Trim();
                            string uniquecode = item["unique_code"].ToString()!.Trim();
                            string width = item["width"].ToString()!.Trim();
                            string lenght = item["large"].ToString()!.Trim();
                            string msi = item["msi"].ToString()!.Trim();
                            string splice = item["splice"].ToString()!.Trim();
                            string rollid = item["roll_id"].ToString()!.Trim();
                            string codeperson = item["code_person"].ToString()!.Trim();
                            string status = item["status"].ToString()!.Trim();
                            string fecha = DateTime.Today.ToShortDateString();
                            string orden = item["numero"].ToString()!;
                            string fecha_pro = fecha_produccion!;
                            string fecha_ing = fecha_ingreso!;

                            string linea = $"{item["roll_number"]},{productid},{item["product_name"]},{uniquecode},{width},{lenght},{msi},{splice},{rollid},{codeperson},{status},{fecha},{orden},{fecha_pro},{fecha_ing}";

                            sr.WriteLine(linea);
                        }

                    }
                }

                if (openNotePad)
                {
                    //abri el archivo con el programa predeterminado.
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = ArchivoPath,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }


                return true;

            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al crear el txt de rollos cortados...: " + ex.Message);
                return false;
            }
        }
    }
}
