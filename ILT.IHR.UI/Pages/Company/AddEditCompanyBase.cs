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
using Microsoft.AspNetCore.Hosting;
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.Company
{
    public class AddEditCompanyBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service
        public bool isSaveButtonDisabled { get; set; } = false;



        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int CompanyId { get; set; }
        [Parameter]
        public List<ListValue> PaymentTermList { get; set; }  // Table APi Data
        public List<ListValue> CompanyTypeList { get; set; }  // Table APi Data
        public List<ListValue> InvoicingPeriodList { get; set; }  // Table APi Data
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

        public ILT.IHR.DTO.Company company = new ILT.IHR.DTO.Company();
        public ILT.IHR.DTO.EndClient endclient = new ILT.IHR.DTO.EndClient();
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public bool isEndClientdisable = true;
        public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        public List<RolePermission> RolePermissions;
        public RolePermission CompanyRolePermission;

        protected override async Task OnInitializedAsync()
        {
            CompanyList = new List<ILT.IHR.DTO.Company> { };
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            CompanyRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.COMPANY);
            if (CompanyRolePermission.Update == true)
            {
                isSaveButtonDisabled = false;
            }
            else
            {
                isSaveButtonDisabled = true;
            }
            await LoadDropDown();
            await LoadList();
        }

        protected async Task LoadList()
        {

            var reponses = (await CompanyService.GetCompanies());
            if (reponses.MessageType == MessageType.Success)
            {
                CompanyList = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

        }

        public void Show(int Id)
        {
            CompanyId = Id;
            ResetDialog();
            isEndClientdisable = false;
            if (CompanyId != 0)
            {
                GetDetails(CompanyId);
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
                PaymentTermList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.PAYMENTTERM).ToList();
                CompanyTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.COMPANYTYPE).ToList();
                InvoicingPeriodList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.INVOICINGPERIOD).ToList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }

        }
        protected async Task onChangeCountry(ChangeEventArgs e)
        {
            if (e.Value != "")
            {
                country = Convert.ToString(e.Value);
                await GetStates(country);
            }
            else
            {
                StateList.Clear();
            }
        }

        protected async Task onChangeCompanyType(ChangeEventArgs e)
        {
            if (e.Value != "")
            {
                if (CompanyTypeList.Find(x => x.ListValueID == Convert.ToInt32(e.Value)).Value == "CLIENT")
                {
                    company.CompanyType = CompanyTypeList.Find(x => x.ListValueID == Convert.ToInt32(e.Value)).ValueDesc;
                    isEndClientdisable = false;
                }
                else
                {
                    company.CompanyType = CompanyTypeList.Find(x => x.ListValueID == Convert.ToInt32(e.Value)).ValueDesc;
                    isEndClientdisable = true;
                }
            }
            else
            {
                isEndClientdisable = true;
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
            Response<ILT.IHR.DTO.Company> resp = new Response<ILT.IHR.DTO.Company>();
            resp = await CompanyService.GetCompanyByIdAsync(Id) as Response<ILT.IHR.DTO.Company>;
            if (resp.MessageType == MessageType.Success)
            {
                company = resp.Data;
                if (company.Country != null)
                {
                    await GetStates(company.Country);
                }
                else
                {
                    StateList = new List<State>();
                }
                if (CompanyTypeList.Find(x => x.ListValueID == company.CompanyTypeID).Value == "CLIENT")
                {
                    isEndClientdisable = false;
                }
                else
                {
                    isEndClientdisable = true;
                }

                if (company.IsEndClient)
                {
                    var reponses = (await EndClientService.GetEndClients());
                    if (reponses.MessageType == MessageType.Success)
                    {
                        EndClients = reponses.Data;
                        endclient = EndClients.ToList().Find(ec => ec.CompanyID == company.CompanyID);
                        if (endclient == null)
                        {
                            endclient = new ILT.IHR.DTO.EndClient { };
                        }
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                    isEndClientdisable = true;
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
            if (CompanyId != 0)
            {
                if (!String.IsNullOrEmpty(company.Name))
                {
                    if ((CompanyList.ToList().FindAll(x => x.CompanyID != company.CompanyID && x.Name.ToUpper() == company.Name.Trim().ToUpper()).Count > 0))
                    {
                        ErrorMessage = "Company Name already exists in the system";
                    }
                    else
                    {
                        SaveCompany();
                    }
                }
            }
            else
            {
                if ((CompanyList.ToList().FindAll(x => x.Name.ToUpper() == company.Name.Trim().ToUpper()).Count > 0))
                {
                    ErrorMessage = "Company Name already exists in the system";
                }
                else
                {
                   SaveCompany();
                }
            }

        }

        protected async Task SaveCompany()
        {
            if (CompanyTypeList.Find(x => x.ListValueID == company.CompanyTypeID).Value != "CLIENT")
            {
                company.IsEndClient = false;
            }
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (CompanyId == 0)
            {
                //company.CreatedBy = "Admin";
                company.CreatedBy = user.FirstName + " " + user.LastName;
                // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(company); 
                var result = await CompanyService.SaveCompany(company);
                if (result.MessageType == MessageType.Success)
                {
                    if (company.IsEndClient)
                    {
                        endclient.CompanyID = result.Data.CompanyID;
                        endclient.Name = company.Name;
                        endclient.TaxID = company.TaxID;
                        endclient.Address1 = company.Address1;
                        endclient.Address2 = company.Address2;
                        endclient.City = company.City;
                        endclient.State = company.State;
                        endclient.Country = company.Country;
                        endclient.ZipCode = company.ZipCode;
                        endclient.CreatedBy = user.FirstName + " " + user.LastName;
                        var resultEndClient = await EndClientService.SaveEndClient(endclient);
                        if (resultEndClient.MessageType == MessageType.Success)
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
                    else
                    {
                        toastService.ShowSuccess("Company saved successfully", "");
                        ListValueUpdated.InvokeAsync(true);
                        Cancel();
                    }
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (CompanyId != 0)
            {
                //company.CreatedBy = "Admin";
                company.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await CompanyService.UpdateCompany(CompanyId, company);
                if (result.MessageType == MessageType.Success)
                {
                    if (endclient.EndClientID != 0)
                    {
                        endclient.CompanyID = result.Data.CompanyID;
                        endclient.Name = company.Name;
                        endclient.TaxID = company.TaxID;
                        endclient.Address1 = company.Address1;
                        endclient.Address2 = company.Address2;
                        endclient.City = company.City;
                        endclient.State = company.State;
                        endclient.Country = company.Country;
                        endclient.ZipCode = company.ZipCode;
                        endclient.ModifiedBy = user.FirstName + " " + user.LastName;
                        var resultEndClient = await EndClientService.UpdateEndClient(endclient.EndClientID, endclient);
                        if (resultEndClient.MessageType == MessageType.Success)
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
                    else if (company.IsEndClient)
                    {
                        endclient.CompanyID = result.Data.CompanyID;
                        endclient.Name = company.Name;
                        endclient.TaxID = company.TaxID;
                        endclient.Address1 = company.Address1;
                        endclient.Address2 = company.Address2;
                        endclient.City = company.City;
                        endclient.State = company.State;
                        endclient.Country = company.Country;
                        endclient.ZipCode = company.ZipCode;
                        endclient.CreatedBy = user.FirstName + " " + user.LastName;
                        var resultEndClient = await EndClientService.SaveEndClient(endclient);
                        if (resultEndClient.MessageType == MessageType.Success)
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
                    else
                    {
                        toastService.ShowSuccess("Company saved successfully", "");
                        ListValueUpdated.InvokeAsync(true);
                        Cancel();
                    }
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
        public string LoadpaymentTerm(int companyTypeId)
        {
            return PaymentTermList.Find(x => x.ListValueID == companyTypeId).ValueDesc;
        }
        public string LoadInvoicingPeriod(int companyTypeId)
        {
            return InvoicingPeriodList.Find(x => x.ListValueID == companyTypeId).ValueDesc;
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
            company = new ILT.IHR.DTO.Company { };
        }


        public bool altInvContPhone { get; set; } = false;
        public bool altContPhone { get; set; } = false;
        public void onChangeAltInvContPhone()
        {
            string phone = company.AlternateInvoiceContactPhone;
            if(phone != null)
            {
                if (phone.Length != 0 && phone.Length != 10)
                {
                    altInvContPhone = true;
                }
                else
                {
                    altInvContPhone = false;
                }
            } else
            {
                altInvContPhone = false;
            }
            
        }
        public void onChangeAltContPhone()
        {
            string phone = company.AlternateContactPhone;
            if (phone != null)
            {
                if (phone.Length != 0 && phone.Length != 10)
                {
                    altContPhone = true;
                }
                else
                {
                    altContPhone = false;
                }
            }
            else
            {
                altContPhone = false;
            }

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

        protected async Task onChangePaymentTerm(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && PaymentTermList != null)
            {
                var paymentTerm = Convert.ToInt32(e.Value);
                company.PaymentTerm = PaymentTermList.Find(x => x.ListValueID == paymentTerm).ValueDesc;
            }
        }

        protected async Task onChangeInvoicingPeriod(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && InvoicingPeriodList != null)
            {
                var invoicingPeriod = Convert.ToInt32(e.Value);
                company.InvoicingPeriod = InvoicingPeriodList.Find(x => x.ListValueID == invoicingPeriod).ValueDesc;
            }
        }
    }
}
