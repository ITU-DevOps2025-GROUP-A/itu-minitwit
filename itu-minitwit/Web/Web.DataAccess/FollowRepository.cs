using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.DataAccess.DTO_s;
using Web.Services.DTO_s;
using Web.Services.Repositories;

namespace Web.DataAccess;

public class FollowRepository(HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration),
    IFollowRepository
{
    protected const string BaseUrl = "fllws";
    
    public async Task<bool> DoesFollow(FollowDto dto)
    {
        var response = await HttpClient.GetAsync($"{ApiBaseUrl}/{BaseUrl}/{dto.User}/{dto.OtherUser}");
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<bool>();
        throw new Exception("Request didn't go well");
    }

    public async Task Follow(FollowDto dto)
    {
        var requestDto = new FollowUnfollowDto { Follow = dto.OtherUser };
        
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/{BaseUrl}/{dto.User}", requestDto);
        if (response.IsSuccessStatusCode) return;
        throw new Exception("Request didn't go well");
    }

    public async Task UnFollow(FollowDto dto)
    {
        var requestDto = new FollowUnfollowDto { Unfollow = dto.OtherUser };
        
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/{BaseUrl}/{dto.User}", requestDto);
        if (response.IsSuccessStatusCode) return;
        throw new Exception("Request didn't go well");
    }
}