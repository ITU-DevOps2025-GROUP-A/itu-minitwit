using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class PrivateTimelineBase : ComponentBase
{
    [Parameter] public required string Author { get; set; }
    [Inject] private IMessageService MessageService { get; set;  } = null!;
    [Inject] protected UserState UserState { get; set; } = null!;
    public IEnumerable<DisplayMessageDto>? Messages { get; set; } = null;

    protected MessageGroup MessageGroup { get; } = new MessageGroup();
    protected override async Task OnInitializedAsync()
    {
        if (UserState.IsLoggedIn && UserState.Username == Author)
        {
            Messages = await MessageService.GetUserAndFollowsMessages(new GetUsersMessageDTO(Author, 30));
        }
        else
        {
            Messages = await MessageService.GetUsersMessages(new GetUsersMessageDTO(Author, 30));
        }
    }
}