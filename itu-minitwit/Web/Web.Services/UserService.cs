using Web.Services.DTO_s;
using Web.Services.Repositories;

namespace Web.Services;

public interface IUserService
{
    public Task<(bool, string ErrorMessage)> Register(RegisterDto dto);
    public Task<bool> Login(LoginUserDTO dto);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<(bool, string ErrorMessage)> Register(RegisterDto dto)
    {
        return await userRepository.Register(dto);
    }

    public Task<bool> Login(LoginUserDTO dto)
    {
        return userRepository.Login(dto);
    }
}