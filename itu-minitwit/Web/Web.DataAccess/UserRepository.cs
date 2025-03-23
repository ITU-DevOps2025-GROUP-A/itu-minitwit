using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.Services.DTO_s;
using Web.Services.Repositories;

namespace Web.DataAccess;

public class UserRepository (HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration), IUserRepository
{
    public async Task<(bool, string ErrorMessage)> Register(RegisterDto dto)
    {
       var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/Register", dto);
       return response.IsSuccessStatusCode ? (true, string.Empty) 
           : (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<bool> Login(LoginUserDTO dto)
    {
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/login", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<bool>())!;
    }
}

