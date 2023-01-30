using ILT.IHR.UI.Service;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.RolePermissions
{
    public class RolePermissionsBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        public int RoleId { get; set; }  //DropdownID
        public List<RolePermission> RolePermissionsList { get; set; }  // Table APi Data
        public RolePermission PermissionsRolePermission { get; set; }
        public List<Role> Roles { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstRoles { get; set; } //Grid Drop Down Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        public AddEditRolePermissionBase AddEditRolePermissionModal { get; set; }

        protected ILT.IHR.DTO.RolePermission selected;

        public List<RolePermission> RolePermissions;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }

        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            RoleId = 0;
            RolePermissionsList = new List<RolePermission> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            await LoadTableConfig();
            await LoadDropDown();
        }

        private async Task LoadTableConfig()
        {
            PermissionsRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.PERMISSION);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger"
            };
            /*RowActions = new List<IRowActions> { m1, m2 };*/
            RowActions = new List<IRowActions> { };
            if (PermissionsRolePermission != null)
            {
                if (PermissionsRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (PermissionsRolePermission.Delete == true)
                {
                    RowActions.Add(m2);
                }
            }
            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsDisabled = RoleId != 0 ? false : true
            };
            /*HeaderAction = new List<IHeaderActions> { m3 };*/
            if (PermissionsRolePermission != null && PermissionsRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }
            RolePermissionsList = new List<RolePermission> { };
        }

        public void Delete()
        {
            if (selected != null)
            {
                DeleteConfirmation.Show();
            }
        }

        public void Edit()
        {
            if (selected != null)
            {
                AddEditRolePermissionModal.Show(selected.RolePermissionID, RoleId, "");
            }
        }
        public void Add()
        {
            var rolePermission = Roles.Find(x => x.RoleID == RoleId).RoleName;            
            AddEditRolePermissionModal.Show(0, RoleId, rolePermission);
        }

        private async Task LoadDropDown()
        {
            Response<List<Role>> respRoles = await RoleService.GetRoles();
            if (respRoles.MessageType == MessageType.Success)
            {
                Roles = respRoles.Data;
                if (Roles != null)
                {
                    SetRolesList();
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
                
           
    }
        protected async Task LoadRolePermissionList(int ID)
        {
            RoleId = ID;
            LoadTableConfig();
            if (RoleId != 0)
            {
                Response<Role> respRole = (await RoleService.GetRoleByIdAsync(RoleId));
                if(respRole.MessageType == MessageType.Success)
                    RolePermissionsList = respRole.Data.RolePermissions;
                else
                    RolePermissionsList = new List<RolePermission> { };
            }
            else
            {
                RolePermissionsList = new List<RolePermission> { };
            }
            StateHasChanged();
        }

        protected async Task LoadList()
        {
            LoadTableConfig();
            if (RoleId != 0)
            {
                Response<Role> respRole = (await RoleService.GetRoleByIdAsync(RoleId));
                if (respRole.MessageType == MessageType.Success)
                    RolePermissionsList = respRole.Data.RolePermissions;
                else
                    RolePermissionsList = new List<RolePermission> { };
            }
        }
        public void RowClick(ILT.IHR.DTO.RolePermission data)
        {
            selected = data;
            StateHasChanged();
        }

        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }



        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {                
                await RoleService.DeleteRolePermission(selected.RolePermissionID);
                RoleId = selected.RoleID;
                Response<Role> respRole = (await RoleService.GetRoleByIdAsync(RoleId));
                if (respRole.MessageType == MessageType.Success)
                    RolePermissionsList = respRole.Data.RolePermissions;
                else
                    RolePermissionsList = new List<RolePermission> { };
                toastService.ShowSuccess("Role Permission Deleted successfully", "");               
            }
        }
        protected void SetRolesList()
        {
            lstRoles = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstRoles = (from lookupItem in Roles
                        select new IDropDownList { ID = Convert.ToInt32(lookupItem.RoleID), Value = lookupItem.RoleName }).ToList();
                ListItem.ID = 0;
                ListItem.Value = "Select";
            lstRoles.Insert(0, ListItem);
        }
        protected void onChangeRole(int ID)
        {
            LoadRolePermissionList(ID);
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
