using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Models.DTO;
using PixelPort.Server.Repository.IRepository;

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

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO model)
        {
            try
            {
                var loginResponse = await _userRepo.Login(model);

                if(loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _logger.LogInformation($"ERROR: Login - Login or password is incorrect");
                    return StatusCode(400, "Login or password is incorrect");
                }

                _logger.LogInformation($"Login with email - {loginResponse.User.Email}");
                return StatusCode(200, loginResponse);

            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Login - {ex.Message}");

                return StatusCode(500);
            }
        }

        [HttpPost("register", Name = "Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            try
            {
                bool isUserEmailUnique = await _userRepo.IsUniqueUser(model.Email);

                if (!isUserEmailUnique)
                {
                    _logger.LogInformation($"ERROR: Register - User with this email already exists");
                    return StatusCode(400, "User with this email already exists");
                }



                var user = await _userRepo.Register(model);

                if (user == null)
                {
                    _logger.LogInformation($"ERROR: Register");
                    return StatusCode(400);
                }

                _logger.LogInformation($"Registering new user with email - {user.Email}");
                return StatusCode(201);
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR: Register - {ex.Message}");

                return StatusCode(500);
            }
        }

    }
}
