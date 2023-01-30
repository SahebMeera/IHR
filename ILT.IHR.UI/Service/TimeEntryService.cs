using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using ILT.IHR.DTO;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components;

namespace ILT.IHR.UI.Service
{
    public class TimeEntryService : ITimeEntryService
    {
        private readonly HttpClient httpClient;

        private ISessionStorageService _sessionStorageService;

        public TimeEntryService(HttpClient httpClient, ISessionStorageService sessionStorageService)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
        }

        public async Task<Response<List<TimeEntry>>> GetTimeEntries()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<List<TimeEntry>>>("TimeEntry");
        }
        public async Task<Response<TimeEntry>> GetTimeEntryByIdAsync(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<TimeEntry>>($"TimeEntry/{Id}");
        }
        public async Task<Response<TimeEntry>> UpdateTimeEntry(int Id, TimeEntry updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PutJsonAsync<Response<TimeEntry>>($"TimeEntry/{Id}", updateObject);
        }

        public async Task<Response<TimeEntry>> DeleteTimeEntry(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            await httpClient.DeleteAsync($"TimeSheet/{id}");
            Response<TimeEntry> response = new Response<TimeEntry>();
            await Task.Delay(1000);
            return response;
        }

        public async Task<Response<TimeEntry>> SaveTimeEntry(TimeEntry obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<Response<TimeEntry>>("TimeEntry", obj);
        }
    }
}
