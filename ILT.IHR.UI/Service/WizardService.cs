using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components.Authorization;

namespace ILT.IHR.UI.Service
{
    public class WizardService : IWizardService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;

        public WizardService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<ProcessWizard>>> GetWizards()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<ProcessWizard>>>("ProcessWizard");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<ProcessWizard>>();
            }
        }

        public async Task<Response<ProcessWizard>> GetWizardByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<ProcessWizard>>($"ProcessWizard/{id}");
        }
        public async Task<Response<ProcessWizard>> UpdateWizard(int id, ProcessWizard updateWizard)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PutJsonAsync<Response<ProcessWizard>>($"ProcessWizard/{id}", updateWizard);
        }
        public async Task<Response<ProcessWizard>> SaveWizard(ProcessWizard saveWizard)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<Response<ProcessWizard>>($"ProcessWizard", saveWizard);
        }
    }
}
