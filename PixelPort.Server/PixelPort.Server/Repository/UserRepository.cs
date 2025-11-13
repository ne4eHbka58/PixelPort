using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Repository.IRepository;
using PixelPort.Server.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PixelPort.Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PixelPortDbContext _db;
        private readonly IMapper _mapper;
        private readonly IHashing _hashing;
        private string secretKey;
        public UserRepository(PixelPortDbContext db, IMapper mapper, IHashing hashing, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _hashing = hashing;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                var userDto = _mapper.Map<UserDTO>(user);
                return userDto;
            }
            return new UserDTO();
        }

        public async Task<bool> IsUniqueUser(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginRequestDTO.Email 
                && u.Password == _hashing.ComputeHashSha128(loginRequestDTO.Password));

            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            // Если пользователь найден, генерируем JWT токен

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user)
            };

            return loginResponseDTO;
        }

        public async Task<User> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            User tempUser = _mapper.Map<User>(registrationRequestDTO);

            tempUser.Password = _hashing.ComputeHashSha128(registrationRequestDTO.Password);

            await _db.Users.AddAsync(tempUser);
            await _db.SaveChangesAsync();

            return tempUser;
        }


    }
}
