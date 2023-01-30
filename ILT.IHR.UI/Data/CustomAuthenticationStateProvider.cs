using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        private ISessionStorageService _sessionStorageService;
        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorageService, NavigationManager navigationManager)
        {
            _sessionStorageService = sessionStorageService;
            NavigationManager = navigationManager;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var email = await _sessionStorageService.GetItemAsync<string>("email");
            ClaimsIdentity identity;
            if (email != null)
            {
                identity = new ClaimsIdentity(new[]
               {
                   new Claim(ClaimTypes.Name, email),
               }, "apiauth_type");
            }
            else
            {
                identity = new ClaimsIdentity();
            }           

            var user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));
        }

        public void MarkUserAsAuthenticated(string email)
        {
           var identity = new ClaimsIdentity(new[]
           {
               new Claim(ClaimTypes.Name, email),
           }, "apiauth_type");

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            _sessionStorageService.RemoveItemAsync("email");
            _sessionStorageService.RemoveItemAsync("token");
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        public void SessionExpired(string e)
        {
            if (e.ToString().Contains("401"))
            {
                _sessionStorageService.RemoveItemAsync("email");
                _sessionStorageService.RemoveItemAsync("token");
                var identity = new ClaimsIdentity();
                var user = new ClaimsPrincipal(identity);
                NavigationManager.NavigateTo("/login", forceLoad: true);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
        }
    }
}
