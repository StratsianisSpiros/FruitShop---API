using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Specifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductSpecParams specParams)
            : base(x =>
                (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || x.ProductBrandId == specParams.BrandId) &&
                (!specParams.TypeId.HasValue || x.ProductTypeId == specParams.TypeId)
            )
        {
        }
    }
}
