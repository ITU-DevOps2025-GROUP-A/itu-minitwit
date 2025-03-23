namespace Web.Services;

public class UserState
{
    public event Action? OnChange;
    
    public string? Username { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Username);

    public void SetUser(string username)
    {
        Username = username;
        NotifyStateChanged();
    }

    public void ClearUser()
    {
        Username = null;
        NotifyStateChanged();
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}