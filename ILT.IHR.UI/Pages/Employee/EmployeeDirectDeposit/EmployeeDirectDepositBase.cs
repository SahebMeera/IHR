using Blazored.SessionStorage;
using Blazored.Toast.Services;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Pages.DeleteConfirmation;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Pages.Employee.EmployeeDirectDeposit
{
    public class EmployeeDirectDepositBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Name { get; set; }
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        public List<DirectDeposit> EmployeeDepositList { get; set; }  // Table APi Data
        [Inject]
        public IContactService ContactService { get; set; } //Service

        public IEnumerable<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

        protected DirectDeposit selected;
        public AddEditDirectDepositBase AddEditDirectDepositModal { get; set; }
        protected ConfirmBase DeleteConfirmation { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission NPIPermission { get; set; }
        public bool isViewPermissionForNPIRole { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeDepositList = new List<DirectDeposit> { };
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            isViewPermissionForNPIRole = !NPIPermission.View;
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeDeposits();
            }
        }
        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
              //  IsShow = EmployeeInfoRolePermission.Update
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
            EmployeeDepositList = new List<DirectDeposit> { };
        }
        protected async Task LoadEmployeeDeposits()
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(Id) as Response<ILT.IHR.DTO.Employee>;
            //Temp - resp check 
            EmployeeDepositList = resp.Data.DirectDeposits;
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
                AddEditDirectDepositModal.Show(selected.DirectDepositID, Id);
            }
        }
        public void Add()
        {
            AddEditDirectDepositModal.Show(0, Id);
        }
        public void EditMobile(ILT.IHR.DTO.DirectDeposit data)
        {
            selected = data;
            Edit();
        }
        public void RowClick(DirectDeposit data)
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
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
    }
}
