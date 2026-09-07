

using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProductsService
{
    public interface IProductsService
    {
        public Task<DataSet> Load(CancellationToken cancellationToken = default);
        public Task<bool> Add(Product producto);
        public Task<bool> Update(Product producto);
        public bool Anular(string IdProduct);
        public bool ValidProductid(string id);

    }
}
