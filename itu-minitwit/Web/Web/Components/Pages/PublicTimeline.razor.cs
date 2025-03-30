using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class PublicTimelineBase : ComponentBase
{
    public IEnumerable<DisplayMessageDto>? Messages { get; set; }
    [Inject] public IMessageService MessageService { get; set;  } = null!;
    protected MessageGroup MessageGroup { get; } = new MessageGroup();

    protected override async Task OnInitializedAsync()
    { 
        Messages = await MessageService.GetMessages();
    }

}