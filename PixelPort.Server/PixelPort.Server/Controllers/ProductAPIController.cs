using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Models.DTO;
using PixelPort.Server.Repository;
using PixelPort.Server.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelPort.Server.Controllers
{
    [ApiController]
    [Route("api/ProductAPI")]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _dbProduct;

        private readonly ILogger<ProductAPIController> _logger;

        private readonly IMapper _mapper;

        public ProductAPIController(IProductRepository dbProduct, ILogger<ProductAPIController> logger, IMapper mapper)
        {
            _dbProduct = dbProduct;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductResponseDTO>>> Get()
        {
            try // Получаем все товары
            {
                IEnumerable<Product> productsList = await _dbProduct.GetAllWithDetailsAsync();

                _logger.LogInformation("Getting all products");

                return StatusCode(200, _mapper.Map<List<ProductResponseDTO>>(productsList));
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get all Products - {ex.Message}");

                return StatusCode(404);
            }
        }

        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponseDTO>> GetProduct(int productId)
        {
            if (productId == 0) // Product.id = 0
            {
                _logger.LogInformation($"ERROR: Get Product with Id = {productId}");

                return StatusCode(400);
            }

            try // Ищем товар
            {
                Product product = await _dbProduct.GetWithDetailsAsync(p => p.Id == productId, false);

                if (product == null) // Не найден
                {
                    _logger.LogInformation($"ERROR: Get Product with Id = {productId} - NotFound");

                    return StatusCode(404);
                }

                _logger.LogInformation($"Getting Product with Id = {productId}");

                return StatusCode(200, _mapper.Map<ProductResponseDTO>(product));
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCreateDTO>> CreateProduct([FromBody] ProductCreateDTO tempCreateProduct)
        {
            try
            {
                if (tempCreateProduct == null) // Попытка создать пустой товар
                {
                    _logger.LogInformation($"ERROR: Create Product - Empty Product");

                    return StatusCode(400);
                }
                else if (await _dbProduct.GetAsync(p => p.ProductName.ToLower() == tempCreateProduct.ProductName.ToLower()) != null) // Попытка создать товар с уже существующим названием
                {
                    _logger.LogInformation($"ERROR: Create Product - product with same name");

                    return StatusCode(400, "Product with that name already exeists!");
                }
                else // Создание
                {
                    Product model = _mapper.Map<Product>(tempCreateProduct); // Маппим товар

                    var responseModel = await _dbProduct.CreateAsync(model); // Создаём товар
                    var responseDto = _mapper.Map<ProductResponseDTO>(responseModel); // Маппим ответ

                    _logger.LogInformation($"Creating Product");

                    return StatusCode(201, responseDto);
                }

            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Create Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            if (productId == 0) // Product.Id = 0
            {
                _logger.LogInformation($"ERROR: Delete Product with productId = 0");

                return StatusCode(400);
            }

            try
            {
                var result = await _dbProduct.GetWithDetailsAsync(p => p.Id == productId);
                if (result == null) // Не найден
                {
                    _logger.LogInformation($"ERROR: Delete Product with Id = {productId}");

                    return StatusCode(404);
                }

                await _dbProduct.RemoveAsync(result);

                _logger.LogInformation($"Deleting Product with Id = {productId}");

                return StatusCode(200);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Delete Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int productId, [FromBody] ProductUpdateDTO updateTempProduct) {

            try
            {
                if (updateTempProduct == null || productId != updateTempProduct.Id) // Пустой товар или айди не равен переданному в теле запроса
                {
                    _logger.LogInformation($"ERROR: Update Product with Id = {productId} - Empty product or productId != product.Id");

                    return StatusCode(400);
                }

                var existingProduct = await _dbProduct.GetWithDetailsAsync( // Ищем товар
                    p => p.Id == productId
                );

                if (existingProduct == null) // Если пустой, то возвращаем 404
                {
                    return StatusCode(404);
                }

                _mapper.Map(updateTempProduct, existingProduct); // Маппим основные свойства (без характеристик)


                // Маппим характеристики
                var newCharacteristics = updateTempProduct.Characteristics?
                    .Select(c => _mapper.Map<ProductCharacteristic>(c))
                    .ToList();

                var responseModel = await _dbProduct.UpdateWithCharacteristicsAsync(existingProduct, newCharacteristics); // Обновляем товар
                var responseDto = _mapper.Map<ProductResponseDTO>(responseModel); // Маппим ответ

                _logger.LogInformation($"Update Product with Id = {productId}");

                return StatusCode(200, responseDto);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Update Product with Id = {productId} - {ex.Message}");

                return StatusCode(500);
            }
        }
    }

}
