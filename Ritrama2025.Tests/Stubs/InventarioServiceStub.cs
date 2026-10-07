using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.InventarioService;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// IInventarioService en memoria para las pruebas de la pestana Inventario de FrmProductos:
/// no toca la base. Solo <see cref="BuscarMastersDeProducto"/> responde, con la tabla que se
/// le programe, y deja constancia de los codigos pedidos para comprobar que la carga es
/// perezosa. El resto de miembros no los usa la pestana y lanzan NotImplementedException.
/// </summary>
internal sealed class InventarioServiceStub : IInventarioService
{
    /// <summary>Tabla que devolvera la proxima busqueda.</summary>
    public DataTable ResultadoMasters { get; set; } = new();

    /// <summary>Codigos pedidos, en orden (para comprobar la carga perezosa).</summary>
    public List<string> CodigosConsultados { get; } = [];

    public Task<DataTable?> BuscarMastersDeProducto(string productId)
    {
        CodigosConsultados.Add(productId);
        return Task.FromResult<DataTable?>(ResultadoMasters);
    }

    public bool SaveMasterInitialDB(ProductMAP producto) => throw new NotImplementedException();

    public Result CrearPlantillaMaster(string pathFileName) => throw new NotImplementedException();

    public bool ValidProductid(string id) => throw new NotImplementedException();

    public bool InsertProduct(Product producto) => throw new NotImplementedException();

    public Task<DataTable?> BuscarMasterInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? estado)
        => throw new NotImplementedException();

    public Task<DataTable?> BuscarRollosCortadosInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? uniqueCode, string? codePerson, string? numeroOC)
        => throw new NotImplementedException();

    public List<string> GetExistingProductIds(IEnumerable<string> productIds) => throw new NotImplementedException();

    public List<string> GetExistingRollIds(IEnumerable<string> rollIds) => throw new NotImplementedException();

    public bool BorrarMasterDB(string rollid) => throw new NotImplementedException();

    public bool BorrarMastersDB(IEnumerable<string> rollIds) => throw new NotImplementedException();

    public bool DropTableInit(int indexTable) => throw new NotImplementedException();

    public bool ValidRollId(string rollid) => throw new NotImplementedException();

    public bool SaveRolloCortado(RolloCortado rollo) => throw new NotImplementedException();
}
