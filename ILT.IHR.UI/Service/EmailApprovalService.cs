using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using ILT.IHR.UI.ExtensionMethods;

namespace ILT.IHR.UI.Service
{
    public class EmailApprovalService : IEmailApprovalService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;

        public EmailApprovalService(HttpClient httpClient, ISessionStorageService sessionStorageService)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
        }
        public async Task<Response<IEnumerable<EmailApproval>>> GetEmailApprovals()
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<IEnumerable<EmailApproval>>>("EmailApproval");
        }

        public async Task<Response<EmailApproval>> GetEmailApprovalByIdAsync(Guid id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetJsonAsync<Response<EmailApproval>>($"EmailApproval/{id}");
        }
        public async Task<Response<EmailApproval>> UpdateEmailApproval(int id, EmailApproval updateEmailApproval)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PutJsonAsync<Response<EmailApproval>>($"EmailApproval/{id}", updateEmailApproval);
        }
        public async Task<Response<EmailApproval>> SaveEmailApproval(EmailApproval saveEmailApproval)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<Response<EmailApproval>>($"EmailApproval", saveEmailApproval);
        }

        public  async Task<string> EamilApprovalAction(string ClientID, Guid LinkID, string Value, string Module="Timesheet")
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.GetStringAsync($"EmailApproval/{ClientID}/{LinkID}/{Value}/{Module}");
        }
    }
}
