using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.User
{
    public class UsersBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IUserService UserService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.User> UsersList { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

        public AddEditUserBase AddEditUserModal { get; set; }

        protected ILT.IHR.DTO.User selected;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission UserRolePermission;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }

      
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            UsersList = new List<ILT.IHR.DTO.User> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);            
            await LoadList();
        }

        private async Task LoadTableConfig()
        {
            UserRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.USER);
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

            RowActions = new List<IRowActions> { };
            if (UserRolePermission != null)
            {
                if (UserRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (UserRolePermission.Delete == true)
                {
                    RowActions.Add(m2);
                }
            }            
            
            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD"
            };
            
            if (UserRolePermission != null && UserRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }
                
            UsersList = new List<ILT.IHR.DTO.User> { };
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
                AddEditUserModal.Show(selected.UserID, "Edit");
            }
        }
        public void Add()
        {
            AddEditUserModal.Show(0, "");
        }

        protected async Task LoadList()
        {
            await LoadTableConfig();
            var reponses = (await UserService.GetUsers());
            if (reponses.MessageType == MessageType.Success)
            {
                UsersList = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }

        public void RowClick(ILT.IHR.DTO.User data)
        {
            selected = data;
            StateHasChanged();
        }      

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                // await UserService.DeleteUser(selected.UserID);              
            }
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }


    }
}
