namespace BlazorApp.Client.State;

// save the current "user"
public class UserState
{
    private string? _currentUser;
    public string? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnUserChanged?.Invoke(_currentUser);
            }
        }
    }
    
    public event Action<string?>? OnUserChanged;
}