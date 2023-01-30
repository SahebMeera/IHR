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
    public class CountryService : ICountryService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;

        public CountryService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
      
        public async Task<Response<IEnumerable<Country>>> GetCountries()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Country>>>("Country");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Country>>();
            }
        }

        public async Task<Response<Country>> GetCountryByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<Country>>($"Country/{id}");
        }
        public async Task<Response<Country>> UpdateCountry(int id, Country updateCountry)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PutJsonAsync<Response<Country>>($"Country/{id}", updateCountry);
        }
        public async Task<Response<Country>> SaveCountry(Country saveCountry)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<Response<Country>>($"Country", saveCountry);
        }
    }
}
