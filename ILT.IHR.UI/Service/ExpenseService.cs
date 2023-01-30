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
    public class ExpenseService : IExpenseService
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        
        public ExpenseService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<Expense>>> GetExpenses()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<Expense>>>("Expense");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<Expense>>();
            }
        }
        public async Task<Response<Expense>> GetExpenseByIdAsync(int Id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<Expense>>($"Expense/{Id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Expense>();
            }

        }
        public async Task<Response<Expense>> UpdateExpense(int Id, Expense updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<Expense>>($"Expense/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Expense>();
            }
        }

        public async Task<Response<Expense>> DeleteExpense(int id)
        {
            Response < Expense > response =  new Response<Expense>();
            await Task.Delay(1000);

            return  response;
             // return await httpClient.DeleteAsync<Response<Expense>>($"Expense/{id}");
        }

        public async Task<Response<Expense>> SaveExpense(Expense obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<Expense>>("Expense", obj);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<Expense>();
            }
        }

        public async Task<Response<FileDownloadResponse>> DownloadFile(string Client, string FileName)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var a = await httpClient.GetJsonAsync<Response<FileDownloadResponse>>($"Expense/DownloadImage?Client=" + Client + "&FileName=" + FileName);
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
