using BetterLogs;
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
        private readonly BetterLog _betterLog;

        public UserAPIController(IUserRepository userRepo, BetterLog betterLog)
        {
            _userRepo = userRepo;
            _betterLog = betterLog;
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
                    _betterLog.WriteLog("Getting current user - Invalid Token", "error");

                    return StatusCode(401);
                }

                var user = await _userRepo.GetUser(userId, ct: cancellationToken);

                if (user == null)
                {
                    _betterLog.WriteLog($"Getting current user - Not Found with id = {userId}", "error");
                    return StatusCode(404);
                }

                _betterLog.WriteLog("Getting current user", "info");

                return StatusCode(200, user);
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog("Get Current User - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Get Current User - {ex.Message}", "error");

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

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _betterLog.WriteLog("Login - Login or password is incorrect", "error");

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

                _betterLog.WriteLog($"Login with email - {loginResponse.User.Email}", "info");
                return StatusCode(200, loginResponse);

            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog("Login - Клиент отменил запрос", "error");

                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Login - {ex.Message}", "error");


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

                _betterLog.WriteLog("Logging out", "info");


                return StatusCode(200);
            }

            catch (Exception ex)
            {
                _betterLog.WriteLog($"Logout - {ex.Message}", "error");

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

                    _betterLog.WriteLog("Checking Auth", "info");


                    return StatusCode(200, new { authenticated = true, userDto = user });
                }

                return StatusCode(200, new { authenticated = false });
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog("CheckAuth - Клиент отменил запрос", "error");

                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"CheckAuth - {ex.Message}", "error");

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
                    _betterLog.WriteLog("Register - User with this email already exists", "error");
                    return StatusCode(400, "User with this email already exists");
                }

                var user = await _userRepo.Register(model, ct: cancellationToken);

                if (user == null)
                {
                    _betterLog.WriteLog("Ошибка при регистрации - user == null", "error");
                    return StatusCode(400);
                }

                _betterLog.WriteLog($"Registering new user with email - {user.Email}", "info");
                return StatusCode(201);
            }
            catch (OperationCanceledException)
            {
                _betterLog.WriteLog("Register - Клиент отменил запрос", "error");
                return StatusCode(499); // Клиент отменил запрос
            }
            catch (Exception ex)
            {
                _betterLog.WriteLog($"Register - {ex.Message}", "error");

                return StatusCode(500);
            }
        }

    }
}
