using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IReadOnlyCollection<Product>> GetProductsAsync();
        Task<IReadOnlyCollection<ProductBrand>> GetProductsBrandsAsync();
        Task<IReadOnlyCollection<ProductType>> GetProductsTypesAsync();
    }
}
