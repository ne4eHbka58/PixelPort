using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string email, CancellationToken ct = default);
        Task<UserDTO> GetUser(int id, CancellationToken ct = default);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO, CancellationToken ct = default);
        Task<User> Register(RegistrationRequestDTO registrationRequestDTO, CancellationToken ct = default);
    }
}
