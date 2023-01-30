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
using System.Data;
using Newtonsoft.Json;

namespace ILT.IHR.UI.Service
{
    public class FormI9Service : IFormI9Service
    {
        private readonly HttpClient httpClient;
        private ISessionStorageService _sessionStorageService;
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public FormI9Service(HttpClient httpClient, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<IEnumerable<FormI9>>> GetFormI9(int EmployeeID)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<IEnumerable<FormI9>>>($"FormI9/?EmployeeID={EmployeeID}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<IEnumerable<FormI9>>();
            }
        }
        
        public async Task<Response<FormI9>> GetFormI9ByIdAsync(int id)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.GetJsonAsync<Response<FormI9>>($"FormI9/{id}");
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<FormI9>();
            }
        }
        public async Task<Response<FormI9>> UpdateFormI9(int id, FormI9 updateEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PutJsonAsync<Response<FormI9>>($"FormI9/{id}", updateEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<FormI9>();
            }
            
        }
        public async Task<Response<FormI9>> SaveFormI9(FormI9 saveEmployee)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                return await httpClient.PostJsonAsync<Response<FormI9>>($"FormI9", saveEmployee);
            }
            catch (Exception e)
            {
                ((Data.CustomAuthenticationStateProvider)_authenticationStateProvider).SessionExpired(e.ToString());
                return new Response<FormI9>();
            }
        }
        public async Task<DataTable> GetI9ExpiryForm(DateTime expirydate)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("token");
            httpClient.SetAuthorizationHeader(token);
            try
            {
                var resp = await httpClient.GetJsonAsync<Response<List<FormI9>>>($"FormI9/GetI9ExpiryForm?expirydate={expirydate.ToString("MM / dd / yyyy")}");
                if(resp.Data != null)
                {
                    resp.Data.ForEach(x =>
                    {
                        if (x.I94ExpiryDate == null && (x.ListCDocumentTitleID == 24 || x.ListCDocumentTitleID == 25))
                        {
                            x.I94ExpiryDate = DateTime.Parse("12/31/9999");
                        }
                        if ((x.ListAExpirationDate == null || x.ListADocumentTitle == null) && (x.ListCDocumentTitleID == 24 || x.ListCDocumentTitleID == 25))
                        {
                            x.ListAExpirationDate = DateTime.Parse("12/31/9999");
                            x.ListADocumentTitle = "Social Security Account Number Card";
                        } 
                        if ((x.ListBExpirationDate == null || x.ListBDocumentTitle == null) && (x.ListCDocumentTitleID == 24 || x.ListCDocumentTitleID == 25))
                        {
                            x.ListBExpirationDate = DateTime.Parse("12/31/9999");
                            x.ListBDocumentTitle = "Social Security Account Number Card";
                        }
                        
                        if ((x.ListCExpirationDate == null || x.ListCDocumentTitle == null) && (x.ListCDocumentTitleID == 24 || x.ListCDocumentTitleID == 25))
                        {
                            x.ListCExpirationDate = DateTime.Parse("12/31/9999");
                            x.ListCDocumentTitle = "Social Security Account Number Card".ToString();
                        }
                        x.InputDate = Convert.ToDateTime(expirydate);
                    });
                }
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