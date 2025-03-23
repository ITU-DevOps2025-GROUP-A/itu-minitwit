using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components;

public class CreateMessageBase : ComponentBase, IDisposable
{
    [Inject] protected UserState UserState { get; set; } = default!;
    [Inject] protected IMessageService MessageService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    
    
    [Parameter] public required string Username { get; set; } // Parent sets this
    protected string Message { get; set; } = "";
    protected string StatusMessage { get; set; } = "";

    protected override void OnInitialized()
    {
        UserState.OnChange += StateHasChangedHandler;
    }

    private async void StateHasChangedHandler()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    protected void OnInputHandler(ChangeEventArgs e)
    {
        if (e.Value != null) Message = e.Value.ToString() ?? "";
    }

    protected async Task PostMessage()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            StatusMessage = "Message cannot be empty.";
            Console.WriteLine("Message cannot be empty.");
            return;
        }

        if (Message.Length > 160)
        {
            StatusMessage = "Message cannot be more than 160 characters.";
            Console.WriteLine("Message cannot be more than 160 characters.");
            return;
        }

        var newMessage = new CreateMessageDto
        {
            Username = UserState.Username!,
            Content = Message
        };

        try
        {
            if (await MessageService.CreateMessage(newMessage))
            {
                Message = "";
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
            else
            {
                StatusMessage = "Message failed to send, try agian later";
                return;
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }
    
    public void Dispose()
    {
        UserState.OnChange -= StateHasChangedHandler;
    }
}