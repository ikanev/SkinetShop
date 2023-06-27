
using API.DTOs;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class ProductsController : BaseApiController
    {

        private readonly IGenericRepository<Product> _prodRepo;
        private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IGenericRepository<ProductType> _typeRepo;
        private readonly IMapper _mapper;


        public ProductsController(IGenericRepository<Product> prodRepo,
                                  IGenericRepository<ProductBrand> brandRepo,
                                  IGenericRepository<ProductType> typeRepo,
                                  IMapper mapper)
        {
            _prodRepo = prodRepo;
            _brandRepo = brandRepo;
            _typeRepo = typeRepo;

            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var totalItem = await _prodRepo.CountAsync(countSpec);

            var p = await _prodRepo.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(p);

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItem, data));

        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _prodRepo.GetEntityWithSpec(spec);

            if(product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return _mapper.Map<Product, ProductToReturnDto>(product); 
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            var b = await _brandRepo.ListAllAsync();
            return Ok(b);
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            var t = await _typeRepo.ListAllAsync();
            return Ok(t);
        }

    }
}