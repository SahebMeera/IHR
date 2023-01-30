using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components.Authorization;

namespace ILT.IHR.UI.Service
{
    public class EmployeeW4Service : IEmployeeW4Service
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public EmployeeW4Service(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<EmployeeW4>>> GetEmployeesW4(int EmployeeID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<EmployeeW4>>>($"EmployeeW4/?EmployeeID={EmployeeID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<EmployeeW4>>();
            }
        }
        
        public async Task<Response<EmployeeW4>> GetEmployeeW4ByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<EmployeeW4>>($"EmployeeW4/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeW4>();
            }
        }
        public async Task<Response<EmployeeW4>> UpdateEmployeeW4(int id, EmployeeW4 updateEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<EmployeeW4>>($"EmployeeW4/{id}", updateEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeW4>();
            }
            
        }
        public async Task<Response<EmployeeW4>> SaveEmployeeW4(EmployeeW4 saveEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<EmployeeW4>>($"EmployeeW4", saveEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeW4>();
            }
        }
    }
}