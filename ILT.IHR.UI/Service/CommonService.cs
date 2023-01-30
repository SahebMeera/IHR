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
    public class CommonService : ICommonService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        
        public CommonService(HttpClient httpClient, ISessionStorageService sessionStorageService)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
        }
        public async Task<Common> SendEmail(Common obj)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            return await httpClient.PostJsonAsync<Common>("Common", obj);
        }
    }
}
