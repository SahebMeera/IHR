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
using System.Data;
using Newtonsoft.Json;

namespace ILT.IHR.UI.Service
{
    public class EmployeeSkillService : IEmployeeSkillService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public EmployeeSkillService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<EmployeeSkill>>> GetEmployeeSkill(int EmployeeID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<EmployeeSkill>>>($"EmployeeSkill/?EmployeeID={EmployeeID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<EmployeeSkill>>();
            }
        }
        
        public async Task<Response<EmployeeSkill>> GetEmployeeSkillByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<EmployeeSkill>>($"EmployeeSkill/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeSkill>();
            }
        }
        public async Task<Response<EmployeeSkill>> UpdateEmployeeSkill(int id, EmployeeSkill updateEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<EmployeeSkill>>($"EmployeeSkill/{id}", updateEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeSkill>();
            }
            
        }
        public async Task<Response<EmployeeSkill>> SaveEmployeeSkill(EmployeeSkill saveEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<EmployeeSkill>>($"EmployeeSkill", saveEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<EmployeeSkill>();
            }
        }
        public async Task DeleteEmployeeSkill(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            await httpClient.DeleteAsync($"EmployeeSkill/{Id}");
        }
    }
}