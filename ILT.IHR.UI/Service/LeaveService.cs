using Blazored.SessionStorage;
using ILT.IHR.DTO;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public class LeaveService : ILeaveService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;

        public LeaveService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<Leave>>> GetLeave(string Parameter = "", int ID = 0)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                if (Parameter.Length == 0)
                {
                    return await httpClient.GetJsonAsync<Response<IEnumerable<Leave>>>("Leave");
                }
                return await httpClient.GetJsonAsync<Response<IEnumerable<Leave>>>($"Leave/?{Parameter}={ID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Leave>>() ;
            }
           
        }
        public async Task<Response<Leave>> GetLeaveByIdAsync(int id, int EmployeeID, int ApproverID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Leave>>($"Leave/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Leave>();
            }
        }
        public async Task<Response<Leave>> UpdateLeave(int Id, Leave updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Leave>>($"Leave/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Leave>();
            }
        }

        public async Task<Response<Leave>> SaveLeave(Leave obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<Leave>>("Leave", obj);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Leave>();
            }
        }

        public async Task<Response<Leave>> GetLeaveDays(string clientId, int employeeId, DateTime startDate, DateTime endDate, bool includesHalfDay)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Leave>>($"Leave/GetLeaveDays?clientId={clientId}&employeeId={employeeId}&startDate={startDate.ToString("MM/dd/yyyy")}&endDate={endDate.ToString("MM/dd/yyyy")}&includesHalfDay={includesHalfDay}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Leave>();
            }
        }
    }
}
