using AutoMapper;
using BetterLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Repository.IRepository;

namespace PixelPort.Server.Controllers
{
    [Route("api/products/{productId:int}/characteristics")]
    [ApiController]
    public class ProductCharacteristicAPIController : ControllerBase
    {
        private readonly IProductCharacteristicRepository _dbProductCharacteristic;
        private readonly BetterLog _betterLog;
        private readonly IMapper _mapper;

        public ProductCharacteristicAPIController(
            IProductCharacteristicRepository dbProductCharacteristic,
            BetterLog betterLog,
            IMapper mapper)
        {
            _dbProductCharacteristic = dbProductCharacteristic;
            _betterLog = betterLog;
            _mapper = mapper;
        }

        [HttpGet("", Name = "GetProductCharacteristics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        public async Task<ActionResult<List<ProductCharacteristicResponseDTO>>> GetCharacteristics(int productId, CancellationToken cancellationToken = default)
        {
            try // Получаем все характеристики товара
            {
                IEnumerable<ProductCharacteristic> characteristicsList = await _dbProductCharacteristic.GetAllAsync(pc => pc.ProductId == productId, ct: cancellationToken);

                _betterLog.WriteLog($"Getting all characteristics by product {productId}", "info");

                return StatusCode(200, _mapper.Map<List<ProductCharacteristicResponseDTO>>(characteristicsList));
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog("Get all characteristics - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Get all characteristics by product {productId} - {ex.Message}", "error");

                return StatusCode(404);
            }
        }

        [HttpGet("{characteristicId:int}", Name = "GetCharacteristic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCharacteristicResponseDTO>> GetCharacteristic(
            int characteristicId, 
            int productId, 
            CancellationToken cancellationToken = default)
        {
            if (characteristicId <= 0)
            {
                _betterLog.WriteLog($"Get Characteristic with Id = {characteristicId} by product {productId}", "error");

                return StatusCode(400);
            }

            try // Ищем характеристику
            {
                ProductCharacteristic characteristic = await _dbProductCharacteristic.GetAsync(
                    pc => pc.Id == characteristicId && pc.ProductId == productId, 
                    tracked: false, 
                    ct: cancellationToken);

                if (characteristic == null) // Не найдена
                {
                    _betterLog.WriteLog($"Get Characteristic with Id = {characteristicId} - NotFound by product {productId}", "error");

                    return StatusCode(404);
                }

                _betterLog.WriteLog($"Getting Characteristic with Id = {characteristicId} by product {productId}", "info");

                return StatusCode(200, _mapper.Map<ProductCharacteristicResponseDTO>(characteristic));
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog($"Get Characteristic - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Get Characteristic by product {productId} - {ex.Message}", "error");

                return StatusCode(500);
            }
        }

        [HttpPost("", Name = "CreateCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCharacteristicCreateDTO>> CreateCharacteristic(
            [FromBody] ProductCharacteristicCreateDTO tempCreateCharacteristic, 
            int productId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (tempCreateCharacteristic == null) // Попытка создать пустую характеристику
                {
                    _betterLog.WriteLog($"Create Characteristic - Empty Characteristic", "error");

                    return StatusCode(400);
                }
                else if (
                    await _dbProductCharacteristic.GetAsync(
                    pc => pc.ProductId == productId && pc.CharacteristicName.ToLower() == tempCreateCharacteristic.CharacteristicName.ToLower(),
                    ct: cancellationToken) != null) // Попытка создать характеристику с уже существующим названием
                {
                    _betterLog.WriteLog($"Create Characteristic - characteristic with same name", "error");

                    return StatusCode(400, "Characteristic with that name already exeists!");
                }
                else // Создание
                {
                    ProductCharacteristic model = _mapper.Map<ProductCharacteristic>(tempCreateCharacteristic); // Маппим характеристику
                    model.ProductId = productId;

                    var responseModel = await _dbProductCharacteristic.CreateAsync(model, ct: cancellationToken); // Создаём характеристику
                    var responseDto = _mapper.Map<ProductCharacteristicResponseDTO>(responseModel); // Маппим ответ

                    _betterLog.WriteLog($"Creating Characteristic", "info");

                    return StatusCode(201, responseDto);
                }

            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog($"Create Characteristic - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Create Characteristic - {ex.Message}", "error");

                return StatusCode(500);
            }
        }

        [HttpDelete("{characteristicId:int}", Name = "DeleteCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCharacteristic(int characteristicId, int productId, CancellationToken cancellationToken = default)
        {
            if (characteristicId <= 0)
            {
                _betterLog.WriteLog($"Delete characteristic with id <= 0 by product {productId}", "error");

                return StatusCode(400);
            }

            try
            {
                var result = await _dbProductCharacteristic.GetAsync(p => p.Id == characteristicId, ct: cancellationToken);
                if (result == null) // Не найден
                {
                    _betterLog.WriteLog($"Delete Characteristic with Id = {characteristicId} by product {productId}", "error");

                    return StatusCode(404);
                }

                await _dbProductCharacteristic.RemoveAsync(result, ct: cancellationToken);

                _betterLog.WriteLog($"Deleting Characteristic with Id = {characteristicId}", "info");

                return StatusCode(200);
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog($"Delete Characteristic - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Delete Characteristic with Id = {characteristicId} by product {productId} - {ex.Message}", "error");

                return StatusCode(500);
            }
        }

        [HttpPut("{characteristicId:int}", Name = "UpdateCharacteristic")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ProductCharacteristicResponseDTO>> UpdateProductCharacteristic(
            int characteristicId,
            int productId, 
            [FromBody] ProductCharacteristicUpdateDTO updateTempCharacteristic,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (updateTempCharacteristic == null || characteristicId != updateTempCharacteristic.Id) // Пустая характеристика или айди не равен переданному в теле запроса
                {
                    _betterLog.WriteLog($"Update Characteristic with Id = {characteristicId} by product {productId} - Empty characteristic or id != characteristic.Id", "error");
                    
                    return StatusCode(400);
                }

                var existingCharacteristic = await _dbProductCharacteristic.GetAsync(pc => pc.Id == characteristicId && pc.ProductId == productId, ct: cancellationToken);

                if (existingCharacteristic == null) // Если пустой, то возвращаем 404
                {
                    return StatusCode(404);
                }

                _mapper.Map(updateTempCharacteristic, existingCharacteristic);

                var responseModel = await _dbProductCharacteristic.UpdateCharacteristicAsync(existingCharacteristic, ct: cancellationToken); // Обновляем характеристику
                var responseDto = _mapper.Map<ProductCharacteristicResponseDTO>(responseModel); // Маппим ответ

                _betterLog.WriteLog($"Update Characteristic with Id = {characteristicId} by product {productId}", "info");

                return StatusCode(200, responseDto);
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog($"Update Characteristic - Клиент отменил запрос", "error");

                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Update Characteristic with Id = {characteristicId} by product {productId} - {ex.Message}", "error");

                return StatusCode(500);
            }
        }
    }
}
