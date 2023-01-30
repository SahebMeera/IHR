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

namespace ILT.IHR.UI.Pages.RolePermissions
{
    public class AddEditRolePermissionBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        public List<Role> Roles { get; set; } //Drop Down Api Data
        public List<Module> Modules { get; set; } //Drop Down Api Data
        private int RolePermissionId { get; set; }
        [Parameter]
        public EventCallback<bool> RolePermissionUpdated { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service  
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        [Parameter]
        public string empID { get; set; }

        protected string Title = "Add";
        public string ErrorMessage;
        public RolePermission rolepermission = new RolePermission();
        public bool ShowDialog { get; set; }
        public ILT.IHR.DTO.User user;
        public bool isSaveButtonDisabled { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }

        //protected override async Task OnParametersSetAsync()
        //{
        //    if (!string.IsNullOrEmpty(empID))
        //    {
        //        Title = "Edit";
        //        emp = await Http.GetJsonAsync<Employee>("/api/Employee/" + empID);
        //    }
        //}
        public RolePermission PermissionsRolePermission { get; set; }
        protected async Task SaveRolePermission()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (RolePermissionId == 0)
            {
                PermissionsRolePermission = new RolePermission();
                rolepermission.CreatedBy = user.FirstName + " " + user.LastName;
                PermissionsRolePermission = RolePermissionsList.Find(x => x.RoleID == rolepermission.RoleID && x.ModuleID == rolepermission.ModuleID);
                ErrorMessage = "";
                if (PermissionsRolePermission != null)
                {
                    ErrorMessage = "Role Permission already exists in the system";
                } else
                {
                    var result = await RoleService.SaveRolePermission(rolepermission);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Role Permission saved successfully", "");
                        RolePermissionUpdated.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
            }
            else if (RolePermissionId > 0)
            {
                rolepermission.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await RoleService.UpdateRolePermission(RolePermissionId, rolepermission);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Role Permission saved successfully", "");
                    RolePermissionUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            isSaveButtonDisabled = false;
        }

        private async Task LoadDropDown()
        {
            Response<List<Role>> respRoles = await RoleService.GetRoles();
            if (respRoles.MessageType == MessageType.Success)
                Roles = respRoles.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);
            Modules = (await RoleService.GetModules()).ToList();

        }
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.RolePermission> resp = new Response<ILT.IHR.DTO.RolePermission>();
            resp = await RoleService.GetRolePermissionByIdAsync(Id) as Response<ILT.IHR.DTO.RolePermission>;
            if(resp.MessageType == MessageType.Success)
            {
                rolepermission = resp.Data;
            }
           
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }
        public void Cancel()
        {
            RolePermissionId = -1;
            ShowDialog = false;
           // RolePermissionUpdated.InvokeAsync(true);
            StateHasChanged();

        }
        public void Show(int Id, int roleId, string roleName)
        {
            ErrorMessage = "";
            RolePermissionId = Id;
            ResetDialog();
            if (RolePermissionId != 0)
            {
                GetDetails(RolePermissionId);
            }
            else
            {
                rolepermission.RoleID = roleId;
                LoadRolePermissionList(roleId);
                rolepermission.RoleName = roleName;
                isfirstElementFocus = true;
                ShowDialog = true;
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
            rolepermission = new RolePermission { };
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
        protected async Task onChangeModule(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && Modules != null)
            {
                var timeSheetType = Convert.ToInt32(e.Value);
                rolepermission.ModuleName = Modules.Find(x => x.ModuleID == timeSheetType).ModuleName;
            }
        }
        public List<RolePermission> RolePermissionsList;
        protected async Task LoadRolePermissionList(int ID)
        {
            if (ID != 0)
            {
                Response<Role> respRole = (await RoleService.GetRoleByIdAsync(ID));
                if (respRole.MessageType == MessageType.Success)
                    RolePermissionsList = respRole.Data.RolePermissions;
                else
                    RolePermissionsList = new List<RolePermission> { };
            }
            else
            {
                RolePermissionsList = new List<RolePermission> { };
            }
        }
    }
}
