using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Microsoft.Extensions.Configuration;
using Blazored.SessionStorage;
using ILT.IHR.UI.Shared;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeW4
{
    public class EmployeeW4BBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string SSN { get; set; }
        [Parameter]
        public string Name { get; set; }
        public IEnumerable<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected DTO.EmployeeW4 selected;
        [Inject]
        public IEmployeeW4Service EmployeeW4Service { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        public List<DTO.EmployeeW4> EmployeeW4List { get; set; }  // Table APi Data
        public CommonUtils commonUtils { get; set; }
        public AddEditEmployeeW4Base AddEditEmployeeW4BaseModal { get; set; }
        public RolePermission NPIPermission { get; set; }
        public bool isViewPermissionForNPIRole { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            commonUtils = new CommonUtils();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.W4INFO);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeW4List = new List<DTO.EmployeeW4> { };
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            isViewPermissionForNPIRole = !NPIPermission.View;
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeW4s();
            }
        }
        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
                // IsShow = EmployeeInfoRolePermission.Update
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger",
                IsShow = EmployeeInfoRolePermission.Delete
            };
            RowActions = new List<IRowActions> { m1, m2 };
            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsShow = EmployeeInfoRolePermission.Add
            };
            HeaderAction = new List<IHeaderActions> { m3 };
            EmployeeW4List = new List<DTO.EmployeeW4> { };
        }

        protected async Task LoadEmployeeW4s()
        {
            var resp = await EmployeeW4Service.GetEmployeesW4(Id);
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeW4List = resp.Data.ToList();
            } else
            {
                EmployeeW4List = new List<DTO.EmployeeW4> { };
            }
                
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
                AddEditEmployeeW4BaseModal.Show(selected.EmployeeW4ID, Id, SSN);
            }
        }
        public void Add()
         {

            AddEditEmployeeW4BaseModal.Show(0, Id, SSN);
        }
        public void EditMobile(ILT.IHR.DTO.EmployeeW4 data)
        {
            selected = data;
            Edit();
        }
        public void RowClick(DTO.EmployeeW4 data)
        {
            selected = data;
            StateHasChanged();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                //  await ContactService.DeleteContact(selected.ContactID);
                // await LoadEmployeeContacts();
                //  toastService.ShowSuccess("Contact Delete successfully", "");

            }
        }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null && dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
