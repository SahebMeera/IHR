using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using ILT.IHR.UI.ExtensionMethods;

namespace ILT.IHR.UI.Service
{
    public class StateService : IStateService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;

        public StateService(HttpClient httpClient, ISessionStorageService sessionStorageService)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
        }
        public async Task<IEnumerable<State>> GetStates()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<IEnumerable<State>>("State");
        }

        public async Task<State> GetStateByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<State>($"State/{id}");
        }
        public async Task<State> UpdateState(int id, State updateState)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PutJsonAsync<State>($"State/{id}", updateState);
        }
        public async Task<State> SaveState(State saveCountry)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<State>($"State", saveCountry);
        }
    }
}
