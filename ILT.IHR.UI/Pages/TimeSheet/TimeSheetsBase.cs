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
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.TimeSheet
{
    public class TimeSheetsBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service        
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ITimeSheetService TimeSheetService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; }

        [Inject]
        public DataProvider dataProvider { get; set; }
        public List<ILT.IHR.DTO.TimeSheet> TimeSheetsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.TimeSheet> lstTimeSheetRequest { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions      

        public AddEditTimeSheetBase AddEditTimeSheetModal { get; set; }

        protected ILT.IHR.DTO.TimeSheet selected;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission TimeSheetRolePermission;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        private int EmployeeID { get; set; }
        protected int statusId { get; set; }
        protected int DefaultStatusID { get; set; }
        public string status { get; set; }
        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        private DTO.User user { get; set; }

        public TabControl TimesheetTabControl { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            DefaultStatusID = 0;
            TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            if (user != null && user.EmployeeID != null)
            {
                EmployeeID = Convert.ToInt32(user.EmployeeID);
            }
            Response<IEnumerable<ListValue>> resp = await LookupService.GetListValues();
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETSTATUS).ToList();
                if(StatusList != null)
                {
                    setStatusList();
                }
            }
            
            LoadTableConfig(0);
            await LoadTimeSheetRequest();
            if (dataProvider.storage != null)
            {
                TimesheetTabControl.ActivePageByIndex(1);
            }
            if (dataProvider != null && dataProvider.TabIndex == 1)
            {
                TimesheetTabControl.ActivePageByIndex(1);
                dataProvider.TabIndex = 0;
            }
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

        private void LoadTableConfig(int StatusId)
        {
            string Status;
            if (StatusId != 0)
                Status = StatusList.Find(x => x.ListValueID == StatusId).Value;
            else
                Status = "All";
            TimeSheetRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.TIMESHEET);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger"
            };

            RowActions = new List<IRowActions> { };
            if (TimeSheetRolePermission != null)
            {
                if (TimeSheetRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (TimeSheetRolePermission.Delete == true)
                {
                    RowActions.Add(m2);
                }
            }

            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD"
            };

            if (TimeSheetRolePermission != null && TimeSheetRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }

            // TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
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
                AddEditTimeSheetModal.Show(selected.TimeSheetID);
            }
        }
        public void Add()
        {
            AddEditTimeSheetModal.Show(0);
        }

        protected async Task LoadTimeSheetRequest()
        {
            lstTimeSheetRequest = null;
            statusId = 0;
            LoadTableConfig(statusId);
            if (user.EmployeeID != null)
            {
                var respTimeSheetRequest = (await TimeSheetService.GetTimeSheets((int)user.EmployeeID, user.UserID));
                if (respTimeSheetRequest.MessageType == MessageType.Success)
                {
                    TimeSheetsList = respTimeSheetRequest.Data.ToList();
                    lstTimeSheetRequest = TimeSheetsList;
                }
                else
                {
                    TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
                    lstTimeSheetRequest = TimeSheetsList;
                }
            }
            else
            {
                TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
                lstTimeSheetRequest = TimeSheetsList;
            }
            StateHasChanged();
        }

        protected async Task loadList(int StatusID)
        {
            // await LoadTableConfig();
            if (StatusID != 0)
            {
                // status = StatusList.Find(x => x.ListValueID == StatusID).Value;
                lstTimeSheetRequest = TimeSheetsList.Where(x => x.StatusID == StatusID).ToList();
            }
            else
            {
                lstTimeSheetRequest = TimeSheetsList;
            }
            StateHasChanged();
        }

        protected async Task onStatusChange(ChangeEventArgs e)
        {
            int StatusId = Convert.ToInt32(e.Value);
            LoadTableConfig(StatusId);
            await loadList(StatusId);
        }

        protected async Task RefreshList()
        {
            await LoadTimeSheetRequest();
            await loadList(statusId);
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
        public void onSatusChange(int StatusID)
        {
            DefaultStatusID = StatusID;
            LoadTableConfig(StatusID);
            loadList(StatusID);
        }


        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await TimeSheetService.DeleteTimeSheet(selected.TimeSheetID);
                if (user.EmployeeID != null)
                {
                    var reponses = (await TimeSheetService.GetTimeSheets((int)user.EmployeeID, user.UserID));
                    if (reponses.MessageType == MessageType.Success)
                    {
                        TimeSheetsList = reponses.Data.ToList();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                toastService.ShowSuccess("TimeSheet Deleted successfully", "");
            }
        }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }

        protected void OnDrpDwnChange(ChangeEventArgs e)
        {
            DefaultStatusID = Convert.ToInt32(e.Value);
            onSatusChange(DefaultStatusID);
            // onDropDownChange.Invoke(DefaultID);
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
