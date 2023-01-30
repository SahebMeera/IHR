using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using BlazorTable;
using ILT.IHR.UI.Pages.DeleteConfirmation;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeSalary
{
    public class EmployeeSalaryBase: ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Inject]
        public IAssignmentService AssignmentService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public IDependentService DependentService { get; set; } //Service
        public List<Salary> Salaries { get; set; }  // Table APi Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected Salary selected;
        public AddEditEmployeeSalaryBase AddEditSalaryModal { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }
        protected ConfirmBase ChildDeleteConfirmation { get; set; }
        public List<RolePermission> RolePermissions;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }

        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            Salaries = new List<Salary> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeSalaries();
            }
        }


        private async Task LoadTableConfig()
        {
            RolePermission SalaryRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SALARY);

            IRowActions edit = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            IRowActions delete = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger",
                IsShow = SalaryRolePermission.Delete
            };
            RowActions = new List<IRowActions> { edit, delete };
          
            IHeaderActions add = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsShow = SalaryRolePermission.Add
            };
            HeaderAction = new List<IHeaderActions> { add };
            IRowActions childEdit = new IRowActions
            {
                IconClass = "oi oi-pencil",
                //ActionMethod = EditChild,
                ButtonClass = "btn-primary"
            };
            
            Salaries = new List<Salary> { };
        }
        protected async Task LoadEmployeeSalaries()
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(Id);
            if(resp.MessageType == MessageType.Success)
            Salaries = resp.Data.Salaries;
            if(selected != null)
            {
                RowClick(selected);
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
                AddEditSalaryModal.Show(selected.SalaryID);

            }
        }
        public void Add()
        {
            AddEditSalaryModal.Show(0);
        }
        public void EditMobile(ILT.IHR.DTO.Salary data)
        {
            selected = data;
            Edit();
        }

        public async void RowClick(Salary data)
        {
            selected = data;
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await AssignmentService.DeleteAssignment(selected.SalaryID);
                Response<ILT.IHR.DTO.Employee> respAssignment = await EmployeeService.GetEmployeeByIdAsync(Id);
                if (respAssignment.MessageType == MessageType.Success)
                    Salaries = respAssignment.Data.Salaries;
                else
                    Salaries = respAssignment.Data.Salaries;
                toastService.ShowSuccess("Salary Deleted successfully", "");
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
