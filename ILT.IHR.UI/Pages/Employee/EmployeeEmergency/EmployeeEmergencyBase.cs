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
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeEmergency
{
    public class EmployeeEmergencyBase: ComponentBase
    
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Name { get; set; }
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        public List<Contact> EmployeeContactList { get; set; }  // Table APi Data
        [Inject]
        public IContactService ContactService { get; set; } //Service

        public IEnumerable<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
                                                                     
        protected Contact selected;
         public AddEditEmergencyContactBase AddEditEmergencyContactModal { get; set; }
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        protected async override Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeContactList = new List<Contact> { };
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeContacts();
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
            EmployeeContactList = new List<Contact> { };
        }
        protected async Task LoadEmployeeContacts()
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(Id) as Response<ILT.IHR.DTO.Employee>;
            if(resp.MessageType == MessageType.Success)
            EmployeeContactList = resp.Data.Contacts;
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
                AddEditEmergencyContactModal.Show(selected.ContactID, Id);
            }
        }
        public void Add()
        {

            AddEditEmergencyContactModal.Show(0, Id);
        }
        public void EditMobile(ILT.IHR.DTO.Contact data)
        {
            selected = data;
            Edit();
        }
        public void RowClick(Contact data)
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
