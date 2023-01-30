using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using ILT.IHR.UI.Shared;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.User
{
    public class AddEditUserBase : ComponentBase
    {
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration  
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IUserService UserService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.User> Users { get; set; } //Drop Down Api Data
        public List<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data
        public List<Role> Roles { get; set; } //Drop Down Api Data
        public List<ILT.IHR.DTO.Company> Companies { get; set; } //Drop Down Api Data
        private int UserId { get; set; }
        public int EmployeeId { get; set; }
        [Parameter]
        public EventCallback<bool> UserUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        protected string Title = "Add";
        public ILT.IHR.DTO.User user = new ILT.IHR.DTO.User();
        public ILT.IHR.DTO.Employee employee = new ILT.IHR.DTO.Employee();
        public bool ShowDialog { get; set; }

        public string LoginMessage;
        public bool disabledvalue;
        public bool empnonempdisabledvalue;
        public bool isRoleUpdated { get; set; }
        [Parameter]
        public List<DTO.UserRole> Items { get; set; }
        public DTO.User loggedinuser { get; set; }
        public List<RolePermission> RolePermissions;
        public bool isSaveButtonDisabled { get; set; } = false;


        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            loggedinuser = await sessionStorage.GetItemAsync<DTO.User>("User");
            await LoadDropDown();
        }

        protected async Task SaveUser()
        {
            LoginMessage = "";
            var index = Items.FindIndex(x => x.IsDefault == true);
            var rolesSelected = Items.Where(x => x.IsSelected == true);
            if (user.FirstName == null || user.FirstName == "")
            {
                LoginMessage = "Please enter First Name";
            }
            else if (user.LastName == null || user.LastName == "")
            {
                LoginMessage = "Please enter Last Name";
            }
            else if (user.Email == null || user.Email == "")
            {
                LoginMessage = "Please enter Email Address";
            }
            else if (user.IsOAuth == false && (user.NewPassword == null || user.NewPassword == ""))
            {
                LoginMessage = "Please enter Password";
            }
            else if (user.IsOAuth == false && (user.ConfirmPassword == null || user.ConfirmPassword == ""))
            {
                LoginMessage = "Please enter Confirm Password";
            }
            else if (user.NewPassword != user.ConfirmPassword)
            {
                LoginMessage = "Password and Confirm Password should be same";
            }
            else if (rolesSelected.Count() == 0)
            {
                LoginMessage = "Please select Role";
            }
            else if(rolesSelected.Count() > 1 && index == -1)
            {
                LoginMessage = "Please select Default Role";
            }
            else if (user.CompanyID == 0)
            {
                LoginMessage = "Please select Company";
            }
            else if (user.UserID == 0 && Users.ToList().FindAll(x => x.Email.ToUpper() == user.Email.Trim().ToUpper()).Count > 0)
            {
                LoginMessage = "User Email already exists in the system";
            }
            else
            {
                user.Password = (user.Password != user.ConfirmPassword) ? user.ConfirmPassword : null;
                user.CreatedBy = loggedinuser.FirstName + " " + loggedinuser.LastName;
                user.UserRoles = Items.Where(x => x.IsSelected == true).ToList();
                user.UserRoles.ForEach(x =>
                {
                    x.ModifiedBy = loggedinuser.FirstName + " " + loggedinuser.LastName;
                    x.ModifiedDate = DateTime.Now;
                });
                if (isSaveButtonDisabled)
                    return;
                isSaveButtonDisabled = true;
                if (UserId == 0)
                {
                    if (user.EmployeeID == 0)
                    {
                        user.EmployeeID = null;
                    }
                    var result = await UserService.SaveUser(user);
                    if (result.MessageType == MessageType.Success)
                    {                        
                        toastService.ShowSuccess("User saved successfully", "");
                        UserUpdated.InvokeAsync(true);                        
                        Cancel();
                        await sendMail();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                else if (UserId > 0)
                {
                    user.ModifiedBy = loggedinuser.FirstName + " " + loggedinuser.LastName;
                    var result = await UserService.UpdateUser(UserId, user);
                    if (result.MessageType == MessageType.Success)
                    {                        
                        toastService.ShowSuccess("User saved successfully", "");
                        UserUpdated.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                        
                    }
                }
                isSaveButtonDisabled = false;
            }
        }

        public async Task sendMail()
        {
            string uri = Configuration["EmailApprovalUrl:" + loggedinuser.ClientID];
            string strPassword = "";
            if (user.IsOAuth == true)
            {
                strPassword = "Office 365 Password";
            }
            else
            {
                strPassword = "Contact your Manager/HR";
            }
            Common common = new Common();
            common.EmailTo = user.Email;
            common.EmailSubject = "IHR User Created";
            common.EmailBody = "IHR User Details:" + "<br/>" + "Client ID: " + loggedinuser.ClientID.ToUpper() + "<br/>" +
                "Email Address: " + user.Email + "<br/>" +                
                "Password: " + strPassword + "<br/>" +
                "<br/>Login to <a href='" + uri + "'> InfoHR</a><br/><div>NOTE: This is an outgoing message only. Please do not reply to this message</div>";
            Cancel();
            var result = await CommonService.SendEmail(common);
        }
        private async Task LoadDropDown()
        {
            var reponses = (await UserService.GetUsers());
            if (reponses.MessageType == MessageType.Success)
            {
                Users = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

            Response<List<ILT.IHR.DTO.Employee>> respEmployees = await UserService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
                Employees = respEmployees.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);

            Response<List<Role>> respRoles = await RoleService.GetRoles();
            if (respRoles.MessageType == MessageType.Success)
                Roles = respRoles.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);

            Response<IEnumerable<ILT.IHR.DTO.Company>> respCompanies = await CompanyService.GetCompanies();
            if (respCompanies.MessageType == MessageType.Success)
                Companies = respCompanies.Data.ToList();
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);
        }
        public bool isSaveButShowHide { get; set; }
        private async Task GetDetails(int Id, string profiletype)
        {
            isSaveButShowHide = false;
            Response<ILT.IHR.DTO.User> resp = new Response<ILT.IHR.DTO.User>();
            resp = await UserService.GetUserByIdAsync(Id) as Response<ILT.IHR.DTO.User>;
            if(resp.MessageType == MessageType.Success){
                user = resp.Data;
                user.NewPassword = user.Password;
                user.ConfirmPassword = user.Password;
                UpdateSelectedList();
                if (profiletype == "Edit")
                {
                    empnonempdisabledvalue = false;
                    if (user.EmployeeID != null)
                    {
                        disabledvalue = true;
                    }
                    else
                    {
                        disabledvalue = false;
                    }
                }
                else if (profiletype == "Myprofile")
                {
                    RolePermission UserRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.USER);
                    if (UserRolePermission != null)
                    {
                        if (UserRolePermission.Update == false)
                        {
                            isSaveButShowHide = true;
                        } else
                        {
                            isSaveButShowHide = false;
                        }
                    }
                    disabledvalue = true;
                    empnonempdisabledvalue = true;
                }
                Title = "Edit";
                ShowDialog = true;
                StateHasChanged();
            }
        }

        protected async Task GetEmployeeDetails(ChangeEventArgs e)
        {

            if (e.Value != "")
            {
                EmployeeId = Convert.ToInt32(e.Value);
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await UserService.GetEmployeeByIdAsync(EmployeeId) as Response<ILT.IHR.DTO.Employee>;
                if(resp.MessageType == MessageType.Success)
                {
                    employee = resp.Data;
                    user.EmployeeID = EmployeeId;
                    user.FirstName = employee.FirstName;
                    user.LastName = employee.LastName;
                    if (!string.IsNullOrEmpty(employee.WorkEmail))
                    {
                        user.Email = employee.WorkEmail;
                    }
                    else
                    {
                        user.Email = employee.Email;
                    }
                    user.CompanyID = Companies.Find(c => c.CompanyType.ToUpper() == "SELF").CompanyID;
                    disabledvalue = true;
                }
            }
            else
            {
                user.EmployeeID = 0;
                user.FirstName = "";
                user.LastName = "";
                user.Email = "";
                disabledvalue = false;
            }

        }

        public void Cancel()
        {
            UserId = -1;
            ShowDialog = false;
            // UserUpdated.InvokeAsync(true);
            StateHasChanged();

        }
        public void Show(int Id, string profiletype)
        {
            
            isRoleUpdated = false;
            isSaveButShowHide = false;
            LoginMessage = "";
            UserId = Id;
            ResetDialog();
            if (UserId != 0)
            {
                GetDetails(UserId, profiletype);
            }
            else
            {
                disabledvalue = false;
                Title = "Add";
                ShowDialog = true;
                SetNewUserRoleList();
                StateHasChanged();
            }
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        private void ResetDialog()
        {
            user = new DTO.User { };
        }
        protected void onAuthCheck(ChangeEventArgs e)
        {
            if(Convert.ToBoolean(e.Value) == true)
            {
                user.NewPassword = "";
                user.ConfirmPassword = "";
            }
        }
        
        protected void CheckboxChanged(ChangeEventArgs e, int key)
        {
            isRoleUpdated = true;
            if(Items != null)
            {
                var index = this.Items.FindIndex(x => x.RoleID == key);
                var userRole = this.Items.Find(x => x.RoleID == key);
                userRole.IsSelected = !userRole.IsSelected;
                if (userRole.IsSelected)
                {
                    userRole.CreatedBy = loggedinuser.FirstName + " " + loggedinuser.LastName;
                    userRole.CreatedDate = DateTime.Now;
                    userRole.UserID = UserId;
                    userRole.UserRoleID = 0;
                }
                else
                {
                    userRole.IsDefault = false;
                }
                this.Items[index] = userRole;
            }
        }
        protected void setDefaultRole(string action,DTO.UserRole role)
        {
            isRoleUpdated = true;
            if (role != null)
            {
                Items.ForEach(x =>
                {
                    if (action == "Add" && x.RoleID == role.RoleID)
                    {
                        x.IsDefault = true;
                    }
                    else
                    {
                        x.IsDefault = false;
                    }
                });
            }
        }
        protected void UpdateSelectedList()
        {
            Items = new List<DTO.UserRole>();
            foreach(var item in Roles)
            {
                var roleselected = user.UserRoles.Find(x => x.RoleID == item.RoleID);
                var tempRole = user.UserRoles.Find(x => x.RoleShort == item.RoleShort);
                DTO.UserRole userRole = new DTO.UserRole();
                userRole.RoleID = item.RoleID;
                userRole.RoleName = item.RoleName;
                userRole.RoleShort = item.RoleShort;
                if (tempRole != null)
                    userRole.UserRoleID = tempRole.UserRoleID;
                if (roleselected != null)
                {
                    userRole.CreatedBy = roleselected.CreatedBy;
                    userRole.CreatedDate = roleselected.CreatedDate;
                    userRole.IsSelected = true;
                    userRole.IsDefault = roleselected.IsDefault;
                }
                else
                {
                    userRole.IsSelected = false;
                }
                Items.Add(userRole);
            }   
        }

        protected void SetNewUserRoleList()
        {

            Items = new List<DTO.UserRole>();
            if(Roles != null)
            {
                foreach (var item in Roles)
                {
                    DTO.UserRole userRole = new DTO.UserRole();
                    userRole.RoleID = item.RoleID;
                    userRole.RoleName = item.RoleName;
                    userRole.RoleShort = item.RoleShort;
                    userRole.IsSelected = false;
                    Items.Add(userRole);
                }
            }
        }

        protected string getSelectedRoles()
        {
            string roles = "";
            if(Items != null)
            {
                if (Items.FindIndex(x=>x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in Items)
                    {
                        if (string.IsNullOrEmpty(roles) && item.IsSelected)
                        {
                            roles = item.RoleName;
                        }
                        else if(item.IsSelected)
                        {
                            roles = roles + ", " + item.RoleName;
                        }
                    }
                }
            }
            return roles;
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
        }

        protected async Task OnChangeCompany(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && Companies != null)
            {
                var company = Convert.ToInt32(e.Value);
                user.CompanyName = Companies.Find(x => x.CompanyID == company).Name;
            }
        }
    }
}
