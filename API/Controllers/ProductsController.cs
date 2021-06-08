using API.Errors;
using API.Helpers;
using AutoMapper;
using Entities.Dtos;
using Entities.Interfaces;
using Entities.Models;
using Entities.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _productsContext;
        private readonly IGenericRepository<ProductBrand> _brandsContext;
        private readonly IGenericRepository<ProductType> _typesContext;

        public ProductsController(IGenericRepository<Product> productsContext,
            IGenericRepository<ProductBrand> brandsContext,
            IGenericRepository<ProductType> typesContext, IMapper mapper)
        {
            _mapper = mapper;
            _productsContext = productsContext;
            _brandsContext = brandsContext;
            _typesContext = typesContext;
        }

        [Cached(600)]
        [HttpGet]
        [ProducesResponseType(typeof(Pagination<ProductDto>), StatusCodes.Status200OK)]

        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(specParams);
            var countSpec = new ProductsWithFiltersForCountSpecification(specParams);
            var totalItems = await _productsContext.CountAsync(countSpec);
            var products = await _productsContext.ListAsync(specification);

            return new Pagination<ProductDto>(specParams.PageIndex, specParams.PageSize,
                totalItems, _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products));
        }

        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productsContext.GetEntityWithSpec(specification);

            if (product == null) return BadRequest(new ApiResponse(404));

            return _mapper.Map<Product, ProductDto>(product);
        }

        [Cached(600)]
        [HttpGet("brands")]
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrands()
        {
            return await _brandsContext.ListAllAsync();
        }

        [Cached(600)]
        [HttpGet("types")]
        public async Task<IReadOnlyList<ProductType>> GetProductTypes()
        {
            return await _typesContext.ListAllAsync();
        }
    }
}
