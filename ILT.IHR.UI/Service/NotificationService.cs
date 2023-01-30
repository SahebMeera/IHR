using System;
using Blazored.SessionStorage;
using ILT.IHR.DTO;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace ILT.IHR.UI.Service
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public NotificationService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<Response<Notification>> SaveNotification(int Id, Notification notification)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Notification>>($"Notification/{Id}", notification);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Notification>();
            }
        }
    }
}
