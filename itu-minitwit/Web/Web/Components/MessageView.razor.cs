using Microsoft.AspNetCore.Components;
using Web.Services.DTO_s;

namespace Web.Components;

public class MessageViewBase : ComponentBase
{
    // [Parameter] public string pageUrl { get; set; } = "/";
    //
    // [Parameter] public int page { get; set; } = 1;

    [Parameter] public IEnumerable<DisplayMessageDto> Messages { get; set; } = new List<DisplayMessageDto>();

    protected override async Task OnInitializedAsync()
    {
        var messages = new List<DisplayMessageDto>();
        for (int i = 0; i < 10; i++)
        {
            messages.Add(new DisplayMessageDto(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras cursus justo ante, et sollicitudin arcu mollis sit amet. Sed luctus tempor nisi et dignissim. Etia",
                "Mr. Test", "test@test.com", DateTime.Now));
        }
        
        Messages = messages;
    }
}