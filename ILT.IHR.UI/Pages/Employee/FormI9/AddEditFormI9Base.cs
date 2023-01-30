using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using System.Linq;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using ILT.IHR.UI.Shared;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.FormI9
{
    public class AddEditFormI9Base : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service   
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public II9DocumentService I9DocumentService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public IFormI9Service FormI9Service { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        public List<ListType> ListTypeValues { get; set; }  // Table APi Data  
        public List<ListValue> WorkAuthorizationList { get; set; }  // Table APi Data  
        public List<ListValue> ListValues { get; set; }  // Table APi Data  
        public List<I9Document> I9DocumentsList { get; set; }  // Table APi Data  
        public List<I9Document> ListADocumentList { get; set; }  // Table APi Data  
        public List<I9Document> ListBDocumentList { get; set; }  // Table APi Data  
        public List<I9Document> ListCDocumentList { get; set; }  // Table APi Data  
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        public string ConfirmMessage;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase SubmitConfirmation { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int FormI9ID { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public DTO.FormI9 FormI9 = new DTO.FormI9();
        public ILT.IHR.DTO.User user;
        protected bool isShow { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission NPIPermission { get; set; }
        public bool empSSN { get; set; } = false;
        protected InputMask mask { get; set; }
        protected ILT.IHR.DTO.Employee Employee { get; set; }
        public int currentAddressTypeID { get; set; }
        public List<ListValue> AddressTypeList { get; set; }
        public EmployeeAddress currentAddress { get; set; }
        public bool ssnRequired { get; set; } = false;
        public bool isSaveButtonDisabled { get; set; } = false;
        public bool I9ANumber { get; set; } = false;
        public bool I9ANumberDisabled { get; set; } = false;
        public bool I9USCISNumber { get; set; } = false;
        public bool I9USCISNumberDisabled { get; set; } = false;
        public bool I9PassportCountry { get; set; } = false;
        public bool I9PassportNumber { get; set; } = false;
        public bool I9I94Number { get; set; } = false;
        public bool I9ListAIssAuth { get; set; } = false;
        public bool I9ListADocNum { get; set; } = false;
        public bool I9ListAExpDate { get; set; } = false;
        public bool I9ListBIssAuth { get; set; } = false;
        public bool I9ListBDocNum { get; set; } = false;
        public bool I9ListBExpDate { get; set; } = false;
        public bool I9ListCIssAuth { get; set; } = false;
        public bool I9ListCDocNum { get; set; } = false;
        public bool I9ListCExpDate { get; set; } = false;
        public bool I9I94ExpiryDate { get; set; } = false;
        public bool I94FieldsShow { get; set; } = false;
        public string ErrorMessage;
        public string SSNNumber { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public bool isViewPermissionForNPIRole { get; set; } = false;
        public List<DTO.FormI9> FormI9List { get; set; }
        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.I9INFO);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            isViewPermissionForNPIRole = !NPIPermission.View;
            FormI9List = new List<DTO.FormI9> { };
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
            FormI9ID = Id;
            FormI9.EmployeeID = employeeId;
            ResetDialog();
            if (FormI9ID != 0)
            {
                CountryName = "";
                StateName = "";
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(FormI9ID, employeeId);
            }
            else
            {                
                isShow = EmployeeInfoRolePermission.Add;
                Title = "Add";
                CountryName = "";
                StateName = "";
                //FormI9.StartDate = DateTime.Now;
                FormI9.EmployeeID = employeeId;
                LoadDropDown();
                loadEmployeeData(employeeId);
                isfirstElementFocus = true;
                ShowDialog = true;
                //StateHasChanged();
            }
        }
        public bool isSaveButtonHidden { get; set; } = false;
        protected async Task LoadFormI9s(int employeeId)
        {
            FormI9List = new List<DTO.FormI9> { };
            var resp = await FormI9Service.GetFormI9(employeeId);
            if (resp.MessageType == MessageType.Success)
            {
                FormI9List = resp.Data.ToList();
                int index = FormI9List.FindIndex(x => x.FormI9ID == FormI9ID);
                if (index == FormI9List.Count - 1)
                {
                    isSaveButtonHidden = false;
                }
                else
                {
                    isSaveButtonHidden = true;
                }
            }
            else
            {
                FormI9List = new List<DTO.FormI9> { };
            }

        }

        protected async Task loadEmployeeData(int employeeId)
        {
            if (employeeId != 0)
            {
                currentAddress = new EmployeeAddress();
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await EmployeeService.GetEmployeeByIdAsync(employeeId) as Response<ILT.IHR.DTO.Employee>;
                if (resp.MessageType == MessageType.Success)
                {
                    Employee = resp.Data;
                    if (Employee.EmployeeID != 0)
                    {
                        FormI9.FirstName = Employee.FirstName;
                        FormI9.LastName = Employee.LastName;
                        FormI9.MiddleName = Employee.MiddleName;
                        FormI9.BirthDate = Employee.BirthDate;
                        FormI9.SSN = Employee.SSN;                        
                        SSNNumber = NPIPermission.View == true ? Employee.SSN : "***-**-****";
                        FormI9.WorkAuthorizationID = Employee.WorkAuthorizationID;
                        FormI9.WorkAuthorization = Employee.WorkAuthorization;
                        FormI9.HireDate = Employee.HireDate;
                        FormI9.Phone = Employee.Phone;
                        FormI9.Email = Employee.Email;
                        if (AddressTypeList != null)
                        {
                            currentAddressTypeID = AddressTypeList.Find(x => x.Value == EmployeeAddresses.CURRADD).ListValueID;
                            var currentAdd = Employee.EmployeeAddresses.Find(x => x.AddressTypeID == currentAddressTypeID);
                            if (currentAdd != null)
                            {
                                currentAddress = currentAdd;
                                FormI9.Address1 = currentAddress.Address1;
                                FormI9.Address2 = currentAddress.Address2;
                                FormI9.City = currentAddress.City;
                                FormI9.Country = currentAddress.Country;
                                FormI9.ZipCode = currentAddress.ZipCode;
                                if (FormI9.Country != null)
                                {
                                    CountryName = currentAddress.Country;
                                    await GetStates(FormI9.Country);
                                    FormI9.State = currentAddress.State;
                                    StateName = currentAddress.State;
                                }
                                else
                                {
                                    StateList = new List<State>();
                                }
                            }
                            LoadI9Documents(FormI9.WorkAuthorizationID);
                            ErrorMessage = "";
                            if (string.IsNullOrEmpty(FormI9.SSN))
                            {
                                ErrorMessage = "SSN Number is required. Please update SSN from Employee tab";
                                ssnRequired = true;
                                isSaveButtonDisabled = true;
                            } else
                            {
                                ssnRequired = false;
                            }
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
                AddressTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ADDRESSTYPE).ToList();
                WorkAuthorizationList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WORKAUTHORIZATION).ToList();
                ListValues = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.I9DOCUMENTTYPE).ToList();

            }
            //List<ListValue> lstValues = new List<ListValue>();
            //Response<IEnumerable<ListValue>> respTypes = (await LookupService.GetLookups());
            //if (respTypes.MessageType == MessageType.Success)
            //{
            //    ListValues = lstValues.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.I9LISTADOCUMENTS || x.Type.ToUpper() == ListTypeConstants.I9LISTBDOCUMENTS || x.Type.ToUpper() == ListTypeConstants.I9LISTCDOCUMENTS).ToList();
            //}            
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
           

            List<I9Document> lstI9Documents = new List<I9Document>();
            Response<IEnumerable<I9Document>> respI9Doc = (await I9DocumentService.GetI9Documents());
            if (respI9Doc.MessageType == MessageType.Success)
            {
                I9DocumentsList = respI9Doc.Data.ToList();
            }
        }

        protected async Task onANumberChange(ChangeEventArgs e)
        {
            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.GREENCARD || x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == FormI9.WorkAuthorizationID).ToList().Count > 0 && (string.IsNullOrEmpty(e.Value.ToString())))
            {
                I9ANumber = true;
            }
            else
            {               
                I9ANumber = false;
            }
        }

        protected async Task onUSCISNumberChange(ChangeEventArgs e)
        {
            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == FormI9.WorkAuthorizationID).ToList().Count > 0 && (string.IsNullOrEmpty(e.Value.ToString())))
            {
                I9USCISNumber = true;
            }
            else
            {
                I9USCISNumber = false;
            }
        }

        protected async Task onI94NumberChange(ChangeEventArgs e)
        {
            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == FormI9.WorkAuthorizationID).ToList().Count > 0 && (string.IsNullOrEmpty(e.Value.ToString())))
            {
                I9I94Number = true;
            }
            else
            {
                I9I94Number = false;
            }
        }

        protected async Task onListAIssuAuthChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListAIssAuth = true;
            }
            else
            {
                I9ListAIssAuth = false;
            }
        }

        protected async Task onListADocNumChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListADocNum = true;
            }
            else
            {
                I9ListADocNum = false;
            }
        }

        protected async Task onListAExpDateChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListAExpDate = true;
            }
            else
            {
                I9ListAExpDate = false;
            }
        }

        protected async Task onListBIssuAuthChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListBIssAuth = true;
            }
            else
            {
                I9ListBIssAuth = false;
            }
        }

        protected async Task onListBDocNumChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListBDocNum = true;
            }
            else
            {
                I9ListBDocNum = false;
            }
        }

        protected async Task onListBExpDateChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListBExpDate = true;
            }
            else
            {
                I9ListBExpDate = false;
            }
        }

        protected async Task onListCIssuAuthChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListCIssAuth = true;
            }
            else
            {
                I9ListCIssAuth = false;
            }
        }

        protected async Task onListCDocNumChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListCDocNum = true;
            }
            else
            {
                I9ListCDocNum = false;
            }
        }

        protected async Task onListCExpDateChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9ListCExpDate = true;
            }
            else
            {
                I9ListCExpDate = false;
            }
        }

        protected async Task onI94ExpiryDateChange(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                I9I94ExpiryDate = true;
            }
            else
            {
                I9I94ExpiryDate = false;
            }
        }

        private async Task LoadI9Documents(int intWorkAuthId)
        {
            ErrorMessage = "";
            if (WorkAuthorizationList.Where(x => x.Value == ListTypeConstants.USCITIZEN && x.ListValueID == intWorkAuthId).ToList().Count > 0)
            {
                I9USCISNumberDisabled = true;
                I9ANumberDisabled = true;
            }
            else
            {
                I9USCISNumberDisabled = false;
                I9ANumberDisabled = false;
            }

            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == intWorkAuthId).ToList().Count > 0)
            {
                I94FieldsShow = true;
            }
            else
            {
                I94FieldsShow = false;
            }

            //if (WorkAuthorizationList.Where(x => x.Value == ListTypeConstants.GREENCARD && x.ListValueID == intWorkAuthId).ToList().Count > 0 && (string.IsNullOrEmpty(FormI9.ANumber)))
            //{
            //    I9ANumber = true;
            //}
            //else
            //{
            //    I9ANumber = false;
            //}

            //if (WorkAuthorizationList.Where(x => x.Value == ListTypeConstants.GREENCARD && x.ListValueID == intWorkAuthId).ToList().Count > 0 && (string.IsNullOrEmpty(FormI9.USCISNumber)))
            //{               
            //    I9USCISNumber = true;
            //}
            //else
            //{
            //    I9USCISNumber = false;
            //}

            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == intWorkAuthId).ToList().Count > 0 && (string.IsNullOrEmpty(FormI9.I94Number)))
            {
                I9I94Number = true;
            }
            else
            {
                I9I94Number = false;
            }

            if (WorkAuthorizationList.Where(x => (x.Value == ListTypeConstants.H1B || x.Value == ListTypeConstants.E3 || x.Value == ListTypeConstants.TN || x.Value == ListTypeConstants.H4EAD) && x.ListValueID == intWorkAuthId).ToList().Count > 0 && (FormI9.I94ExpiryDate == null))
            {
                I9I94ExpiryDate = true;
            }
            else
            {
                I9I94ExpiryDate = false;
            }

            ListADocumentList = I9DocumentsList.Where(x => x.I9DocTypeID == ListValues.Find(y => y.Value.ToUpper() == ListTypeConstants.I9LISTADOCUMENTS).ListValueID && x.WorkAuthID == intWorkAuthId).ToList();
            ListBDocumentList = I9DocumentsList.Where(x => x.I9DocTypeID == ListValues.Find(y => y.Value.ToUpper() == ListTypeConstants.I9LISTBDOCUMENTS).ListValueID && x.WorkAuthID == intWorkAuthId).ToList();
            ListCDocumentList = I9DocumentsList.Where(x => x.I9DocTypeID == ListValues.Find(y => y.Value.ToUpper() == ListTypeConstants.I9LISTCDOCUMENTS).ListValueID && x.WorkAuthID == intWorkAuthId).ToList();

            if (FormI9.ListADocumentTitleID != null && FormI9.ListADocumentTitleID != 0)
            {
                if (string.IsNullOrEmpty(FormI9.ListAIssuingAuthority))
                {
                    I9ListAIssAuth = true;
                }
                else
                {
                    I9ListAIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListADocumentNumber))
                {
                    I9ListADocNum = true;
                }
                else
                {
                    I9ListADocNum = false;
                }
                if (FormI9.ListAExpirationDate == null)
                {
                    I9ListAExpDate = true;
                }
                else
                {
                    I9ListAExpDate = false;
                }
            }
            if (FormI9.ListBDocumentTitleID != null && FormI9.ListBDocumentTitleID != 0)
            {
                if (string.IsNullOrEmpty(FormI9.ListBIssuingAuthority))
                {
                    I9ListBIssAuth = true;
                }
                else
                {
                    I9ListBIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListBDocumentNumber))
                {
                    I9ListBDocNum = true;
                }
                else
                {
                    I9ListBDocNum = false;
                }
                if (FormI9.ListBExpirationDate == null)
                {
                    I9ListBExpDate = true;
                }
                else
                {
                    I9ListBExpDate = false;
                }
            }
            if (FormI9.ListCDocumentTitleID != null && FormI9.ListCDocumentTitleID != 0)
            {
                if (string.IsNullOrEmpty(FormI9.ListCIssuingAuthority))
                {
                    I9ListCIssAuth = true;
                }
                else
                {
                    I9ListCIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListCDocumentNumber))
                {
                    I9ListCDocNum = true;
                }
                else
                {
                    I9ListCDocNum = false;
                }
                if (FormI9.ListCExpirationDate == null)
                {
                    I9ListCExpDate = true;
                }
                else
                {
                    I9ListCExpDate = false;
                }
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

        private async Task GetStates(string country)
        {
            if(CountryList != null)
            countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                StateName = FormI9.State;
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }

        private async Task GetDetails(int Id, int employeeId)
        {
            Response<DTO.FormI9> resp = new Response<DTO.FormI9>();
            resp = await FormI9Service.GetFormI9ByIdAsync(Id);
            if (resp.MessageType == MessageType.Success)
            {
                CountryName = "";
                FormI9 = resp.Data;
                
                SSNNumber = NPIPermission.View == true ? FormI9.SSN : "***-**-****";
                if (FormI9.Country != null)
                {
                    CountryName = FormI9.Country;
                    await GetStates(FormI9.Country);
                }
                else
                {
                    StateList = new List<State>();
                }
                Title = "Edit";
                LoadI9Documents(FormI9.WorkAuthorizationID);
                isfirstElementFocus = true;
                await LoadFormI9s(employeeId);
                ShowDialog = true;
                StateHasChanged();
            }
            StateHasChanged();
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
            FormI9 = new DTO.FormI9 { };
        }

        protected async Task ConfirmSubmit_Click(bool submitConfirmed)
        {
            if (submitConfirmed)
            {
                await SaveFormI9();
            }
        }

        protected async Task OnSubmitI9()
        {
            if (FormI9ID != 0)
            {
                ConfirmMessage = "User should create a new record for any change other than typo. Are you fixing a typo?";
                SubmitConfirmation.Show();
            }
            else
            {
                await SaveFormI9();
            }
        }
        protected async Task SaveFormI9()
        {
            if (I9ANumber == true || I9USCISNumber == true || I9I94Number == true || I9ListAIssAuth == true || I9ListADocNum == true || I9ListAExpDate == true || I9ListBIssAuth == true || I9ListBDocNum == true || I9ListBExpDate == true || I9ListCIssAuth == true || I9ListCDocNum == true || I9ListCExpDate == true || I9I94ExpiryDate == true)
                return;

            if (WorkAuthorizationList.Where(x => x.Value == ListTypeConstants.GREENCARD && x.ListValueID == FormI9.WorkAuthorizationID).ToList().Count > 0 && string.IsNullOrEmpty(FormI9.ANumber) && string.IsNullOrEmpty(FormI9.USCISNumber))
            {
                ErrorMessage = "A Number or USCIS Number required";
                return;
            }
            if (FormI9.ListADocumentTitleID == null || FormI9.ListADocumentTitleID == 0)
            {
                if ((FormI9.ListBDocumentTitleID == null || FormI9.ListBDocumentTitleID == 0) || (FormI9.ListCDocumentTitleID == null || FormI9.ListCDocumentTitleID == 0))
                {
                    ErrorMessage = "List A or List (B and C) Documents required";
                    return;
                }
            }


            FormI9.EmployeeName = EmployeeName;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (FormI9.FormI9ID != 0)
            {
                Response<ILT.IHR.DTO.FormI9> resp = new Response<ILT.IHR.DTO.FormI9>();
                if (!string.IsNullOrEmpty(FormI9.SSN) && mask != null)
                    FormI9.SSN = mask.rawValue.StartsWith("*") ? FormI9.SSN : mask.rawValue; 
                FormI9.ModifiedBy = user.FirstName + " " + user.LastName;

                resp = await FormI9Service.UpdateFormI9(FormI9.FormI9ID, FormI9);
                if (resp.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("FormI9 saved successfully", "");
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
                Response<ILT.IHR.DTO.FormI9> resp = new Response<ILT.IHR.DTO.FormI9>();
                //if (!string.IsNullOrEmpty(Employee.SSN) && mask != null)
                //    FormI9.SSN = mask.rawValue;
                FormI9.CreatedBy = user.FirstName + " " + user.LastName;
                resp = await FormI9Service.SaveFormI9(FormI9);
                if (resp.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("FormI9 saved successfully", "");
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

        protected async Task onChangeListADocumentTitle(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && ListADocumentList != null)
            {
                var listADocumentTitle = Convert.ToInt32(e.Value);
                FormI9.ListADocumentTitle = ListADocumentList.Find(x => x.I9DocumentID == listADocumentTitle).I9DocName;

                if (string.IsNullOrEmpty(FormI9.ListAIssuingAuthority))
                {
                    I9ListAIssAuth = true;
                }
                else
                {
                    I9ListAIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListADocumentNumber))
                {
                    I9ListADocNum = true;
                }
                else
                {
                    I9ListADocNum = false;
                }
                if (FormI9.ListAExpirationDate == null)
                {
                    I9ListAExpDate = true;
                }
                else
                {
                    I9ListAExpDate = false;
                }
            }
            else
            {
                FormI9.ListADocumentTitle = "";
                FormI9.ListAIssuingAuthority = "";
                FormI9.ListADocumentNumber = "";
                FormI9.ListAExpirationDate = null;
                I9ListAIssAuth = false;
                I9ListADocNum = false;
                I9ListAExpDate = false;
            }
        }
        protected async Task onChangeListBDocumentTitle(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && ListBDocumentList != null)
            {
                var listBDocumentTitle = Convert.ToInt32(e.Value);
                FormI9.ListBDocumentTitle = ListBDocumentList.Find(x => x.I9DocumentID == listBDocumentTitle).I9DocName;

                if (string.IsNullOrEmpty(FormI9.ListBIssuingAuthority))
                {
                    I9ListBIssAuth = true;
                }
                else
                {
                    I9ListBIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListBDocumentNumber))
                {
                    I9ListBDocNum = true;
                }
                else
                {
                    I9ListBDocNum = false;
                }
                if (FormI9.ListBExpirationDate == null)
                {
                    I9ListBExpDate = true;
                }
                else
                {
                    I9ListBExpDate = false;
                }
            }
            else
            {
                FormI9.ListBDocumentTitle = "";
                FormI9.ListBIssuingAuthority = "";
                FormI9.ListBDocumentNumber = "";
                FormI9.ListBExpirationDate = null;
                I9ListBIssAuth = false;
                I9ListBDocNum = false;
                I9ListBExpDate = false;
            }
        }

        public bool isListCExpirationDate { get; set; } = false;
        protected async Task onChangeListCDocumentTitle(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && ListCDocumentList != null)
            {
                var listCDocumentTitle = Convert.ToInt32(e.Value);
                FormI9.ListCDocumentTitle = ListCDocumentList.Find(x => x.I9DocumentID == listCDocumentTitle).I9DocName;
                isListCExpirationDate = false;
                if (FormI9.ListCDocumentTitle.ToLower() == "Social Security Account Number Card".ToString().ToLower() || FormI9.ListCDocumentTitle.ToLower() == "Birth Certificate".ToString().ToLower())
                {
                    FormI9.ListCExpirationDate = new DateTime(9999, 12, 31);
                    isListCExpirationDate = true;
                } else
                {
                    FormI9.ListCExpirationDate = null;
                    isListCExpirationDate = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListCIssuingAuthority))
                {
                    I9ListCIssAuth = true;
                }
                else
                {
                    I9ListCIssAuth = false;
                }
                if (string.IsNullOrEmpty(FormI9.ListCDocumentNumber))
                {
                    I9ListCDocNum = true;
                }
                else
                {
                    I9ListCDocNum = false;
                }
                if (FormI9.ListCExpirationDate == null)
                {
                    I9ListCExpDate = true;
                }
                else
                {
                    I9ListCExpDate = false;
                }
            }
            else
            {
                FormI9.ListCDocumentTitle = "";
                FormI9.ListCIssuingAuthority = "";
                FormI9.ListCDocumentNumber = "";
                FormI9.ListCExpirationDate = null;
                I9ListCIssAuth = false;
                I9ListCDocNum = false;
                I9ListCExpDate = false;
            }
        }
    }
}
