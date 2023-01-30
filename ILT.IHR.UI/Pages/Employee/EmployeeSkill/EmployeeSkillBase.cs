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

namespace ILT.IHR.UI.Pages.Employee.EmployeeSkill
{
    public class EmployeeSkillBase: ComponentBase
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
        protected DTO.EmployeeSkill selected;
        [Inject]
        public IEmployeeSkillService EmployeeSkillService { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        public List<DTO.EmployeeSkill> EmployeeSkillList { get; set; }  // Table APi Data
        public CommonUtils commonUtils { get; set; }
        public AddEditEmployeeSkillBase AddEditEmployeeSkillBaseModal { get; set; }
        public RolePermission NPIPermission { get; set; }
        public bool isViewPermissionForNPIRole { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            commonUtils = new CommonUtils();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SKILL);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            EmployeeSkillList = new List<DTO.EmployeeSkill> { };
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            isViewPermissionForNPIRole = !NPIPermission.View;
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadEmployeeSkill();
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
            EmployeeSkillList = new List<DTO.EmployeeSkill> { };
        }

        protected async Task LoadEmployeeSkill()
        {
            var resp = await EmployeeSkillService.GetEmployeeSkill(Id);
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeSkillList = resp.Data.ToList();
            } else
            {
                EmployeeSkillList = new List<DTO.EmployeeSkill> { };
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
              AddEditEmployeeSkillBaseModal.Show(selected.EmployeeSkillID, Id);
            }
        }
        public void Add()
         {
            AddEditEmployeeSkillBaseModal.Show(0, Id);
        }
        public void EditMobile(ILT.IHR.DTO.EmployeeSkill data)
        {
            selected = data;
            Edit();
        }
        public void RowClick(DTO.EmployeeSkill data)
        {
            selected = data;
            StateHasChanged();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                 await EmployeeSkillService.DeleteEmployeeSkill(selected.EmployeeSkillID);
                 await LoadEmployeeSkill();
                 toastService.ShowSuccess("EmployeeSkill Delete successfully", "");

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
