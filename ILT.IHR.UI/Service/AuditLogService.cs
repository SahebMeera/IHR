using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using ILT.IHR.DTO;
using ILT.IHR.UI.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace ILT.IHR.UI.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        public AuditLogService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<Response<IEnumerable<AuditLog>>> GetAuditLog(int? AuditLogID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                if (AuditLogID == 0)
                {
                    return await httpClient.GetJsonAsync<Response<IEnumerable<AuditLog>>>("AuditLog");
                }
                return await httpClient.GetJsonAsync<Response<IEnumerable<AuditLog>>>($"AuditLog/?AuditLogID={AuditLogID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<AuditLog>>();
            }
        }
        public async Task<Response<AuditLog>> GetAuditLogById(int ID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<AuditLog>>($"AuditLog/{ID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<AuditLog>();
            }
        }
        public async Task<Response<AuditLog>> updateAuditLog(int Id, AuditLog updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<AuditLog>>($"AuditLog/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<AuditLog>();
            }
        }
        public async Task<DataTable> GetReportAuditLogInfo(DTO.Report reportReq)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var resp = await httpClient.PostJsonAsync<Response<List<AuditLog>>>("AuditLog/GetAuditLogInfo", reportReq);
                string json = JsonConvert.SerializeObject(resp.Data);
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                return dt;
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new DataTable();
            }
        }
        
    }
}
