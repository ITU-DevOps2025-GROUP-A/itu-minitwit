using System.Net;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.Services; 
using Web.Services.DTO_s;

namespace Web.DataAccess;

public class MessageRepository(HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration), IMessageRepository
{
    private string Endpoint { get; } = "msgs";

    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        var endpoint = $"{ApiBaseUrl}/{Endpoint}";
        return GetMessagesAsync(endpoint, 30);
    }
    
    public async Task<IEnumerable<DisplayMessageDto>> GetUsersMessages(GetUsersMessageDTO dto)
    {
        var endpoint = $"{ApiBaseUrl}/{Endpoint}/{dto.Username}";
        return await GetMessagesAsync(endpoint, dto.NumberOfMessages);
    }

    public async Task<IEnumerable<DisplayMessageDto>> GetUserAndFollowsMessages(GetUsersMessageDTO dto)
    {
        var endpoint = $"{ApiBaseUrl}/{Endpoint}/fllws/{dto.Username}";
        return await GetMessagesAsync(endpoint, dto.NumberOfMessages);
    }

    private async Task<IEnumerable<DisplayMessageDto>> GetMessagesAsync(string endpoint, int numberOfMessages)
    {
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["no"] = numberOfMessages.ToString();
        uriBuilder.Query = query.ToString();

        var response = await HttpClient.GetAsync(uriBuilder.ToString());
        response.EnsureSuccessStatusCode();
        if(response.StatusCode == HttpStatusCode.NoContent)
        {
            return new List<DisplayMessageDto>();
        }
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<DisplayMessageDto>>();
        return list ?? [];
    }
    
    public async Task<bool> CreateMessage(CreateMessageDto message)
    {
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/{Endpoint}/{message.Username}"
        ,new { message.Content });
        return response.EnsureSuccessStatusCode().IsSuccessStatusCode;
    }
}