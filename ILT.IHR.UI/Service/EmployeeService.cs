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
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public EmployeeService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<Employee>>> GetEmployees()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Employee>>>("Employee");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Employee>>();
            }
        }
        public async Task<Response<IEnumerable<Employee>>> GetEmployeeInfo()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Employee>>>("Employee/GetEmployeeInfo");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Employee>>();
            }
        }
        public async Task<Response<IEnumerable<Department>>> GetDepartments()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Department>>>("Department");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Department>>();
            }
        }
        public async Task<Response<Employee>> GetEmployeeByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Employee>>($"Employee/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Employee>();
            }
        }
        public async Task<Response<Employee>> UpdateEmployee(int id, Employee updateEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Employee>>($"Employee/{id}", updateEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Employee>();
            }
            
        }
        public async Task<Response<Employee>> SaveEmployee(Employee saveEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<Employee>>($"Employee", saveEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Employee>();
            }
        }
    }
}