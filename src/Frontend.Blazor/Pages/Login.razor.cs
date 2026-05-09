using Frontend.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Frontend.Blazor.Pages;

public partial class Login: ComponentBase
{
    private readonly LoginModel _model = new();
    private bool _loginFailed;
    
    protected override void OnInitialized()
    {
        Console.WriteLine("Hello");
    }

    private async Task OnLoginAsync(EditContext editContext)
    {
        if (editContext.Validate() is false)
        {
            _loginFailed = true;
            return;
        }

        _loginFailed = false;
        var loginResult = await LoginService.LoginAsync(_model);
        if (loginResult)
        {
            _loginFailed = false;
            Navigation.NavigateTo("/", true);
        }
        else
        {
            _loginFailed = true;
        }
    }
}