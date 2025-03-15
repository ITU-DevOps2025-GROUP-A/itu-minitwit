﻿using System.Net.Http.Json;
using Web.DataAccess.Abstract;
using Web.Services; 
using Web.Services.DTO_s;

namespace Web.DataAccess;

public class MessageRepository(HttpClient httpClient) : BaseAPIRepository(httpClient), IMessageRepository
{
    private string Endpoint { get; } = "/msgs";

    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return GetAllAsync<DisplayMessageDto>(Endpoint);
    }
    
    // public Task<bool> PostMessageAsync(DisplayMessageDto message)
    // {
    //     return CreateAsync<DisplayMessageDto,>()
    // }
}