using Api.DataAccess.Models;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.LogDecorator;
using Api.Services.Logging;
using Api.Services.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Api.DataAccess.Repositories;

public class UserRepository(MinitwitDbContext db, IPasswordHasher<User> passwordHasher, ILogger<UserRepository> logger) : IUserRepository
{
    
    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task<ReadUserDTO> Register(CreateUserDTO createUserDto)
    {
        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
        };
        user.PwHash = passwordHasher.HashPassword(user, createUserDto.Pwd);

        if (await db.Users.AnyAsync(u => u.Username == user.Username))
        {
            var e = new UserAlreadyExists($"User \"{user.Username}\" already exists");
            logger.LogThrowingException(e);
            throw e;
        }
        
        var createdUser = await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return new ReadUserDTO()
            { UserId = createdUser.Entity.UserId, Username = user.Username, Email = user.Email };
    }

    [LogTime]
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task<bool> Login(LoginUserDTO dto)
    {
        if (!await db.Users.AnyAsync(u => u.Username == dto.Username))
        {
            logger.LogInformation($"User: {dto.Username} does not exist");
        }

        var user = (await db.Users.Where(u => u.Username == dto.Username).FirstOrDefaultAsync())!;
        var verifyHashedPassword
            = passwordHasher.VerifyHashedPassword(user, user.PwHash, dto.Password);

        if (verifyHashedPassword == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("Password is incorrect");
        }
        
        return verifyHashedPassword != PasswordVerificationResult.Failed;
    }
}