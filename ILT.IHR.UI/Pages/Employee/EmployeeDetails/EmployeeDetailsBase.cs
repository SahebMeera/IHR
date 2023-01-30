using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using ILT.IHR.DTO;
using ILT.IHR.UI.Shared;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.Employee.EmployeeDetails
{
    public class EmployeeDetailsBase: ComponentBase
    {
        [Inject]
        protected DataProvider dataProvider { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService toastService { get; set; }
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public ICountryService CountryService { get; set; } 
        [Inject]
        public IRoleService RoleService { get; set; }
        [Inject]
        public INotificationService NotificationService { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; }
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service   
        [Inject]
        public IFormI9Service FormI9Service { get; set; } //Service
        [Inject]
        public IAssignmentService AssignmentService { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration  
        protected ILT.IHR.DTO.Employee OldEmployee { get; set; }
        protected ILT.IHR.DTO.Employee Employee { get; set; }

        public List<ListValue> GenderList { get; set; }
        public List<ListValue> EmployMentList { get; set; }
        public List<ListValue> TitleList { get; set; }
        public List<ListValue> WorkAuthorizationList { get; set; }  // Table APi Data
        public List<ListValue> MaritalStautsList { get; set; }  // Table APi Data
        public List<ListValue> WithHoldingList { get; set; }  // Table APi Data
        public List<ListValue> AddressTypeList { get; set; }
        public List<Department> DepartmentList { get; set; } 
        public List<DTO.Employee> ManagerList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission EmployeeAssigmentRolePermission { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public RolePermission EmployeeRolePermission { get; set; }
        public RolePermission W4InfoPermission { get; set; }
        public RolePermission I9InfoPermission { get; set; }
        public RolePermission SalaryInfoPermission { get; set; }
        public RolePermission SKillInfoPermission { get; set; }
        public RolePermission NPIPermission { get; set; }
        public EmployeeAddress currentAddress { get; set; }
        public EmployeeAddress permanentAddress { get; set; }
        public EmployeeAddress mailingAddress { get; set; }
        public ILT.IHR.DTO.User user;
        protected AddEditAddressBase AddEditAddressModal { get; set; }
        [Parameter]
        public string Id { get; set; }
        public int countryId { get; set; }
        public int currentAddressTypeID { get; set; }
        public int permanentAddressTypeID { get; set; }
        public int mailingAddressTypeID { get; set; }
        protected bool showSpinner;
        public string country { get; set; }
        protected InputMask mask { get; set; }
        public bool canUpdate { get; set; }
        public bool isAddressInvalid { get; set; }
        public bool empPAN { get; set; } = false;
        public bool empAadharNumber { get; set; } = false;
        public bool empSSN { get; set; } = false;
        public bool isShowW4I9Info { get; set; } = false;
        public string ErrorMessage;

        public string EmpEmail { get; set; }
        public string EmpWorkEmail { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;
        public bool isViewPermissionForNPIRole { get; set; } = false;
        public bool isEditPermissionForNPIRole { get; set; } = false;
        public string AadharNumber { get; set; }
        public string PanCardNumber { get; set; }
        public string SSNNumber { get; set; }
        public string empSalary { get; set; }
        public string empVariablePay { get; set; }
        protected string EmployeeEmail { get; set; }

        public List<Module> Modules { get; set; } //Drop Down Api Data
        public List<DTO.FormI9> FormI9List { get; set; }

        public string ConfirmMessage = "";
        public string confirmType = "";
        public TabControl EmployeeDetailsTabControl { get; set; }
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase SubmitConfirmation { get; set; }
        protected bool isSalaryMacthed = true;

        protected ElementReference salaryLabel;
        public List<Assignment> EmployeeAssignments { get; set; }  // Table APi Data
        protected override async Task OnInitializedAsync()
        {
            currentAddress = new EmployeeAddress();
            permanentAddress = new EmployeeAddress();
            mailingAddress = new EmployeeAddress();
            user = new DTO.User();
            Employee = new ILT.IHR.DTO.Employee { };
            EmployeeAssignments = new List<Assignment> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":Employee"];
            EmployeeAssigmentRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.ASSIGNMENT);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            EmployeeRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEE);
            W4InfoPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.W4INFO);
            I9InfoPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.I9INFO);
            SalaryInfoPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SALARY);
            SKillInfoPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SKILL);
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            if (dataProvider.storage != null)
            {
                DTO.Employee Empdata = (DTO.Employee)dataProvider.storage;
                if (Empdata.EmployeeID != 0)
                {
                    Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                    resp = await EmployeeService.GetEmployeeByIdAsync(Empdata.EmployeeID) as Response<ILT.IHR.DTO.Employee>;
                    if(resp.MessageType == MessageType.Success)
                    {
                        isViewPermissionForNPIRole = NPIPermission.View;
                        isEditPermissionForNPIRole = NPIPermission.Update;
                        Employee = resp.Data;
                        if(resp.Data != null && resp.Data.Assignments != null)
                        {
                            EmployeeAssignments = resp.Data.Assignments;
                        }
                        //if (Employee.Salaries != null && Employee.Salaries.Count() > 0)
                        //{
                        //    isSalaryMacthed = Convert.ToDouble(Employee.Salaries.FirstOrDefault().CostToCompany) == Convert.ToDouble(Employee.Salary);
                        //}


                        AadharNumber = NPIPermission.View == true? Employee.AadharNumber : "************";
                        PanCardNumber = NPIPermission.View == true? Employee.PAN : "**********";
                        SSNNumber = NPIPermission.View == true? Employee.SSN : "***-**-****";
                        empSalary = NPIPermission.View == true ? Employee.Salary.ToString() : "*********.**";
                        empVariablePay = NPIPermission.View == true ? Employee.VariablePay.ToString() : "*********.**";
                        EmpEmail = Employee.Email;
                        EmpWorkEmail = Employee.WorkEmail;
                        if (Employee != null && Employee.Country != null && Employee.Country.ToUpper() == Countries.UNITEDSTATES)
                        {
                            isShowW4I9Info = false;
                        }
                        else
                        {
                            isShowW4I9Info = true;
                        }
                    }
                    canUpdate = EmployeeRolePermission.Update;
                }
                else if(Empdata.EmployeeID == 0)
                {
                    Employee.EmployeeAddresses = new List<EmployeeAddress>();
                    isShowW4I9Info = false;
                    Employee.Country = "United States";
                    canUpdate = EmployeeRolePermission.Add;
                }
                Modules = (await RoleService.GetModules()).ToList();
                await LoadDropDown();
                dataProvider.storage = null;

            }
            else
            {
                NavigationManager.NavigateTo("/employees");
            }
               
       
        }
        
        private async Task LoadDropDown()
        {
            Response<IEnumerable<DTO.Employee>> respManager = (await EmployeeService.GetEmployees());
            if (respManager.MessageType == MessageType.Success)
            {
                ManagerList = respManager.Data.ToList();
            }
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                GenderList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.GENDER).ToList();
                EmployMentList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.EMPLOYMENTTYPE).ToList();
                TitleList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TITLE).ToList();
                WorkAuthorizationList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WORKAUTHORIZATION).ToList();
                MaritalStautsList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.MARITALSTATUS).ToList();
                WithHoldingList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WITHHOLDINGSTATUS).ToList();
                AddressTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ADDRESSTYPE).ToList();
            }
            //if(Employee.EmployeeID == 0)
            //{
            //    Employee.EmploymentTypeID = EmployMentList.Find(x => x.Value == EmployeeTypeConstants.FTE).ListValueID;
            //    Employee.EmployeeCode = "ILTUS1001";
            //   // Employee.EmploymentType = EmployMentList.Find(x => x.ListValueID == Employee.EmploymentTypeID).ValueDesc;
            //}
            SetAddressTypeID();
            Response<IEnumerable<Department>> departmentResp = (await EmployeeService.GetDepartments());
            if (departmentResp.MessageType == MessageType.Success)
            {
                DepartmentList = departmentResp.Data.ToList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
            if (Employee.Country != null)
            {
                await GetStates(Employee.Country);
            }
            else
            {
                StateList = new List<State>();
            }

            if(Employee.EmployeeID != 0)
            {
                setEmployeeAddresses();
            }

            if (Employee.EmployeeID == 0)
            {
                Employee.EmployeeCode = "SYSGENERATED";
            }
        }
   
        protected async Task onChangeEmployeeCountry(ChangeEventArgs e)
        {
            country = Convert.ToString(e.Value);
            //await GetStates(country);
        }
        private async Task GetStates(string country)
        {
            countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }

        protected async Task checkEmailExist()
        {
            ErrorMessage = "";
            if (Employee.EmployeeID != 0)
            {
                if ((EmpEmail.Trim().ToUpper() != Employee.Email.Trim().ToUpper()) || (!String.IsNullOrEmpty(EmpWorkEmail) && !String.IsNullOrEmpty(Employee.WorkEmail) && EmpWorkEmail.ToUpper() != Employee.WorkEmail.ToUpper()))
                {
                    if ((ManagerList.ToList().FindAll(x => x.EmployeeID != Employee.EmployeeID && x.Email.ToUpper() == Employee.Email.Trim().ToUpper()).Count > 0 || ManagerList.ToList().FindAll(x => x.WorkEmail != null && !String.IsNullOrEmpty(Employee.WorkEmail) && x.EmployeeID != Employee.EmployeeID && x.WorkEmail.ToUpper() == Employee.WorkEmail.Trim().ToUpper()).Count > 0))
                    {
                        ErrorMessage = "Employee Email already exists in the system";
                    }
                    else
                    {
                        SaveEmployee();
                    }
                }
                else
                {
                    if ((ManagerList.ToList().FindAll(x => x.EmployeeID != Employee.EmployeeID && x.Email.ToUpper() == Employee.Email.Trim().ToUpper()).Count > 0 || ManagerList.ToList().FindAll(x => x.WorkEmail != null && !String.IsNullOrEmpty(Employee.WorkEmail) && x.EmployeeID != Employee.EmployeeID && x.WorkEmail.ToUpper() == Employee.WorkEmail.Trim().ToUpper()).Count > 0))
                    {
                        ErrorMessage = "Employee Email already exists in the system";
                    } else
                    {
                        SaveEmployee();
                    }
                }
            }
            else
            {
                if ((ManagerList.ToList().FindAll(x => x.Email.ToUpper() == Employee.Email.Trim().ToUpper()).Count > 0 || ManagerList.ToList().FindAll(x => x.WorkEmail != null && !String.IsNullOrEmpty(Employee.WorkEmail) && x.WorkEmail.ToUpper() == Employee.WorkEmail.Trim().ToUpper()).Count > 0))
                {
                    ErrorMessage = "Employee Email already exists in the system";
                }
                else
                {
                    SaveEmployee();
                }
            }

        }

        protected async Task SaveEmployee()
        {
   
            isAddressInvalid = false;
            showSpinner = true;
            if (!string.IsNullOrEmpty(currentAddress.Address1))
            {
                saveEmployeeDetails();
            }
            else
            {
                isAddressInvalid = true;
            }
            showSpinner = false;
        }

        //protected async Task SaveEmployee()
        //{
        //    ErrorMessage = "";
        //    if (Employee.EmployeeID == 0 && (ManagerList.ToList().FindAll(x => x.Email.ToUpper() == Employee.Email.Trim().ToUpper()).Count > 0 || ManagerList.ToList().FindAll(x => x.WorkEmail != null && !String.IsNullOrEmpty(Employee.WorkEmail) && x.WorkEmail.ToUpper() == Employee.WorkEmail.Trim().ToUpper()).Count > 0))
        //    {
        //        ErrorMessage = "Employee Email already exists in the system";
        //    }
        //    else
        //    {
        //        isAddressInvalid = false;
        //        showSpinner = true;
        //        if (!string.IsNullOrEmpty(currentAddress.Address1))
        //        {
        //            saveEmployeeDetails();
        //        }
        //        else
        //        {
        //            isAddressInvalid = true;
        //        }

        //        showSpinner = false;

        //    }

        //}
        protected async Task saveEmployeeDetails()
        {
            Employee.EmployeeName = Employee.FirstName + " " + Employee.LastName;
            //throw new Exception("Error from save emp");
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (!string.IsNullOrEmpty(PanCardNumber))
                Employee.PAN = PanCardNumber.ToUpper();
            if (!string.IsNullOrEmpty(AadharNumber))
                Employee.AadharNumber = AadharNumber;
            if (Employee.EmployeeID != 0)
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                if (!string.IsNullOrEmpty(SSNNumber) && mask != null)
                Employee.SSN =  mask.rawValue.StartsWith("*") ? SSNNumber : mask.rawValue;
                Employee.ModifiedBy = user.FirstName + " " + user.LastName;
                Employee.Salary = empSalary.StartsWith("*") ? Employee.Salary : Convert.ToInt32(empSalary);
                Employee.VariablePay = empVariablePay.StartsWith("*") ? Employee.VariablePay : Convert.ToInt32(empVariablePay);
                resp = await EmployeeService.UpdateEmployee(Employee.EmployeeID, Employee);
                if (resp.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Employee saved successfully", "");
                    isSaveButtonDisabled = true;
                    await LoadAfterSave(Employee.EmployeeID);
                    StateHasChanged();
                }
                else
                    toastService.ShowError(ErrorMsg.ERRORMSG);

                //var respI9 = await FormI9Service.GetFormI9(Employee.EmployeeID);
                //if (resp.MessageType == MessageType.Success)
                //{
                //    FormI9List = respI9.Data.ToList();
                //}
                //else
                //{
                //    FormI9List = new List<DTO.FormI9> { };
                //}
                //DTO.FormI9 formI9 = FormI9List.Last();

                //bool isAssignmentTermDateChanged = false;

                //if (Employee.Assignments != null && Employee.Assignments.Count() > 0 && Employee.TermDate != null)
                //{
                //    isAssignmentTermDateChanged = Employee.Assignments.Any(x => x.EndDate == null || x.EndDate > Employee.TermDate);
                //}

                //if (formI9 != null && formI9.WorkAuthorizationID != Employee.WorkAuthorizationID)
                //{
                //    this.ConfirmMessage = "Work Authorization has been updated";
                //    this.confirmType = "WORKAUTHORIZATION";
                //    this.SubmitConfirmation.Show();
                //}
                //else if (isAssignmentTermDateChanged)
                //{
                //    this.ConfirmMessage = "Term Date has been updated";
                //    this.confirmType = "TERMDATE";
                //    this.SubmitConfirmation.Show();
                //}
                //else
                //{ 
                
                //}
            }
            else
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                if (!string.IsNullOrEmpty(SSNNumber) && mask != null)
                    Employee.SSN = mask.rawValue;
                Employee.CreatedBy = user.FirstName + " " + user.LastName;
                resp = await EmployeeService.SaveEmployee(Employee);
                if (resp.MessageType == MessageType.Success)
                {
                    string emailBody = "First Name: " + Employee.FirstName + "|Last Name: " +
                        Employee.LastName + "|Title: " + Employee.Title + "|Department: " + Employee.Department
                        + "|Country: " + Employee.Country + "|Employee Type: " + Employee.EmploymentType;
                    string employeeName = Employee.FirstName + " " + Employee.LastName;
                    // await SaveEmailApproval("Employee", resp.Data.RecordID,employeeName, emailBody);
                    toastService.ShowSuccess("Employee saved successfully", "");
                    isSaveButtonDisabled = true;
                    await LoadAfterSave(resp.Data.RecordID);
                    StateHasChanged();
                }
                else
                    toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            isSaveButtonDisabled = false;
        }


        private async Task SaveEmailApproval(string ModuleName, int ID, string Name, string EmailBody)
        {
            DTO.EmailApproval emailapproval = new DTO.EmailApproval();
            emailapproval.EmailApprovalID = 0;
            emailapproval.ModuleID = Modules.Find(m => m.ModuleName.ToUpper() == ModuleName.ToUpper()).ModuleID;
            emailapproval.ID = ID;
            emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
            emailapproval.LinkID = Guid.Empty;
            // emailapproval.ApproverEmail = EmployeeAssignment.TSApproverEmail;
            emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
            // EmailFields emailFields = new EmailFields();
            emailapproval.EmailSubject = "New " + ModuleName + " : " + Name;// emailFields.EmailSubject;
            string EmailTo = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":ChangeNotification"];
            emailapproval.EmailTo = EmailTo.Replace(',', ';'); //"hmihir@infologitech.com";
            emailapproval.EmailFrom = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":FromEmail"];
            emailapproval.EmailBody = "New " + ModuleName + " " + Name + " has been added " +
                "<br/><ul>";
            string[] orderedList = EmailBody.Split('|').ToArray();

            foreach (string item in orderedList)
            {
                emailapproval.EmailBody += "<li>" + item + "</li>";
            }
            emailapproval.EmailBody += "</ul><br/>";
            emailapproval.IsActive = true;
            var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
        }

        public async Task<EmailFields> prepareNewEmployeeMail()
        {
            EmailFields emailFields = new EmailFields();
            string EmailTo = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":EmployeeTo"];
            emailFields.EmailTo = EmailTo.Replace(',', ';'); //"hmihir@infologitech.com";
            emailFields.EmailSubject = "New Employee";
            emailFields.EmailBody = "New employee " + Employee.FirstName + " "  + Employee.LastName + " has been Created " +
                "<br/>" +
                "<ul><li>First Name: " + Employee.FirstName + "<br/>" +
                "</li><li>Last Name: " + Employee.LastName +
                "</li><li>Title : " + Employee.Title +
                "</li><li>Department : " + Employee.Department +
                "</li><li>Country : " + Employee.Country +
                "</li><li>Employee Type : " + Employee.EmploymentType +
                // "</li><li>Created Date: " + FormatDate(Employee.CreatedDate) +
                "</li></ul><br/>";
            return emailFields;
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dd MMM yyy HH:mm:ss") + " GMT";
                formattedDate = date;
            }
            return formattedDate;
        }

        protected void CheckCurrentAddress()
        {
            if (string.IsNullOrEmpty(currentAddress.Address1))
            {
                isAddressInvalid = true;
            }
            else
            {
                isAddressInvalid = false;
            }
            //if (!string.IsNullOrEmpty(currentAddress.Country))
            //{
            //    if (Employee.Country.ToUpper() == "INDIA")
            //    {
            //        empAadharNumber = true;
            //        empPAN = true;
            //        empSSN = false;
            //    }
            //    else
            //    {
            //        empAadharNumber = false;
            //        empPAN = false;
            //        empSSN = true;
            //    }
            //}
        }

        public void onChangeEmpPAN()
        {
            //string PAN = Employee.PAN;
            //if (PAN != null)
            //{
            //    if (PAN.Length != 0 && PAN.Length != 10)
            //    {
            //        empPAN = true;
            //    }
            //    else
            //    {
            //        empPAN = false;
            //    }
            //}
            //else
            //{
            //    empPAN = false;
            //}

        }
        public void onChangeEmpAadharNumber()
        {
            //string aadharNumber = Employee.AadharNumber;
            //if (aadharNumber != null)
            //{
            //    if (aadharNumber.Length != 0 && aadharNumber.Length != 12)
            //    {
            //        empAadharNumber = true;
            //    }
            //    else
            //    {
            //        empAadharNumber = false;
            //    }
            //}
            //else
            //{
            //    empAadharNumber = false;
            //}

        }
        protected void Cancel()
        {
            NavigationManager.NavigateTo("/employees");
        }

        protected async Task addAddress(string AddressType)
        {
            if(currentAddress != null)
            {
                AddEditAddressModal.currentAddress = currentAddress;
            }
             var item= AddressTypeList.Find(x => x.Value == AddressType.ToUpper());
            await AddEditAddressModal.Show(0, item.ListValueID,new EmployeeAddress(), item.Value);
        }
        protected async Task EditAddress(EmployeeAddress employeeAddress)
        {
            await AddEditAddressModal.Show(Employee.EmployeeID,0,employeeAddress,"");
        }

        protected void setEmployeeAddresses()
        {
            var currentAdd = Employee.EmployeeAddresses.Find(x => x.AddressTypeID == currentAddressTypeID);
            if(currentAdd != null)
            {
                currentAddress = currentAdd;
            }
            var permAdd = Employee.EmployeeAddresses.Find(x => x.AddressTypeID == permanentAddressTypeID);
            if (permAdd != null)
            {
                permanentAddress = permAdd;
            }
            var mailingAdd = Employee.EmployeeAddresses.Find(x => x.AddressTypeID == mailingAddressTypeID);
            if (mailingAdd != null)
            {
                mailingAddress = mailingAdd;
            }
        }

        protected void SetAddressTypeID()
        {
            currentAddressTypeID = AddressTypeList.Find(x => x.Value == EmployeeAddresses.CURRADD).ListValueID;
            permanentAddressTypeID = AddressTypeList.Find(x => x.Value == EmployeeAddresses.PERMADD).ListValueID;
            mailingAddressTypeID = AddressTypeList.Find(x => x.Value == EmployeeAddresses.MAILADD).ListValueID;
        }

        protected void updateEmpAddressList(EmployeeAddress employeeAddress)
        {
            if(employeeAddress.EmployeeAddressID != 0)
            {
                var index = Employee.EmployeeAddresses.FindIndex(x => x.EmployeeAddressID == employeeAddress.EmployeeAddressID);
                Employee.EmployeeAddresses[index] = employeeAddress;
            }
            else
            {
                employeeAddress.EndDate = null;
                if(employeeAddress.AddressTypeID == currentAddressTypeID)
                {
                    currentAddress = employeeAddress;
                    isAddressInvalid = false;
                }else if(employeeAddress.AddressTypeID == permanentAddressTypeID)
                {
                    permanentAddress = employeeAddress;
                }else if (employeeAddress.AddressTypeID == mailingAddressTypeID)
                {
                    mailingAddress = employeeAddress;
                }
                Employee.EmployeeAddresses.Add(employeeAddress);
            }
        }

        protected async Task LoadAfterSave(int EmpID)
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(EmpID) as Response<ILT.IHR.DTO.Employee>;
            if (resp.MessageType == MessageType.Success)
            {
                Employee = resp.Data;
                EmpEmail = Employee.Email;
                EmpWorkEmail = Employee.WorkEmail;
                canUpdate = EmployeeRolePermission.Update;
                if (!string.IsNullOrEmpty(Employee.Country) && Employee.Country.ToUpper() == Countries.UNITEDSTATES)
                {
                    isShowW4I9Info = false;
                } else
                {
                    isShowW4I9Info = true;
                }
               
                if (Employee.EmployeeID != 0)
                {
                    setEmployeeAddresses();
                }
                StateHasChanged();
            }
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
            }
        }

        // DropDown OnChange Methods Starts Here
        protected async Task onDepartmentChange(ChangeEventArgs e)
        {
            if(e.Value != "" && e.Value != null && DepartmentList != null)
            {
                var department = Convert.ToInt32(e.Value);
                Employee.Department = DepartmentList.Find(x => x.DepartmentID == department).DeptName;
            }
        }

        protected async Task onTitleChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && TitleList != null)
            {
                var title = Convert.ToInt32(e.Value);
                Employee.Title = TitleList.Find(x => x.ListValueID == title).ValueDesc;
            }
        }

        protected async Task onGenderChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && GenderList != null)
            {
                var gender = Convert.ToInt32(e.Value);
                Employee.Gender = GenderList.Find(x => x.ListValueID == gender).ValueDesc;
            }
        }
        protected async Task onMaritalStatusChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && MaritalStautsList != null)
            {
                var maritalStatus = Convert.ToInt32(e.Value);
                Employee.MaritalStatus = MaritalStautsList.Find(x => x.ListValueID == maritalStatus).ValueDesc;
            }
        }

        
        protected async Task onWorkAuthorizationChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && WorkAuthorizationList != null)
            {
                var workAuthorization = Convert.ToInt32(e.Value);
                Employee.WorkAuthorization = WorkAuthorizationList.Find(x => x.ListValueID == workAuthorization).ValueDesc;
            }
        }

        protected async Task onEmployeeTypeChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && EmployMentList != null)
            {
                var employmentType = Convert.ToInt32(e.Value);
                 Employee.EmploymentType = EmployMentList.Find(x => x.ListValueID == employmentType).ValueDesc;
            }
        }

        protected async Task onEmpManagerChange(ChangeEventArgs e)
        {
            if (e.Value != "" && e.Value != null && ManagerList != null)
            {
                ErrorMessage = "";
                var manager = Convert.ToInt32(e.Value);
                Employee.Manager = ManagerList.Find(x => x.EmployeeID == manager).EmployeeName;
                if (ManagerList.ToList().Find(x => x.EmployeeID == manager).TermDate != null)
                {
                    isSaveButtonDisabled = true;
                    ErrorMessage = "Please select active manager.";
                } else
                {
                   isSaveButtonDisabled = false;
                }
            }
        }

        protected async Task ConfirmSubmit_Click(bool submitConfirmed)
        {
            if (submitConfirmed)
            {
                if (confirmType == "TERMDATE")
                {
                    this.EmployeeDetailsTabControl.ActivePageByText("Assignments");
                    this.confirmType = "";
                }
                if (confirmType == "WORKAUTHORIZATION")
                {
                    this.EmployeeDetailsTabControl.ActivePageByText("I9 Info");
                    this.confirmType = "";
                }
            }

            this.confirmType = "";
        }
        public void onchangeTermDate()
        {
            ErrorMessage = "";
            if (EmployeeAssignments != null && EmployeeAssignments.Count > 0) 
            {
                var EmployeeAssignment = EmployeeAssignments.Find(x => x.EndDate == null);
                if(EmployeeAssignment != null)
                {
                    var Dt = Employee.TermDate;
                    ErrorMessage = "Employee has active Assignments. Please terminate Assignments before terminating Employee.";
                    Employee.TermDate = null;
                    //  isSaveButtonDisabled = true;
                } else
                {
                    ErrorMessage = "";
                }
            }
            StateHasChanged();
        }
     }
}
