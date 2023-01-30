using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using System.Linq;
using Blazored.Toast.Services;
using Blazored.SessionStorage;

namespace ILT.IHR.UI.Pages.Employee.EmployeeDetails
{
    public class AddEditAddressBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Parameter]
        public EventCallback<EmployeeAddress> updatedEmpAddress { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        public List<DTO.Country> countries { get; set; }
        public List<State> StateList { get; set; }
        public List<ListValue> AddressTypeList { get; set; }
        public EmployeeAddress employeeAddress = new EmployeeAddress();
        public ILT.IHR.DTO.User user;
        public EmployeeAddress currentAddress { get; set; }

        protected string Title = "Add";
        public string country { get; set; }
        public bool ShowDialog { get; set; }
        public bool copyFromCurrent { get; set; } = false;
        public int addTypeID { get; set; }
        public int countryId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }
        public async Task Show(int ID, int AddressTypeId, EmployeeAddress empAddress, string AddressType)
        {
            copyFromCurrent = false;
            if(ID == 0)
            {
                if(string.IsNullOrEmpty(empAddress.Address1))
                {
                    ResetDialog();
                }
                Title = "Add";
                ShowDialog = true;
                addTypeID = AddressTypeId;
                employeeAddress.AddressTypeID = AddressTypeId;
                if(AddressType !="" && AddressType.ToUpper() != EmployeeAddresses.CURRADD)
                {
                    copyFromCurrent = true;
                }
                StateHasChanged();
            }
            else
            {
                Title = "Edit";
                employeeAddress = empAddress;
                if (employeeAddress.Country != null)
                {
                   await getStateList(employeeAddress.Country);
                }
                else
                {
                    StateList = new List<State>();
                }
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
                AddressTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ADDRESSTYPE).ToList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                countries = response.Data.ToList();
            }
        }
        private async Task getStateList(string country)
        {
            await GetStates(country);
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
            if(countries != null)
            {
                countryId = countries.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
                if (countryId != 0 && countryId != null)
                {
                    StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
                    StateHasChanged();
                }
            }
        }
        protected async Task SaveAddress()
        {
            employeeAddress.EmployeeName = EmployeeName;
            updatedEmpAddress.InvokeAsync(employeeAddress);
            ShowDialog = false;
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
            employeeAddress = new EmployeeAddress { };
        }
        protected async Task CopyFromCurrent()
        {
            employeeAddress = new EmployeeAddress();
            employeeAddress.Address1 = currentAddress.Address1;
            employeeAddress.Address2 = currentAddress.Address2;
            employeeAddress.EmployeeAddressID = 0;
            employeeAddress.AddressTypeID = addTypeID;
            employeeAddress.City = currentAddress.City;
            employeeAddress.Country = currentAddress.Country;
            employeeAddress.ZipCode = currentAddress.ZipCode;
            employeeAddress.StartDate = currentAddress.StartDate;
            employeeAddress.EndDate = null;
            if (employeeAddress.Country != null)
            {
               await getStateList(employeeAddress.Country);
               employeeAddress.State = currentAddress.State;
            }
            else
            {
                StateList = new List<State>();
            }
            StateHasChanged();
        }
    }
}
