using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using System.Linq;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Company
{
    public class AddEditEndClientBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service        
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int EndClientId { get; set; }       
        public List<ListValue> CompanyTypeList { get; set; }  // Table APi Data       
        public IEnumerable<ILT.IHR.DTO.EndClient> EndClients { get; set; }  // Table APi Data
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public DTO.User user { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
       
        public ILT.IHR.DTO.EndClient endclient = new ILT.IHR.DTO.EndClient();
        [Inject]
        public ILookupService LookupService { get; set; } //Service 
        public IEnumerable<ILT.IHR.DTO.EndClient> EndClientList { get; set; }  // Table APi Data
        public bool isSaveButtonDisabled { get; set; } = false;




        protected override async Task OnInitializedAsync()
        {
            EndClientList = new List<ILT.IHR.DTO.EndClient> { };
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            await LoadEndClientList();
            await LoadDropDown();

        }
        protected async Task LoadEndClientList()
        {

            var reponses = (await EndClientService.GetEndClients());
            if (reponses.MessageType == MessageType.Success)
            {
                EndClientList = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

        }

        public void Show(int Id)
        {
            EndClientId = Id;
            ResetDialog();            
            if (EndClientId != 0)
            {
                GetDetails(EndClientId);
            }
            else
            {
                Title = "Add";
                isfirstElementFocus = true;
                ErrorMessage = "";
                ShowDialog = true;
                StateHasChanged();
            }
        }

        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            
            if (resp.MessageType == MessageType.Success)
            {                
                CompanyTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.COMPANYTYPE).ToList();                
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }

        }
        protected async Task onChangeCountry(ChangeEventArgs e)
        {
            if(e.Value != "")
            {
                country = Convert.ToString(e.Value);
                await GetStates(country);
            }
            else
            {
                StateList.Clear();
            }
        }        
        
        private async Task GetStates(string country)
        {
            countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }
        private async Task GetDetails(int Id)
        {
            Response <ILT.IHR.DTO.EndClient> resp = new Response<ILT.IHR.DTO.EndClient>();
            resp = await EndClientService.GetEndClientByIdAsync(Id) as Response<ILT.IHR.DTO.EndClient>;
            if (resp.MessageType == MessageType.Success)
            {
                endclient = resp.Data;
                if (endclient.Country != null)
                {
                    await GetStates(endclient.Country);
                }
                else
                {
                    StateList = new List<State>();
                }                
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            Title = "Edit";
            isfirstElementFocus = true;
            ErrorMessage = "";
            ShowDialog = true; 
            StateHasChanged();
        }

        public string ErrorMessage;
        protected async Task checkNameExist()
        {
            ErrorMessage = "";
            if (EndClientId != 0)
            {
                if (!String.IsNullOrEmpty(endclient.Name))
                {
                    if ((EndClientList.ToList().FindAll(x => x.EndClientID != endclient.EndClientID && x.Name.ToUpper() == endclient.Name.Trim().ToUpper()).Count > 0))
                    {
                        ErrorMessage = "Company Name already exists in the system";
                    }
                    else
                    {
                        SaveEndClient();
                    }
                }
            }
            else
            {
                if ((EndClientList.ToList().FindAll(x => x.Name.ToUpper() == endclient.Name.Trim().ToUpper()).Count > 0))
                {
                    ErrorMessage = "Company Name already exists in the system";
                }
                else
                {
                    SaveEndClient();
                }
            }

        }

        protected async Task SaveEndClient()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (EndClientId == 0)
            {                
                endclient.CreatedBy = user.FirstName + " " + user.LastName;                
                var result = await EndClientService.SaveEndClient(endclient);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("End Client saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (EndClientId != 0)
            {                
                endclient.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await EndClientService.UpdateEndClient(EndClientId, endclient);
                if (result.MessageType == MessageType.Success)
                {
                        toastService.ShowSuccess("Company saved successfully", "");
                        ListValueUpdated.InvokeAsync(true);
                        Cancel();
                                    
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }

            isSaveButtonDisabled = false;
        }
        public string loadCompanyType(int companyTypeId)
        {
           return CompanyTypeList.Find(x => x.ListValueID == companyTypeId).ValueDesc;
        }       
        public void Cancel()
        {
            ShowDialog = false;
           // ListValueUpdated.InvokeAsync(true);
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            endclient = new ILT.IHR.DTO.EndClient { };
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }
    }
}
