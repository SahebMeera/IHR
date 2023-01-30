using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.ManageTimeSheet
{
    public class ManageTimeSheetBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service        
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ITimeSheetService TimeSheetService { get; set; } //Service
        public List<ILT.IHR.DTO.TimeSheet> TimeSheetsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.TimeSheet> lstTimeSheetRequest { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions         

        protected ILT.IHR.DTO.TimeSheet selected;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission ManageTSRolePermission;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }

        protected ILT.IHR.DTO.TimeSheet TimeSheet = new ILT.IHR.DTO.TimeSheet();
        private int EmployeeID { get; set; }
        protected int DefaultStatusId { get; set; }
        private DTO.User user { get; set; }

        [Inject]
        private ILookupService LookupService { get; set; }
        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateTimeSheetList { get; set; }

        protected AddEditManageTimeSheetBase AddEditManageTimeSheetModal { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            ManageTSRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.MANAGETIMESHEET);
            if (user.EmployeeID != null)
            {
                EmployeeID = Convert.ToInt32(user.EmployeeID);
            }            
            await LoadTableConfig();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETSTATUS).ToList();
                // StatusList = StatusList.FindAll(x => x.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper() || x.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper());
                DefaultStatusId = StatusList.Find(x => x.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID;
                if (StatusList != null)
                {
                    setStatusList();
                }
            }
            await LoadTimeSheetRequest();
            StateHasChanged();
        }

        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-eye",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            RowActions = new List<IRowActions> { m1 };
        }

        protected void setStatusList()
        {
            lstStatus = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstStatus = (from lookupItem in StatusList
                         select new IDropDownList { ID = lookupItem.ListValueID, Value = lookupItem.ValueDesc }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstStatus.Insert(0, ListItem);
        }

        protected async Task LoadTimeSheetRequest()
        {           
            var respTimeSheetRequest = (await TimeSheetService.GetTimeSheets(0, 0, 0));
            if (respTimeSheetRequest.MessageType == MessageType.Success)
            {
                TimeSheetsList = respTimeSheetRequest.Data.ToList(); // Where(x => x.TimesheetApproverID == user.UserID).ToList();
               // DefaultStatusId = StatusList.Find(x => x.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID;
                loadList(DefaultStatusId);                
                StateHasChanged();
            }
            else
            {
                TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
            }
        }

        protected void loadList(int StatusID)
        {
            lstTimeSheetRequest = TimeSheetsList.Where(x => x.StatusID == StatusID).ToList();
            StateHasChanged();
        }

        public void RowClick(ILT.IHR.DTO.TimeSheet data)
        {
            selected = data;
            StateHasChanged();
        }
        public void EditMobile(ILT.IHR.DTO.TimeSheet data)
        {
            selected = data;
            Edit();
        }

        protected void OnDrpDwnChange(ChangeEventArgs e)
        {
            DefaultStatusId = Convert.ToInt32(e.Value);
            onStatusChange(DefaultStatusId);
            // onDropDownChange.Invoke(DefaultID);
        }
        protected async Task onStatusChange(ChangeEventArgs e)
        {
            // string status;
            int StatusId = Convert.ToInt32(e.Value);
            if (StatusId != 0)
            {
                // status = StatusList.Find(x => x.ListValueID == StatusId).Value;
                loadList(StatusId);
            }
            else
            {
                lstTimeSheetRequest = TimeSheetsList;
            }
        }

        private void Edit()
        {
            AddEditManageTimeSheetModal.Show(selected.TimeSheetID);
        }

        protected async Task RefreshList()
        {
            var respTimeSheetRequest = (await TimeSheetService.GetTimeSheets(0, user.UserID));
            if (respTimeSheetRequest.MessageType == MessageType.Success)
            {
                var user = await sessionStorage.GetItemAsync<DTO.User>("User");
                TimeSheetsList = respTimeSheetRequest.Data.ToList();
                if (DefaultStatusId != 0)
                {
                    // string status = StatusList.Find(x => x.ListValueID == DefaultStatusId).Value;
                    lstTimeSheetRequest = TimeSheetsList.Where(x => x.StatusID == DefaultStatusId).ToList();
                    StateHasChanged();
                }
                else
                {
                    lstTimeSheetRequest = TimeSheetsList;
                }
            }
            await UpdateTimeSheetList.InvokeAsync(true);
            StateHasChanged();
        }

        public void onStatusChange(int StatusID)
        {
            string status;
            DefaultStatusId = StatusID;
            if (StatusID != 0)
            {
                status = StatusList.Find(x => x.ListValueID == StatusID).Value;
                loadList(StatusID);
            }
            else
            {
                lstTimeSheetRequest = TimeSheetsList;
            }
            StateHasChanged();
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
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
    }
}
