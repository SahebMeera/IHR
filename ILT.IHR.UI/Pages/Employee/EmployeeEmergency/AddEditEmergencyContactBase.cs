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

namespace ILT.IHR.UI.Pages.Employee.EmployeeEmergency
{
    public class AddEditEmergencyContactBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service   
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]

        public IContactService ContactService { get; set; } //Service
        public List<ListValue> ContactTypeList { get; set; }  // Table APi Data
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int ContactId { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public Contact Contact = new Contact();
        public ILT.IHR.DTO.User user;
        protected bool isShow { get; set; }
        public string InputID = "input-id";

        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;

        public bool isSaveButtonDisabled { get; set; } = false;


        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }
       
        public async void Show(int Id, int employeeId)
        {
            ContactId = Id;
            Contact.EmployeeID = employeeId;
            ResetDialog();
            if (ContactId != 0)
            {
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(ContactId, employeeId);
            }
            else
            {
                isShow = EmployeeInfoRolePermission.Add;
                Title = "Add";
                Contact.EmployeeID = employeeId;
                loadEmployeeData(employeeId);
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
              
            }
        }
        public async Task Focus()
        {
            await JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
            StateHasChanged();
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
                        Contact.Country = resp.Data.Country;
                            if (Contact.Country != null)
                            {
                                await GetStates(Contact.Country);
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
                ContactTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.CONTACTTYPE).ToList();
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
            countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }
        private async Task GetDetails(int Id, int employeeId)
        {
          
            Response<ILT.IHR.DTO.Contact> resp = new Response<ILT.IHR.DTO.Contact>();
            resp = await ContactService.GetContactByIdAsync(Id, employeeId);
            if(resp.MessageType == MessageType.Success)
            Contact = resp.Data;
            if (Contact.Country != null)
            {
                await GetStates(Contact.Country);
            }
            else
            {
                StateList = new List<State>();
            }
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }
        protected async Task SaveDependent()
        {
            Contact.EmployeeName = EmployeeName;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (ContactId == 0)
            {
                Contact.CreatedBy = user.FirstName + " " + user.LastName;
                // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(Contact);
                var result = await ContactService.SaveContact(Contact);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Contact saved successfully", "");
                ListValueUpdated.InvokeAsync(true);
                Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (ContactId != 0)
            {
                Contact.ModifiedBy = user.FirstName + " " + user.LastName;
                // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(Contact);
                var result = await ContactService.UpdateContact(ContactId, Contact);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Contact saved successfully", "");
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
            Contact = new Contact { };
        }
        protected async Task onContactChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && ContactTypeList != null)
            {
                var contactType = Convert.ToInt32(e.Value);
                Contact.ContactType = ContactTypeList.Find(x => x.ListValueID == contactType).ValueDesc;
            }
        }


    }
}
