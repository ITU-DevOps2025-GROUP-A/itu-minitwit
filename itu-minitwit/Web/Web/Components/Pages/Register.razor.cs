using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Web.Services;
using Web.Services.DTO_s;

namespace Web.Components.Pages;

public partial class Register : ComponentBase
{
    [SupplyParameterFromForm(FormName = "Register")]
    protected RegisterModel RegisterModel { get; set; } = new();

    protected string? ErrorMessage;

    [Inject] protected IUserService UserService { get; set; } = null!;

    [Inject] protected UserState UserState { get; set; } = null!;

    [Inject] protected NavigationManager Navigation { get; set; } = null!;

    protected async Task HandleValidSubmit()
    {
        ErrorMessage = string.Empty;

        var registerRequest = new RegisterDto
        {
            Username = RegisterModel.Username,
            Email = RegisterModel.Email,
            Pwd = RegisterModel.Password
        };
        
        var (success, error) = await UserService.Register(registerRequest);

        if (success)
        {
            UserState.SetUser(RegisterModel.Username);
            Navigation.NavigateTo("/");
        }
        else
        {
            ErrorMessage = error;
        }
    }
}

public class RegisterModel
{
    [Required] public string Username { get; set; } = null!;

    [Required, EmailAddress] public string Email { get; set; } = null!;

    [Required, MinLength(1)] public string Password { get; set; } = null!;

    [Required, Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;
}