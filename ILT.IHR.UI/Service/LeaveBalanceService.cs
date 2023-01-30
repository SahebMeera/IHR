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
    public class LeaveBalanceService: ILeaveBalanceService
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        public LeaveBalanceService(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<Response<IEnumerable<LeaveBalance>>> GetLeaveBalance(int? EmployeID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                if (EmployeID == 0)
                {
                    return await httpClient.GetJsonAsync<Response<IEnumerable<LeaveBalance>>>("LeaveBalance");
                }
                return await httpClient.GetJsonAsync<Response<IEnumerable<LeaveBalance>>>($"LeaveBalance/?EmployeeID={EmployeID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<LeaveBalance>>();
            }
        }
        public async Task<Response<LeaveBalance>> GetLeaveBalanceById(int ID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<LeaveBalance>>($"LeaveBalance/{ID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<LeaveBalance>();
            }
        }
        public async Task<Response<LeaveBalance>> updateLeaveBalance(int Id, LeaveBalance updateObject)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<LeaveBalance>>($"LeaveBalance/{Id}", updateObject);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<LeaveBalance>();
            }
        }
        public async Task<DataTable> GetReportLeaveInfo(DTO.Report reportReq, string LeaveSummaryStatus)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var resp = await httpClient.PostJsonAsync<Response<List<LeaveBalance>>>("LeaveBalance/GetLeavesCount", reportReq);
                if (!String.IsNullOrEmpty(LeaveSummaryStatus) && resp.Data != null)
                {
                    if (LeaveSummaryStatus == "All")
                    {
                        string json = JsonConvert.SerializeObject(resp.Data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                    else if (LeaveSummaryStatus != "Active")
                    {
                        var data = resp.Data.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                        string json = JsonConvert.SerializeObject(data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                    else
                    {
                        var data = resp.Data.Where(x => x.TermDate == null || x.TermDate > DateTime.Now).ToList();
                        string json = JsonConvert.SerializeObject(data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                }
                else
                {
                    string json = JsonConvert.SerializeObject(resp.Data);
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                    return dt;
                }
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new DataTable();
            }
        }
        public async Task<DataTable> GetReportLeaveDetailInfo(DTO.Report reportReq, string LeaveDetailStatus)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var resp = await httpClient.PostJsonAsync<Response<List<LeaveBalance>>>("LeaveBalance/GetLeaveDetail", reportReq);
                if (!String.IsNullOrEmpty(LeaveDetailStatus) && resp.Data != null)
                {
                    if (LeaveDetailStatus == "All")
                    {
                        string json = JsonConvert.SerializeObject(resp.Data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                    else if (LeaveDetailStatus != "Active")
                    {
                        var data = resp.Data.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                        string json = JsonConvert.SerializeObject(data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                    else
                    {
                        var data = resp.Data.Where(x => x.TermDate == null || x.TermDate > DateTime.Now).ToList();
                        string json = JsonConvert.SerializeObject(data);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                        return dt;
                    }
                } else
                {
                    string json = JsonConvert.SerializeObject(resp.Data);
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                    return dt;
                }
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new DataTable();
            }
        }
    }
}
