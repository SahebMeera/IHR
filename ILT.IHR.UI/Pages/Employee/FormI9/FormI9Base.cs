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
using ILT.IHR.UI.Pages.Employee.FormI9.FormI9ChangesSet;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.FormI9
{
    public class FormI9Base : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Name { get; set; }
        public IEnumerable<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected DTO.FormI9 selected;
        [Inject]
        public IFormI9Service FormI9Service { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        public List<DTO.FormI9> FormI9List { get; set; }  // Table APi Data
        public CommonUtils commonUtils { get; set; }
        public AddEditFormI9Base AddEditFormI9BaseModal { get; set; }
        public FormI9ChangeSetBase FormI9ChangeSetModal { get; set; }

        protected async override Task OnInitializedAsync()
        {
            commonUtils = new CommonUtils();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.I9INFO);
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            FormI9List = new List<DTO.FormI9> { };
            await LoadTableConfig();
            if (Id > 0)
            {
                await LoadFormI9s();
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
                IconClass = "oi oi-loop",
                ActionMethod = changeLog,
                ButtonClass = "btn-primary"
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
            FormI9List = new List<DTO.FormI9> { };
        }

        protected async Task LoadFormI9s()
        {
            var resp = await FormI9Service.GetFormI9(Id);
            if (resp.MessageType == MessageType.Success)
            {
                FormI9List = resp.Data.ToList();
            }
            else
            {
                FormI9List = new List<DTO.FormI9> { };
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
                AddEditFormI9BaseModal.Show(selected.FormI9ID, Id);
            }
        }
        public void EditMobile(ILT.IHR.DTO.FormI9 data)
        {
            selected = data;
            Edit();
        }
        public void Add()
        {

            AddEditFormI9BaseModal.Show(0, Id);
        }
        public void changeLog()
        {
            FormI9ChangeSetModal.show(selected.FormI9ID);
        }
        public void RowClick(DTO.FormI9 data)
        {
            selected = data;
            StateHasChanged();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                //await FormI9Service.DeleteFormI9(selected.FormI9ID);
                //await LoadFormI9();
                //toastService.ShowSuccess("FormI9 Delete successfully", "");

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
