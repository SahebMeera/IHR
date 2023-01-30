using Blazored.SessionStorage;
using Blazored.Toast.Services;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.EmployeeLeaveRequest
{
    public class EmployeeLeaveBase: ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; }
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public DataProvider dataProvider { get; set; }
        public List<LeaveBalance> LeaveBalanceList { get; set; }  // Table APi Data
        public List<Leave> LeaveRequestList { get; set; }  // Table APi Data
        public List<Leave> lstLeaveRequest { get; set; }  // Table APi Data
        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        [Inject]
        protected ILeaveBalanceService LeaveBalanceService { get; set; }
        [Inject]
        protected ILeaveService LeaveService { get; set; }
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected Leave selected { get; set; }
        private int EmployeeID { get; set; }
        protected int statusId { get; set; }
        protected int DefaultStatusID { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission LeaveRolePermission;
        public string status { get; set; }
        public AddEditLeaveBase AddEditLeaveModal { get; set; }

        public TabPage EmployeeLeaveApprovalTab { get; set; }
    
        public TabControl LeaveTabControl { get; set; }


        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            DefaultStatusID = 0;
            LeaveBalanceList = new List<LeaveBalance> { };
            LeaveRequestList = new List<Leave> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            var user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
                setStatusList();
                LoadTableConfig(0);
            }
            if (dataProvider.storage != null)
            {
                
                LeaveTabControl.ActivePageByIndex(1);
            }
            if (dataProvider != null && dataProvider.TabIndex == 1)
            {
                LeaveTabControl.ActivePageByIndex(1);
                dataProvider.TabIndex = 0;
            }

            await LoadLeaveBalance();
            await LoadLeaveRequest();
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
            LeaveRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.LEAVEREQUEST);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
                IsDisabled = Status.ToLower() != "Pending".ToLower() && Status.ToLower() != "All".ToLower() ? true : false
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-circle-x",
                ActionMethod = Cancel,
                ButtonClass = "btn-danger",
                IsDisabled = Status.ToLower() != "Pending".ToLower() && Status.ToLower() != "All".ToLower() ? true : false
            };
            RowActions = new List<IRowActions> { };
            if (LeaveRolePermission != null)
            {
                if (LeaveRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (LeaveRolePermission.Delete == true)
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
            HeaderAction = new List<IHeaderActions> { };
            if (LeaveRolePermission != null && LeaveRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }
        }
        private async Task LoadLeaveBalance()
        {
            if(EmployeeID != 0)
            {
                var respLeaveBalance = (await LeaveBalanceService.GetLeaveBalance(EmployeeID));
                if (respLeaveBalance.MessageType == MessageType.Success)
                    LeaveBalanceList = respLeaveBalance.Data.ToList();
                else
                    LeaveBalanceList = new List<LeaveBalance> { };
            }
            StateHasChanged();
        }
        protected async Task LoadLeaveRequest()
        {
            await LoadLeaveBalance();
            lstLeaveRequest = null;
            statusId = 0;
            LoadTableConfig(statusId);
            var respLeaveRequest = (await LeaveService.GetLeave("EmployeeID",EmployeeID));
            if (respLeaveRequest.MessageType == MessageType.Success)
            {
                LeaveRequestList = respLeaveRequest.Data.ToList();
                lstLeaveRequest = LeaveRequestList;
            }
            else
            {
                LeaveRequestList = new List<Leave> { };
                lstLeaveRequest = LeaveRequestList;
            }
            StateHasChanged();
        }

        protected async Task loadList(int StatusID)
        {
            //LoadTableConfig(StatusID);
            if (StatusID != 0)
            {
                status = StatusList.Find(x => x.ListValueID == StatusID).ValueDesc;
                lstLeaveRequest = LeaveRequestList.Where(x => x.Status.ToUpper() == status.ToUpper()).ToList();
            }else
            {
                lstLeaveRequest = LeaveRequestList;
            }
            StateHasChanged();
        }
        protected async Task onStatusChange(ChangeEventArgs e)
        {
            int StatusId = Convert.ToInt32(e.Value);
            LoadTableConfig(StatusId);
            await loadList(StatusId);
        }
        private void Add()
        {
            AddEditLeaveModal.Show(0, EmployeeID, LeaveRequestList);
        }        
        private void Edit()
        {
            AddEditLeaveModal.Show(selected.LeaveID, EmployeeID, LeaveRequestList);
        }
        private void Cancel()
        {
            AddEditLeaveModal.Show(selected.LeaveID, EmployeeID, LeaveRequestList, true);
            //CancelLeave();
        }
        private async Task CancelLeave()
        {
            selected.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
            var result = await LeaveService.UpdateLeave(selected.LeaveID, selected);
            if (result.MessageType == MessageType.Success)
            {
                lstLeaveRequest = new List<Leave> { };
                await LoadLeaveRequest();
                await loadList(0);
                toastService.ShowSuccess("Leave Request cancelled successfully", "");
                StateHasChanged();
            }
        }
        protected async Task RefreshList()
        {
            await LoadLeaveRequest();
            await loadList(statusId);
            StateHasChanged();
        }
        public void RowClick(Leave data)
        {
            selected = data;
            StateHasChanged();
        }
        public void EditMobile(ILT.IHR.DTO.Leave data)
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
        protected void OnDrpDwnChange(ChangeEventArgs e)
        {
            DefaultStatusID = Convert.ToInt32(e.Value);
            onSatusChange(DefaultStatusID);
            // onDropDownChange.Invoke(DefaultID);
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
    }
}
