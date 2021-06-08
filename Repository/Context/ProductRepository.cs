using Entities.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Context
{
    public class ProductRepository : IProductRepository
    {
        public readonly StoreContext context;
        public ProductRepository(StoreContext storeContext)
        {
            context = storeContext;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await context.Products
                .Include(b => b.ProductBrand)
                .Include(t => t.ProductType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyCollection<Product>> GetProductsAsync()
        {
            return await context.Products
                .Include(b => b.ProductBrand)
                .Include(t => t.ProductType)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<ProductBrand>> GetProductsBrandsAsync()
        {
            return await context.ProductBrands.ToListAsync();
        }

        public async Task<IReadOnlyCollection<ProductType>> GetProductsTypesAsync()
        {
            return await context.ProductTypes.ToListAsync();
        }
    }
}
