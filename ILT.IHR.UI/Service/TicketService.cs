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
    public class TicketService : ITicketService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;

        private ISessionStorageService _sessionStorageService;

        public TicketService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<Ticket>>> GetTickets()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Ticket>>>("Ticket");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Ticket>>();
            }
        }
        public async Task<Response<IEnumerable<Ticket>>> GetTicketsList(int RequestedByID, int? AssignedToID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Ticket>>>($"Ticket/?RequestedByID={RequestedByID}&AssignedToID={AssignedToID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Ticket>>();
            }
        }

        //public async Task<Response<IEnumerable<TimeSheet>>> GetTimeSheets(int EmployeeID, int SubmittedByID, int? StatusID = null)
        //{
        //    var token = await _sessionStorageService.GetItemAsync<string>("token");
        //    httpClient.SetAuthorizationHeader(token);
        //    try
        //    {
        //        return await httpClient.GetJsonAsync<Response<IEnumerable<TimeSheet>>>($"TimeSheet/?EmployeeID={EmployeeID}&SubmittedByID={SubmittedByID}&StatusID={StatusID}");
        //    }
        //    catch (Exception e)
        //    {
        //        ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
        //        return new Response<IEnumerable<TimeSheet>>();
        //    }
        //}



        public async Task<Response<Ticket>> GetTicketByIdAsync(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Ticket>>($"Ticket/{Id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Ticket>();
            }
        }
        public async Task<Response<Ticket>> UpdateTicket(int Id, Ticket updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Ticket>>($"Ticket/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Ticket>();
            }
        }

        public async Task<Response<Ticket>> DeleteTicket(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            await httpClient.DeleteAsync($"Ticket/{id}");
            Response<Ticket> response = new Response<Ticket>();
            await Task.Delay(1000);
            return response;
        }

       
        public async Task<Response<Ticket>> SaveTicket(Ticket obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<Ticket>>("Ticket", obj);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Ticket>();
            }
        }
    }
}
