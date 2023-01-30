using Blazored.SessionStorage;
using Blazored.Toast.Services;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.Expenses
{
    public class ExpensesBase: ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IExpenseService ExpenseService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; }
        public int ExpenseId { get; set; }  //DropdownID
        public List<Expense> lstExpensesList { get; set; }  // Table APi Data
        public DTO.User user { get; set; }
        public List<Expense> ExpensesList { get; set; }  // Table APi Data
        public List<ListValue> StatusList { get; set; }  // Table APi Data
        public List<ListValue> ExpensesTypeList { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstStatus { get; set; } //Drop Down Api Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        public AddEditExpenseBase AddEditExpenseModal { get; set; }

        protected Expense selected;

        public List<RolePermission> RolePermissions;
        public RolePermission ExpenseRolePermission;
        protected int DefaultStatusID { get; set; }
        protected int submittedStatusID { get; set; }
        public CommonUtils commonUtils { get; set; }

        
        protected async override Task OnInitializedAsync()
        {
            commonUtils = new CommonUtils();
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            lstStatus = new List<IDropDownList>();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ExpenseStatus).ToList();
                ExpensesTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ExpenseType).ToList();
                submittedStatusID = StatusList.Find(x => x.Value.ToUpper() == "Submitted".ToUpper()).ListValueID;
                DefaultStatusID = submittedStatusID;
                setStatusList();
            }
            await LoadTableConfig();
            await LoadExpenses();
        }

        private async Task LoadTableConfig()
        {
            ExpenseRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EXPENSES);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",

            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger",
                IsShow = ExpenseRolePermission.Delete
            };
            RowActions = new List<IRowActions> { m1,m2 };

            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsShow = ExpenseRolePermission.Add
            };
            HeaderAction = new List<IHeaderActions> { m3 };
            ExpensesList = new List<Expense> { };
        }

        protected void setStatusList()
        {
            lstStatus.Clear();
            IDropDownList ListItem = new IDropDownList();
            lstStatus = (from status in StatusList
                              select new IDropDownList { ID = status.ListValueID, Value = status.ValueDesc }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstStatus.Insert(0, ListItem);
        }

        public void Delete()
        {
            if (selected != null)
            {
            }
        }

        public void Edit()
        {
            if (selected != null)
            {
                AddEditExpenseModal.Show(selected.ExpenseID);
            }
        }
        public void Add()
        {
            AddEditExpenseModal.Show(0);
        }

        protected async Task LoadExpenses()
        {
            string RoleShort = await sessionStorage.GetItemAsync<string>("RoleShort");
            var resp = (await ExpenseService.GetExpenses());
            if (resp.MessageType == MessageType.Success)
            {
                if (resp.Data != null && (RoleShort.ToUpper() == UserRole.EMP || RoleShort.ToUpper() == UserRole.CONTRACTOR))
                {
                    ExpensesList = resp.Data.Where(x => x.EmployeeID == user.EmployeeID).ToList();
                }
                else
                {
                    ExpensesList = resp.Data.ToList();
                }
                LoadExpensesByStatus(DefaultStatusID);
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }
        public void RowClick(Expense data)
        {
            selected = data;
            StateHasChanged();
        }

        public void OnStatusTypeChange(int statusID)
        {
            DefaultStatusID = statusID;
            LoadExpensesByStatus(statusID);
        }
        protected void LoadExpensesByStatus(int statusID)
        {
            if (statusID != 0)
            {
                lstExpensesList = ExpensesList.Where(x => x.StatusID == statusID).ToList();
            }
            else
            {
                lstExpensesList = ExpensesList;
            }
            StateHasChanged();
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
