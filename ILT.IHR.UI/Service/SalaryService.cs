using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using ILT.IHR.UI.ExtensionMethods;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ILT.IHR.UI.Service
{
    public class SalaryService: ISalaryService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public SalaryService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<Salary>> GetSalaryById(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Salary>>($"Salary/{Id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Salary>();
            }
        }
            
        public async Task<Response<Salary>> UpdateSalary(int Id,Salary updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Salary>>($"Salary/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Salary>();
            }
        }

        public async Task DeleteSalary(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                await httpClient.DeleteAsync($"Salary/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
            }
        }

        public async Task<Response<Salary>> SaveSalary(Salary obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<Salary>>("Salary", obj);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Salary>();
            }
        }
        
    }
}