using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using ILT.IHR.DTO;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ILT.IHR.UI.Service
{
    public class TimeSheetService : ITimeSheetService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;

        private ISessionStorageService _sessionStorageService;

        public TimeSheetService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }


        public async Task<Response<IEnumerable<TimeSheet>>> GetTimeSheets(int EmployeeID, int SubmittedByID, int? StatusID = null)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<TimeSheet>>>($"TimeSheet/?EmployeeID={EmployeeID}&SubmittedByID={SubmittedByID}&StatusID={StatusID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<TimeSheet>>();
            }
        }
        public async Task<Response<TimeSheet>> GetTimeSheetByIdAsync(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<TimeSheet>>($"TimeSheet/{Id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<TimeSheet>();
            }
        }
        public async Task<Response<TimeSheet>> UpdateTimeSheet(int Id, TimeSheet updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<TimeSheet>>($"TimeSheet/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<TimeSheet>();
            }
        }

        public async Task<Response<TimeSheet>> DeleteTimeSheet(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            await httpClient.DeleteAsync($"TimeSheet/{id}");
            Response<TimeSheet> response = new Response<TimeSheet>();
            await Task.Delay(1000);
            return response;
        }

        public async Task<Response<TimeSheet>> SaveTimeSheet(TimeSheet obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<TimeSheet>>("TimeSheet", obj);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<TimeSheet>();
            }
        }

        public async Task<Response<FileDownloadResponse>> DownloadFile(string Client, Guid Doc)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var a = await httpClient.GetJsonAsync<Response<FileDownloadResponse>>($"Timesheet/DownloadFile?Client="+ Client +"&Doc=" + Doc);
                return a;
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<FileDownloadResponse>();
            }
        }
    }
}
