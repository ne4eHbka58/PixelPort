using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Repository.IRepository;

namespace PixelPort.Server.Controllers
{
    [Route("api/products/{productId}/characteristics")]
    [ApiController]
    public class ProductCharacteristicAPIController : ControllerBase
    {
        private readonly IProductCharacteristicRepository _dbProductCharacteristic;
        private readonly IProductRepository _dbProduct;
        private readonly ILogger<ProductCharacteristicAPIController> _logger;
        private readonly IMapper _mapper;

        public ProductCharacteristicAPIController(
            IProductCharacteristicRepository dbProductCharacteristic,
            IProductRepository dbProduct,
            ILogger<ProductCharacteristicAPIController> logger,
            IMapper mapper)
        {
            _dbProductCharacteristic = dbProductCharacteristic;
            _dbProduct = dbProduct;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("", Name = "GetProductCharacteristics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductCharacteristicResponseDTO>>> Get(int productId)
        {
            try // Получаем все характеристики товара
            {
                IEnumerable<ProductCharacteristic> characteristicsList = await _dbProductCharacteristic.GetAllAsync(pc => pc.ProductId == productId);

                _logger.LogInformation($"Getting all characteristics by product {productId}");

                return StatusCode(200, _mapper.Map<List<ProductCharacteristicResponseDTO>>(characteristicsList));
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get all characteristics by product {productId} - {ex.Message}");

                return StatusCode(404);
            }
        }

        [HttpGet("{characteristicId:int}", Name = "GetCharacteristic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCharacteristicResponseDTO>> GetCharacteristic(int characteristicId, int productId)
        {
            if (characteristicId == 0)
            {
                _logger.LogInformation($"ERROR: Get Characteristic with Id = {characteristicId} by product {productId}");

                return StatusCode(400);
            }

            try // Ищем характеристику
            {
                ProductCharacteristic characteristic = await _dbProductCharacteristic.GetAsync(pc => pc.Id == characteristicId, false);

                if (characteristic == null) // Не найдена
                {
                    _logger.LogInformation($"ERROR: Get Characteristic with Id = {characteristicId} - NotFound by product {productId}");

                    return StatusCode(404);
                }

                _logger.LogInformation($"Getting Characteristic with Id = {characteristicId} by product {productId}");

                return StatusCode(200, _mapper.Map<ProductCharacteristicResponseDTO>(characteristic));
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get Characteristic by product {productId} - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("", Name = "CreateCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCharacteristicCreateDTO>> CreateCharacteristic([FromBody] ProductCharacteristicCreateDTO tempCreateCharacteristic, int productId)
        {
            try
            {
                if (tempCreateCharacteristic == null) // Попытка создать пустую характеристику
                {
                    _logger.LogInformation($"ERROR: Create Characteristic - Empty Characteristic");

                    return StatusCode(400);
                }
                else if (await _dbProductCharacteristic.GetAsync(pc => pc.ProductId == productId && pc.CharacteristicName.ToLower() == tempCreateCharacteristic.CharacteristicName.ToLower()) != null) // Попытка создать характеристику с уже существующим названием
                {
                    _logger.LogInformation($"ERROR: Create Characteristic - characteristic with same name");

                    return StatusCode(400, "Characteristic with that name already exeists!");
                }
                else // Создание
                {
                    ProductCharacteristic model = _mapper.Map<ProductCharacteristic>(tempCreateCharacteristic); // Маппим характеристику
                    model.ProductId = productId;

                    var responseModel = await _dbProductCharacteristic.CreateAsync(model); // Создаём характеристику
                    var responseDto = _mapper.Map<ProductCharacteristicResponseDTO>(responseModel); // Маппим ответ

                    _logger.LogInformation($"Creating Characteristic");

                    return StatusCode(201, responseDto);
                }

            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Create Characteristic - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpDelete("{characteristicId:int}", Name = "DeleteCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCharacteristic(int characteristicId, int productId)
        {
            if (characteristicId == 0)
            {
                _logger.LogInformation($"ERROR: Delete characteristic with id = 0 by product {productId}");

                return StatusCode(400);
            }

            try
            {
                var result = await _dbProductCharacteristic.GetAsync(p => p.Id == characteristicId);
                if (result == null) // Не найден
                {
                    _logger.LogInformation($"ERROR: Delete Characteristic with Id = {characteristicId} by product {productId}");

                    return StatusCode(404);
                }

                await _dbProductCharacteristic.RemoveAsync(result);

                _logger.LogInformation($"Deleting Characteristic with Id = {characteristicId}");

                return StatusCode(200);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Characteristic with Id = {characteristicId} by product {productId} - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPut("{characteristicId:int}", Name = "UpdateCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ProductCharacteristicResponseDTO>> UpdateProduct(int characteristicId, int productId, [FromBody] ProductCharacteristicUpdateDTO updateTempCharacteristic)
        {
            try
            {
                if (updateTempCharacteristic == null || characteristicId != updateTempCharacteristic.Id) // Пустая характеристика или айди не равен переданному в теле запроса
                {
                    _logger.LogInformation($"ERROR: Update Characteristic with Id = {characteristicId} by product {productId} - Empty characteristic or id != characteristic.Id");

                    return StatusCode(400);
                }

                var existingCharacteristic = await _dbProductCharacteristic.GetAsync(pc => pc.Id == characteristicId && pc.ProductId == productId);

                if (existingCharacteristic == null) // Если пустой, то возвращаем 404
                {
                    return StatusCode(404);
                }

                _mapper.Map(updateTempCharacteristic, existingCharacteristic);

                var responseModel = await _dbProductCharacteristic.UpdateCharacteristicAsync(existingCharacteristic); // Обновляем характеристику
                var responseDto = _mapper.Map<ProductCharacteristicResponseDTO>(responseModel); // Маппим ответ

                _logger.LogInformation($"Update Characteristic with Id = {characteristicId} by product {productId}");

                return StatusCode(200, responseDto);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Update Characteristic with Id = {characteristicId} by product {productId} - {ex.Message}");

                return StatusCode(500);
            }
        }
    }
}
