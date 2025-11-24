using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelPort.Server.Helpers;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Models.DTO;
using PixelPort.Server.Repository.IRepository;

namespace PixelPort.Server.Controllers
{
    [ApiController]
    [Route("api/ProductAPI")]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _dbProduct;
        private readonly ICategoryRepository _dbCategory;
        private readonly IManufacturerRepository _dbManufacturer;

        private readonly ILogger<ProductAPIController> _logger;

        private readonly IMapper _mapper;

        public ProductAPIController(
            IProductRepository dbProduct,
            ICategoryRepository dbCategory,
            IManufacturerRepository dbManufacturer,
            ILogger<ProductAPIController> logger,
            IMapper mapper)
        {
            _dbProduct = dbProduct;
            _dbCategory = dbCategory;
            _dbManufacturer = dbManufacturer;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("", Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]

        public async Task<ActionResult<List<ProductResponseDTO>>> GetProducts(
            [FromQuery] string search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] List<int> manufacturerIds = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string sortBy = "name",
            [FromQuery] bool sortDesc = false,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 24,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(0, page);
            pageSize = Math.Clamp(pageSize, 1, 99); // Ограничиваем максимальный размер страницы

            try // Получаем все товары
            {
                var pagedResult = await _dbProduct.GetAllWithDetailsAsync(
            search, categoryId, manufacturerIds, minPrice, maxPrice, sortBy, sortDesc, page, pageSize, ct: cancellationToken);

                PagedResult<ProductResponseDTO> response = new PagedResult<ProductResponseDTO>()
                {
                    Items = _mapper.Map<List<ProductResponseDTO>>(pagedResult.Items),
                    CurrentPage = pagedResult.CurrentPage,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount
                };

                if (!pagedResult.IsValidPage)
                {
                    _logger.LogInformation($"Запрошена несуществующая страница {page}. Всего страниц: {pagedResult.TotalPages}");
                }
                else
                {
                    _logger.LogInformation($"Получено {pagedResult.Items.Count} продуктов из {pagedResult.TotalCount} всего. Страница {page} из {pagedResult.TotalPages}");
                }

                return StatusCode(200, response);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Get all Products - клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
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
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponseDTO>> GetProduct(int productId, CancellationToken cancellationToken = default)    
        {
            if (productId <= 0) // Product.id = 0
            {
                _logger.LogInformation($"ERROR: Get Product with Id = {productId}");

                return StatusCode(400);
            }

            try // Ищем товар
            {
                Product product = await _dbProduct.GetWithDetailsAsync(p => p.Id == productId, tracked: false, ct: cancellationToken);

                if (product == null) // Не найден
                {
                    _logger.LogInformation($"ERROR: Get Product with Id = {productId} - NotFound");

                    return StatusCode(404);
                }

                _logger.LogInformation($"Getting Product with Id = {productId}");

                return StatusCode(200, _mapper.Map<ProductResponseDTO>(product));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Get Product - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("", Name = "CreateProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductCreateDTO>> CreateProduct(
            [FromBody] ProductCreateDTO tempCreateProduct, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (tempCreateProduct == null) // Попытка создать пустой товар
                {
                    _logger.LogInformation($"ERROR: Create Product - Empty Product");

                    return StatusCode(400);
                }
                else if (await _dbProduct.GetAsync(p => p.ProductName.ToLower() == tempCreateProduct.ProductName.ToLower(), ct: cancellationToken) != null) // Попытка создать товар с уже существующим названием
                {
                    _logger.LogInformation($"ERROR: Create Product - product with same name");

                    return StatusCode(400, "Product with that name already exeists!");
                }
                else // Создание
                {
                    Product model = _mapper.Map<Product>(tempCreateProduct); // Маппим товар

                    var responseModel = await _dbProduct.CreateAsync(model, ct: cancellationToken); // Создаём товар
                    var responseDto = _mapper.Map<ProductResponseDTO>(responseModel); // Маппим ответ

                    _logger.LogInformation($"Creating Product");

                    return StatusCode(201, responseDto);
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Create Product - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Create Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProduct(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0) // Product.Id = 0
            {
                _logger.LogInformation($"ERROR: Delete Product with productId <= 0");

                return StatusCode(400);
            }

            try
            {
                var result = await _dbProduct.GetWithDetailsAsync(p => p.Id == productId, ct: cancellationToken);
                if (result == null) // Не найден
                {
                    _logger.LogInformation($"ERROR: Delete Product with Id = {productId}");

                    return StatusCode(404);
                }

                await _dbProduct.RemoveAsync(result, ct: cancellationToken);

                _logger.LogInformation($"Deleting Product with Id = {productId}");

                return StatusCode(200);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Delete Product - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Delete Product - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(
            int productId, 
            [FromBody] ProductUpdateDTO updateTempProduct, 
            CancellationToken cancellationToken = default) {

            try
            {
                if (updateTempProduct == null || productId != updateTempProduct.Id) // Пустой товар или айди не равен переданному в теле запроса
                {
                    _logger.LogInformation($"ERROR: Update Product with Id = {productId} - Empty product or productId != product.Id");

                    return StatusCode(400);
                }

                var existingProduct = await _dbProduct.GetWithDetailsAsync( // Ищем товар
                    p => p.Id == productId,
                    ct: cancellationToken
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

                var responseModel = await _dbProduct.UpdateWithCharacteristicsAsync(existingProduct, newCharacteristics, ct: cancellationToken); // Обновляем товар
                var responseDto = _mapper.Map<ProductResponseDTO>(responseModel); // Маппим ответ

                _logger.LogInformation($"Update Product with Id = {productId}");

                return StatusCode(200, responseDto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Update Product - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Update Product with Id = {productId} - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpGet("getcategories", Name = "GetCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        public async Task<ActionResult<List<Category>>> GetCategories(CancellationToken cancellationToken = default)
        {
            try // Получаем все товары
            {
                IEnumerable<Category> categoriesList = await _dbCategory.GetAllAsync(ct: cancellationToken);

                _logger.LogInformation("Getting all categories");

                return StatusCode(200, _mapper.Map<List<CategoryResponseDTO>>(categoriesList));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Get all categories - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get all categories - {ex.Message}");

                return StatusCode(404);
            }
        }

        [HttpGet("getmanufacturers", Name = "GetManufacturers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        public async Task<ActionResult<List<ManufacturerResponseDTO>>> GetManufacturers(CancellationToken cancellationToken = default)
        {
            try // Получаем все товары
            {
                IEnumerable<Manufacturer> manufacturersList = await _dbManufacturer.GetAllAsync(ct: cancellationToken);

                _logger.LogInformation("Getting all manufacturers");

                return StatusCode(200, _mapper.Map<List<ManufacturerResponseDTO>>(manufacturersList));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Get all manufacturers - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get all manufacturers - {ex.Message}");

                return StatusCode(404);
            }
        }
    }

}
