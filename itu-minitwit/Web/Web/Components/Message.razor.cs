using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class MessageBase : ComponentBase
{
    [Parameter]
    public DisplayMessageDto MessageDto { get; set; } = new DisplayMessageDto(
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras cursus justo ante, et sollicitudin arcu mollis sit amet. Sed luctus tempor nisi et dignissim. Etia",
        "Mr. Test", "test@test.com", DateTime.Now);

    [Inject] protected UserState userstate { get; set; }
    
    [Inject] protected IFollowService FollowService { get; set; }

    [Inject] private NavigationManager Navigation { get; set; }

    protected bool DoseLoggedInUserFollowUser()
    {
        return FollowService.DoesFollow(userstate.Username, MessageDto.Username).Result;
    }

    protected async Task Follow()
    {
        await FollowService.Follow(userstate.Username, MessageDto.Username);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }

    protected async Task Unfollow()
    {
        await FollowService.UnFollow(userstate.Username, MessageDto.Username);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}