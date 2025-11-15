using Microsoft.AspNetCore.Mvc;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Repository.IRepository;
using System.Security.Claims;

namespace PixelPort.Server.Controllers
{
    [Route("api/UserAuth")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserAPIController> _logger;

        public UserAPIController(IUserRepository userRepo, ILogger<UserAPIController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [HttpGet("getcurrentuser", Name = "GetCurrentUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUser(CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем id пользователя из токена
                var userIdString = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    _logger.LogInformation($"ERROR: Getting current user - Invalid Token");

                    return StatusCode(401);
                }

                var user = await _userRepo.GetUser(userId, ct: cancellationToken);

                if (user == null)
                {
                    _logger.LogInformation($"ERROR: Getting current user - Not Found with id = {userId}");
                    return StatusCode(404);
                }

                _logger.LogInformation($"Getting current user");

                return StatusCode(200, user);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Get Current User - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Get Current User - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO model, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await _userRepo.Login(model, ct: cancellationToken);

                if(loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _logger.LogInformation($"ERROR: Login - Login or password is incorrect");

                    return StatusCode(401, "Login or password is incorrect");
                }

                // Устанавливаем Cookies
                Response.Cookies.Append("auth_token", loginResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // false для http
                    SameSite = SameSiteMode.Lax, // Сейчас стоит Lax для тестов, для https поставить None
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                _logger.LogInformation($"Login with email - {loginResponse.User.Email}");
                return StatusCode(200, loginResponse);

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Login - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Login - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("logout", Name = "Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult Logout()
        {
            try
            {
                //Очистка cookie
                Response.Cookies.Delete("auth_token");

                _logger.LogInformation($"Logging out");

                return StatusCode(200);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Logout - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpGet("check", Name = "CheckAuth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CheckAuth(CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем id пользователя из токена
                var userIdString = User.FindFirst(ClaimTypes.Name)?.Value;

                if (int.TryParse(userIdString, out int userId))
                {
                    var user = await _userRepo.GetUser(userId, ct: cancellationToken);

                    _logger.LogInformation($"Checking Auth");

                    return StatusCode(200, new { authenticated = true, userDto = user });
                }

                return StatusCode(200, new { authenticated = false });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: CheckAuth - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: CheckAuth - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("register", Name = "Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] RegistrationRequestDTO model, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isUserEmailUnique = await _userRepo.IsUniqueUser(model.Email, ct: cancellationToken);

                if (!isUserEmailUnique)
                {
                    _logger.LogInformation($"ERROR: Register - User with this email already exists");
                    return StatusCode(400, "User with this email already exists");
                }

                var user = await _userRepo.Register(model, ct: cancellationToken);

                if (user == null)
                {
                    _logger.LogInformation($"ERROR: Register");
                    return StatusCode(400);
                }

                _logger.LogInformation($"Registering new user with email - {user.Email}");
                return StatusCode(201);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ERROR: Register - Клиент отменил запрос");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Register - {ex.Message}");

                return StatusCode(500);
            }
        }

    }
}
