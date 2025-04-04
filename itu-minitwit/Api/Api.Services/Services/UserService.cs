using Api.Services.Dto_s;
using Api.Services.Exceptions;
using Api.Services.LogDecorator;
using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IUserService
{
    public Task<ReadUserDTO> Register(CreateUserDTO createUserDto);
    public Task<bool> Login(LoginUserDTO dto);
    public Task RegisterUsersFromException(string username, string follow, UserDoesntExistException e);
    public Task RegisterUserFromException(string username, UserDoesntExistException e);
}

public class UserService(IUserRepository userRepository, MetricsConfig metrics) : IUserService
{
    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public Task<ReadUserDTO> Register(CreateUserDTO createUserDto)
    {
        return userRepository.Register(createUserDto);
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValue]
    public Task<bool> Login(LoginUserDTO dto)
    {
        return userRepository.Login(dto);
    }

    public async Task RegisterUsersFromException(string username, string follow, UserDoesntExistException e)
    {
        if (!e.FollowExists)
        {
            await userRepository.Register(new CreateUserDTO
            {
                Email = $"{follow}@email.com",
                Username = follow,
                Pwd = "TestTest"
            });
            metrics.CreateFaultyFollow.Add(1);
        }

        if (!e.UserExists)
        {
            await userRepository.Register(new CreateUserDTO
            {
                Email = $"{username}@email.com",
                Username = username,
                Pwd = "TestTest"
            });
            metrics.CreateFaultyFollower.Add(1);
        }
    }

    public async Task RegisterUserFromException(string username, UserDoesntExistException e)
    {
        await userRepository.Register(new CreateUserDTO
        {
            Username = username,
            Pwd = "TestTest",
            Email = $"{username}@email.com",
        });
        metrics.CreateFaultyFollower.Add(1);
    }
}