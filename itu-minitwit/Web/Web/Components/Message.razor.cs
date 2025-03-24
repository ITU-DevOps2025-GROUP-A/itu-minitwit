using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class MessageBase : ComponentBase, IDisposable
{
    [Parameter]
    public DisplayMessageDto MessageDto { get; set; } = new DisplayMessageDto
    {
        Username = "Mr. Test",
        Email = "test@test.com",
        Text =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras cursus justo ante, et sollicitudin arcu mollis sit amet. Sed luctus tempor nisi et dignissim. Etia",
        PubDate = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()
    };

    [Parameter] public required MessageGroup MessageGroup { get; set; }

    [Inject] protected UserState Userstate { get; set; } = null!;

    [Inject] protected IFollowService FollowService { get; set; } = null!;

    [Inject] private NavigationManager Navigation { get; set; } = null!;
    
    protected bool? IsFollowing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!Userstate.IsLoggedIn) return;
        if (Userstate.Username == MessageDto.Username) return;
        Userstate.OnChange += StateHasChangedUserHandler;
        MessageGroup.Subscribe(StateHasChangedFollowHandler);
        await UpdateIsFollowing();
    }

    protected async Task Follow()
    {
        await FollowService.Follow(new FollowDto{User = Userstate.Username!, OtherUser = MessageDto.Username});
        MessageGroup.NotifyStateChanged();
    }

    protected async Task Unfollow()
    {
        await FollowService.UnFollow(new FollowDto{User = Userstate.Username!, OtherUser = MessageDto.Username});
        MessageGroup.NotifyStateChanged();
    }
    
    private async Task UpdateIsFollowing()
    {
        IsFollowing = await FollowService.DoesFollow(new FollowDto{User = Userstate.Username!, OtherUser = MessageDto.Username});
    }
    
    private async void StateHasChangedUserHandler()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    private async void StateHasChangedFollowHandler()
    {
        await UpdateIsFollowing();
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        Userstate.OnChange -= StateHasChangedUserHandler;
        MessageGroup.Unsubscribe(StateHasChangedFollowHandler);
    }
}
