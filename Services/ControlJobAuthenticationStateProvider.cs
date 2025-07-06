using MudBlazor6.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MudBlazor6.Services
{
    // сервис представляет средства для аутентификации пользователя
    // реализует методы Login и Logout
    // CurrentUser содержит текущего подтвержденного именем и паролем пользователя и его роль
    public class ControlJobAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly ControlJobUserService _controlJobUserService;

        public User? CurrentUser { get; set; } = new();

        public ControlJobAuthenticationStateProvider(ControlJobUserService controlJobUserService)
        {
            AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
            this._controlJobUserService = controlJobUserService;
        }

        private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
        {
            var authenticationState = await task;
            if (authenticationState != null) CurrentUser = User.FromClaimsPrincipal(authenticationState.User);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = new ClaimsPrincipal();
            var user = await _controlJobUserService.FetchUserFromBrowserAsync();
            if (user != null)
            {
                var authenticatedUser=await _controlJobUserService.FindUserFromDBAsync(user.UserName, user.Password);
                CurrentUser = authenticatedUser;
                if (authenticatedUser != null) principal=authenticatedUser.ToClaimsPrincipal();
            }
            return new(principal);
        }

        public async Task LoginAsync(string username, string password)
        {
            var principal =new ClaimsPrincipal();
            var user=await _controlJobUserService.FindUserFromDBAsync(username, password);
            CurrentUser= user;
            if (user != null) principal = user.ToClaimsPrincipal();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));   
        }

        public async Task LogoutAsync()
        {
            await _controlJobUserService.ClearBrowserUserDataAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
        }


        public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync; 
    }
}
