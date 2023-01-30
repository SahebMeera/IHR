using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.Extensions.Configuration;
using Blazored.SessionStorage;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeDependent
{
    public class EmployeeDependentBase : ComponentBase

    {
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Name { get; set; }
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public IDependentService DependentService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } 
        public List<Dependent> EmployeeDependentList { get; set; }  // Table APi Data

        public IEnumerable<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        //[Inject]
        //public NavigationManager NavigationManager { get; set; }
         protected Dependent selected;
        public AddEditDependentBase AddEditDependentModal { get; set; }
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        public int DefaultPageSize { get; set; }
        protected async override Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeDependentList = new List<Dependent> { };
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeDependents();
            }
        }


        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
                //IsShow = EmployeeInfoRolePermission.Update
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
            EmployeeDependentList = new List<Dependent> { };
        }
        protected async Task LoadEmployeeDependents()
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(Id) as Response<ILT.IHR.DTO.Employee>;
            if(resp.MessageType == MessageType.Success)
            EmployeeDependentList = resp.Data.Dependents;
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
                AddEditDependentModal.Show(selected.DependentID, Id);
            }
        }
        public void Add()
        {

            AddEditDependentModal.Show(0, Id);
        }
        public void EditMobile(ILT.IHR.DTO.Dependent data)
        {
            selected = data;
            Edit();
        }

        public void RowClick(Dependent data)
        {
            selected = data;
            StateHasChanged();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                // await LookupService.DeleteListValue(selected.ListValueID);
                // await LoadList();
                // toastService.ShowSuccess("Lookup Delete successfully", "");

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
        protected string FormatDateCard(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
    }
}
