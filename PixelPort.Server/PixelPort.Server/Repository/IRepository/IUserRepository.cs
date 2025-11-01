using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string email);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<User> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
