

using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProductsService
{
    /// <summary>
    /// Contrato del módulo de Productos. Mantiene compatibilidad con FrmProductos (DataSet crudo)
    /// y expone sobrecargas tipadas con <see cref="Result{T}"/> para validaciones sin excepciones.
    /// </summary>
    public interface IProductsService
    {
        /// <summary>
        /// Carga el catálogo de productos como <see cref="DataSet"/> para binding legacy.
        /// Tabla principal: DtProducts (ver <see cref="R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS"/>).
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>DataSet con al menos la tabla DtProducts.</returns>
        public Task<DataSet> Load(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserta un producto. Retorna bool para compatibilidad (true = éxito).
        /// Internamente valida categoría exclusiva y reporta errores vía <see cref="ServiceErrors"/>.
        /// Para manejo fino use <see cref="AddValidatedAsync"/>.
        /// </summary>
        /// <param name="producto">Producto a insertar.</param>
        /// <returns>True si se insertó.</returns>
        public Task<bool> Add(Product producto);

        /// <summary>
        /// Actualiza un producto existente. Un producto anulado no se puede editar (regla de negocio).
        /// Retorna bool para compatibilidad. Para manejo fino use <see cref="UpdateValidatedAsync"/>.
        /// </summary>
        /// <param name="producto">Producto con datos actualizados.</param>
        /// <returns>True si se actualizó.</returns>
        public Task<bool> Update(Product producto);

        /// <summary>
        /// Anula un producto (soft-delete: anulado=1). No elimina físicamente.
        /// </summary>
        /// <param name="IdProduct">Código del producto.</param>
        /// <returns>True si se anuló (fila afectada).</returns>
        public bool Anular(string IdProduct);

        /// <summary>
        /// Verifica si un código de producto ya existe en BD.
        /// </summary>
        /// <param name="id">Código a validar.</param>
        /// <returns>True si existe.</returns>
        public bool ValidProductid(string id);

        // ── Extensiones con Result<T> (no rompen compatibilidad, preparan migración) ──

        /// <summary>
        /// Inserta con validación de dominio y Result para no usar excepciones como control de flujo.
        /// Valida categoría exclusiva (solo un bit entre Master/RolloCortado/Hoja/Graphics).
        /// </summary>
        /// <param name="producto">Producto a insertar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Result.Success(true) si ok; Result.Failure con código VALIDATION_CATEGORY si viola regla.</returns>
        Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza con validación de dominio y verificación de anulado.
        /// </summary>
        /// <param name="producto">Producto a actualizar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Result con éxito/fallo y código de error.</returns>
        Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica existencia de forma asíncrona con Result.
        /// </summary>
        Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Anula con Result y CancellationToken.
        /// </summary>
        Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default);

        /// <summary>
        /// Carga tipada como lista de <see cref="Product"/> evitando DataSet crudo en código nuevo.
        /// Mantiene <see cref="Load"/> para compatibilidad con FrmProductos.
        /// </summary>
        Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un producto. Útil para UI antes de llamar a Add/Update.
        /// </summary>
        Result ValidateProduct(Product producto);
    }
}
