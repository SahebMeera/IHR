using Blazored.Toast.Services;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeDirectDeposit
{
    public class AddEditDirectDepositBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IDirectDepositService DirectDepositService { get; set; } //Service
        public List<ListValue> AccountTypeList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateDirectDeposits { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        private int DirectDepositId { get; set; }
        protected bool isShow { get; set; }
        public DirectDeposit directDeposit = new DirectDeposit();
        public ILT.IHR.DTO.User user;
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission NPIPermission { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;

        public bool isViewPermissionForNPIRole { get; set; } = false;
        public bool isEditPermissionForNPIRole { get; set; } = false;
        public string routingNumber { get; set; }
        public string accountNumber { get; set; }
        public string routingNumberValue { get; set; }
        public string accountNumberValue { get; set; }

        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            isViewPermissionForNPIRole = NPIPermission.View;
            isEditPermissionForNPIRole = NPIPermission.Update;

            await LoadDropDown();
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
        public void Show(int Id, int employeeId)
        {
            DirectDepositId = Id;
            directDeposit.EmployeeID = employeeId;
            ResetDialog();
            if (DirectDepositId != 0)
            {
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(DirectDepositId);
            }
            else
            {
                isShow = EmployeeInfoRolePermission.Add;
                StateList = new List<State>();
                Title = "Add";
                routingNumber = "***********";
                accountNumber = "***********";
                directDeposit.RoutingNumber = routingNumber;
                directDeposit.AccountNumber = accountNumber;
                directDeposit.EmployeeID = employeeId;
                loadEmployeeData(employeeId);
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
        }

        protected async Task loadEmployeeData(int employeeId)
        {
            if (employeeId != 0)
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await EmployeeService.GetEmployeeByIdAsync(employeeId) as Response<ILT.IHR.DTO.Employee>;
                if (resp.MessageType == MessageType.Success)
                {
                    if (resp.Data.EmployeeID != 0)
                    {
                        directDeposit.Country = resp.Data.Country;
                        if (directDeposit.Country != null)
                        {
                            await GetStates(directDeposit.Country);
                        }
                        else
                        {
                            StateList = new List<State>();
                        }
                    }
                }
                StateHasChanged();
            }
        }

        private async Task LoadDropDown()
        {

            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                AccountTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ACCOUNTTYPE).ToList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
        }
       
        protected async Task onChangeCountry(ChangeEventArgs e)
        {
            country = Convert.ToString(e.Value);
            if (!String.IsNullOrEmpty(country))
            {
                await GetStates(country);
            }
            else
            {
                StateList.Clear();
            }
       }
        private async Task GetStates(string country)
        {
            countryId = CountryList != null? CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID : 0;
            if (countryId != 0 && countryId != null)
            {
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<DirectDeposit> resp = new Response<DirectDeposit>();
            resp = await DirectDepositService.GetDirectDepositByIdAsync(Id);
            if(resp.MessageType == MessageType.Success)
            directDeposit = resp.Data;
            routingNumberValue = NPIPermission.View == false ? directDeposit.RoutingNumber : "***********";
            accountNumberValue = NPIPermission.View == false ? directDeposit.AccountNumber : "***********";
            directDeposit.RoutingNumber = NPIPermission.View == true ? directDeposit.RoutingNumber : "***********";
            directDeposit.AccountNumber = NPIPermission.View == true ? directDeposit.AccountNumber : "***********";
           
            if (directDeposit.Country != null)
            {
                await GetStates(directDeposit.Country);
            } else
            {
                StateList = new List<State>();
            }
            
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }
        protected async Task SaveDirectDeposit()
        {
            directDeposit.EmployeeName = EmployeeName;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (DirectDepositId == 0)
            {
                directDeposit.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await DirectDepositService.SaveDirectDeposit(directDeposit);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Direct Deposit saved successfully", "");
                    UpdateDirectDeposits.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (DirectDepositId != 0)
            {
                directDeposit.RoutingNumber = directDeposit.RoutingNumber.StartsWith("*") ? routingNumberValue : directDeposit.RoutingNumber;
                directDeposit.AccountNumber = directDeposit.AccountNumber.StartsWith("*") ? accountNumberValue : directDeposit.AccountNumber;
                directDeposit.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await DirectDepositService.UpdateDirectDeposit(DirectDepositId, directDeposit);
                if (result.MessageType == MessageType.Success)
                {
                   
                    toastService.ShowSuccess("Direct Deposit saved successfully", "");
                    UpdateDirectDeposits.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            isSaveButtonDisabled = false;
        }
        public void Cancel()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            directDeposit = new DirectDeposit { };
        }

        protected async Task onAccountTypeChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && AccountTypeList != null)
            {
                var accountType = Convert.ToInt32(e.Value);
                directDeposit.AccountType = AccountTypeList.Find(x => x.ListValueID == accountType).ValueDesc;
            }
        }
    }
}
