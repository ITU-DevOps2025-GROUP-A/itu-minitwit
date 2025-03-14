using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public partial class PrivateTimeline : ComponentBase
{
    [Parameter] public string Author { get; set; }
    
    public IEnumerable<DisplayMessageDto> messages { get; set; }
    [Inject] private IMessageService messageService { get; set;  }

    protected override async Task OnInitializedAsync()
    { 
        messages = await messageService.GetAuthorMessages(Author);
    }
}