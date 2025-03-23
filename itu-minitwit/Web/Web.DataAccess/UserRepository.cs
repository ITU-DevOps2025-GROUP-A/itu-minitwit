using System.Net.Http.Json;
using System.Text.Json;
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
       if (response.IsSuccessStatusCode) return (true, string.Empty);
       var json = await response.Content.ReadAsStringAsync();
       using var doc = JsonDocument.Parse(json);
       var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();
       return (false, errorMessage ?? "An error happened that could not be read");
    }

    public async Task<bool> Login(LoginUserDTO dto)
    {
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/login", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<bool>())!;
    }
}

