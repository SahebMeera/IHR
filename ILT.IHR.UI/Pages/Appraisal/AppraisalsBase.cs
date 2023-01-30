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

namespace ILT.IHR.UI.Pages.Appraisal
{
    public class AppraisalsBase: ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public DataProvider dataProvider { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public int Id { get; set; }
        [Inject]
        public IAssignmentService AssignmentService { get; set; } //Service
        [Inject]
        public IAppraisalService AppraisalService { get; set; } //Service
        [Inject]
        public IDependentService DependentService { get; set; } //Service
        public List<DTO.Appraisal> Appraisals { get; set; }  // Table APi Data
        public List<DTO.Appraisal> lstAppraisalsList { get; set; }  // Table APi Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<IRowActions> ChildRowActions { get; set; } //Row Actions
        public List<IHeaderActions> HeaderAction { get; set; } //Header Actions
        public List<IHeaderActions> ChildHeaderAction { get; set; } //Header Actions
        protected DTO.Appraisal selected;
        public AppraisalDetail.AppraisalDetailsBase AppraisalDetailsModel { get; set; }
        // public AddEditAppraisalBase AddEditAssignmentModal { get; set; }
        protected ConfirmBase DeleteConfirmation { get; set; }
        protected ConfirmBase ChildDeleteConfirmation { get; set; }
        public List<RolePermission> RolePermissions;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        public ILT.IHR.DTO.User user;
        public List<AppraisalYear> AppraisalsYearList { get; set; }
        protected List<IDropDownList> lstYear { get; set; }
        protected int yearId { get; set; }

        protected async override Task OnInitializedAsync()
        {
            user = new DTO.User();
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            Appraisals = new List<DTO.Appraisal> { };
            loadYearDropdown();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            await LoadTableConfig();
            yearId = Convert.ToInt32(AppraisalsYearList.Find(x => x.text.ToLower() == "Current Year".ToLower()).year);
            if (user != null && user.EmployeeID > 0)
            {
                await LoadAppraisals();
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
            Appraisals = new List<DTO.Appraisal> { };
        }
        protected async Task LoadAppraisals()
        {
            if (user != null && user.EmployeeID != null)
            {
                int EmployeeID = Convert.ToInt32(user.EmployeeID);
                var resp = await AppraisalService.GetAppraisalList(EmployeeID);
                if (resp != null && resp.MessageType == MessageType.Success)
                    Appraisals = resp.Data.ToList();
                lstAppraisalsList = Appraisals;
                if (yearId != 0)
                {
                    onYearChange(yearId);
                }
            }
        }
        public void loadYearDropdown()
        {
            AppraisalYear m1 = new AppraisalYear
            {
                year = DateTime.Now.Year.ToString(),
                text = "Current Year"
            };
            AppraisalYear m2 = new AppraisalYear
            {
                year = (DateTime.Now.Year - 1).ToString(),
                text = "Previous Year"
            };
            AppraisalsYearList = new List<AppraisalYear> { m1, m2 };
            SetYearList();
            lstAppraisalsList = Appraisals.Where(x => x.ReviewYear.ToString() == DateTime.Now.Year.ToString()).ToList();
        }
        public class AppraisalYear
        {
            public string year { get; set; }
            public string text { get; set; }
        }

        protected void onYearChange(int Year)
        {
            yearId = Year;
            if (Year != 0)
                lstAppraisalsList = Appraisals.Where(x => x.ReviewYear == yearId).ToList();
            else
                lstAppraisalsList = Appraisals;
            StateHasChanged();
        }
           protected void SetYearList()
        {
            lstYear = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstYear = (from lookupItem in AppraisalsYearList
                       select new IDropDownList { ID = Convert.ToInt32(lookupItem.year), Value = lookupItem.text }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "Select";
            lstYear.Insert(0, ListItem);
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
                if (selected != null)
                {
                    dataProvider.storage = selected;
                    NavigationManager.NavigateTo($"/appraisalDetails");
                }
                // AppraisalDetailsModel.Show(selected.AppraisalID);
            }
        }
      
        public async void RowClick(DTO.Appraisal data)
        {
            selected = data;
           // GetAssignmentRates();
        }
       

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                //await AssignmentService.DeleteAssignment(selected.AssignmentID);
                //Response<ILT.IHR.DTO.Employee> respAssignment = await AppraisalService.GetEmployeeByIdAsync(Id);
                //if (respAssignment.MessageType == MessageType.Success)
                //    Appraisals = respAssignment.Data.Assignments;
                //else
                //    Appraisals = respAssignment.Data.Assignments;
                //toastService.ShowSuccess("DTO.Appraisal Deleted successfully", "");
            }
        }


        protected string FormatDate(DateTime? dateTime)
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
    }
}
