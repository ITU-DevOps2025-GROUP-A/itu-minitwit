using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public class PrivateTimelineBase : ComponentBase, IDisposable
{
    [Parameter] public required string Author { get; set; }
    [Inject] private IMessageService MessageService { get; set;  } = null!;
    [Inject] protected UserState UserState { get; set; } = null!;
    [Inject] protected IFollowService FollowService { get; set; } = null!;
    public IEnumerable<DisplayMessageDto>? Messages { get; set; } = null;

    protected MessageGroup MessageGroup { get; } = new MessageGroup();
    
    protected bool? IsFollowing { get; set; }

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
        
        if (!UserState.IsLoggedIn) return;
        if (UserState.Username == Author) return;
        MessageGroup.Subscribe(StateHasChangedFollowHandler);
        await UpdateIsFollowing();
    }

    protected async Task Follow()
    {
        await FollowService.Follow(new FollowDto{User = UserState.Username!, OtherUser = Author});
        MessageGroup.NotifyStateChanged();
    }

    protected async Task Unfollow()
    {
        await FollowService.UnFollow(new FollowDto{User = UserState.Username!, OtherUser = Author});
        MessageGroup.NotifyStateChanged();
    }
    
    private async Task UpdateIsFollowing()
    {
        IsFollowing = await FollowService.DoesFollow(new FollowDto{User = UserState.Username!, OtherUser = Author});
    }
    
    private async void StateHasChangedFollowHandler()
    {
        await UpdateIsFollowing();
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        MessageGroup.Unsubscribe(StateHasChangedFollowHandler);
    }
}