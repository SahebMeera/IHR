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

namespace ILT.IHR.UI.Pages.Employee.EmployeeAssignment
{
    public class EmployeeAssignmentBase: ComponentBase
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
        public List<Assignment> EmployeeAssignments { get; set; }  // Table APi Data
        public List<AssignmentRate> EmployeeAssignmentRate { get; set; }  // Table APi Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<IRowActions> ChildRowActions { get; set; } //Row Actions
        public List<IHeaderActions> HeaderAction { get; set; } //Header Actions
        public List<IHeaderActions> ChildHeaderAction { get; set; } //Header Actions
        protected Assignment selected;
        protected AssignmentRate selectedChild;
        public AddEditEmployeeAssignmentBase AddEditAssignmentModal { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }
        protected ConfirmBase ChildDeleteConfirmation { get; set; }
        public List<RolePermission> RolePermissions;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }

        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeAssignments = new List<Assignment> { };
            EmployeeAssignmentRate = new List<AssignmentRate> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeAssignments();
            }
        }


        private async Task LoadTableConfig()
        {
            RolePermission AssigmentsRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.ASSIGNMENT);

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
                IsShow = AssigmentsRolePermission.Delete
            };
            RowActions = new List<IRowActions> { edit, delete };
          
            IHeaderActions add = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsShow = AssigmentsRolePermission.Add
            };
            HeaderAction = new List<IHeaderActions> { add };
            IRowActions childEdit = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = EditChild,
                ButtonClass = "btn-primary"
            };
            IRowActions childDelete = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = DeleteChild,
                ButtonClass = "btn-danger",
                IsShow = AssigmentsRolePermission.Delete
            };
            ChildRowActions = new List<IRowActions> { childEdit, childDelete };
            IHeaderActions childAdd = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = AddChild,
                ActionText = "ADD",
                IsShow = AssigmentsRolePermission.Add
            };
            ChildHeaderAction = new List<IHeaderActions> { childAdd };
            
            EmployeeAssignments = new List<Assignment> { };
            EmployeeAssignmentRate = new List<AssignmentRate> { };
        }
        protected async Task LoadEmployeeAssignments()
        {
            Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
            resp = await EmployeeService.GetEmployeeByIdAsync(Id);
            if(resp.MessageType == MessageType.Success)
            EmployeeAssignments = resp.Data.Assignments;
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
                AddEditAssignmentModal.Show(selected.AssignmentID);

            }
        }
        public void Add()
        {
            AddEditAssignmentModal.Show(0);
        }
        public void DeleteChild()
        {
            if (selectedChild != null)
            {
                ChildDeleteConfirmation.Show();
            }
        }

        public void EditChild()
        {
            if (selectedChild != null)
            {
                AddEditAssignmentModal.ShowChild(selectedChild.AssignmentID,selectedChild.AssignmentRateID);
            }
        }
        public void AddChild()
        {
            AddEditAssignmentModal.ShowChild(selected.AssignmentID,0);
        }
        public void EditMobile(ILT.IHR.DTO.Assignment data)
        {
            selected = data;
            Edit();
        }
        public async void RowClick(Assignment data)
        {
            selected = data;
            GetAssignmentRates();
        }
        private async void GetAssignmentRates()
        {
            Response<Assignment> resp = new Response<Assignment>();
            resp = (await AssignmentService.GetAssignmentById(selected.AssignmentID));
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeAssignmentRate = resp.Data.AssignmentRates.ToList();
            }
            StateHasChanged();
        }
        public async void ChildRowClick(AssignmentRate data)
        {
            selectedChild = data;
            StateHasChanged();
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await AssignmentService.DeleteAssignment(selected.AssignmentID);
                Response<ILT.IHR.DTO.Employee> respAssignment = await EmployeeService.GetEmployeeByIdAsync(Id);
                if (respAssignment.MessageType == MessageType.Success)
                    EmployeeAssignments = respAssignment.Data.Assignments;
                else
                    EmployeeAssignments = respAssignment.Data.Assignments;
                toastService.ShowSuccess("Assignment Deleted successfully", "");
            }
        }
        protected async Task ChildConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await AssignmentService.DeleteAssignmentRate(selectedChild.AssignmentRateID);
                Response<ILT.IHR.DTO.Employee> respAssignment = await EmployeeService.GetEmployeeByIdAsync(Id);
                if (respAssignment.MessageType == MessageType.Success)
                {
                    EmployeeAssignments = respAssignment.Data.Assignments;
                    GetAssignmentRates();
                }
                else
                    EmployeeAssignments = respAssignment.Data.Assignments;
                toastService.ShowSuccess("Assignment Rate Deleted successfully", "");
            }
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
               var date = dateTime.Value.ToString();
                formattedDate = date.Split(" ")[0];
            }
            
            return formattedDate;
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
