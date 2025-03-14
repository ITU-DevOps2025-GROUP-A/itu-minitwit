﻿using Api.Controllers;
using Web.DataAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class PublicTimelineBase : ComponentBase
{
    public IEnumerable<DisplayMessageDto> messages { get; set; }
    [Inject] private IMessageService messageService { get; set;  }

    protected override async Task OnInitializedAsync()
    { 
        messages = await messageService.GetMessages();
    }

}