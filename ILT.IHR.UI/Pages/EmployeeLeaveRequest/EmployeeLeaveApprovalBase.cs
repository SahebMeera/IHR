using Blazored.SessionStorage;
using BlazorTable;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using ILT.IHR.UI.Service;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.EmployeeLeaveRequest
{
    public class EmployeeLeaveApprovalBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        private ILookupService LookupService { get; set; }
        [Inject]
        private IEmployeeService EmployeeService { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        public List<Leave> lstLeaveRequest { get; set; }  // Table APi Data
        public List<Leave> LeaveRequestList { get; set; }  // Table APi Data
        [Inject]
        protected ILeaveService LeaveService { get; set; }
        [Inject]
        protected DataProvider dataProvider { get; set; }
        public List<IRowActions> RowActions { get; set; } //Row Actions
        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected Leave selected;
        protected Leave Leave = new Leave();
        private int EmployeeID { get; set; }
        protected int DefaultStatusId { get; set; }
        private DTO.User user { get; set; }
        protected ApproveDenyLeaveBase ApproveDenyLeaveModal { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            await LoadTableConfig();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
                DefaultStatusId = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
            }
            setStatusList();
            await LoadLeaveRequest();
            StateHasChanged();

            if (dataProvider.storage != null)
            {
                DTO.Leave leave = (DTO.Leave)dataProvider.storage;
                ApproveDenyLeaveModal.Show(leave.LeaveID);
                dataProvider.storage = null;
            }
        }

        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-eye",
                ActionMethod =  Edit,
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

        protected async Task LoadLeaveRequest()
        {
            var respLeaveRequest = (await LeaveService.GetLeave("ApproverID", EmployeeID));
            if (respLeaveRequest.MessageType == MessageType.Success)
            {
                LeaveRequestList = respLeaveRequest.Data.ToList();
                loadList("PENDING");
                DefaultStatusId = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
                StateHasChanged();
            }
            else
            {
                LeaveRequestList = new List<Leave> { };
            }
        }
        protected void loadList(string Status)
        {
            lstLeaveRequest = LeaveRequestList.Where(x => x.Status.ToUpper() == Status.ToUpper()).ToList();
            StateHasChanged();
        }
        protected async Task onStatusChange(ChangeEventArgs e)
        {
            string status;
            int StatusId = Convert.ToInt32(e.Value);
            if (StatusId != 0)
            {
                status = StatusList.Find(x => x.ListValueID == StatusId).ValueDesc;
                loadList(status);
            }
            else
            {
                lstLeaveRequest = LeaveRequestList;
            }
        }

        private void Edit()
        {
            ApproveDenyLeaveModal.Show(selected.LeaveID);
        }

        protected async Task RefreshList()
        {
            var respLeaveRequest = (await LeaveService.GetLeave("ApproverID", EmployeeID));
            if (respLeaveRequest.MessageType == MessageType.Success)
            {
                LeaveRequestList = respLeaveRequest.Data.ToList();
                if(DefaultStatusId != 0)
                {
                    string status = StatusList.Find(x => x.ListValueID == DefaultStatusId).ValueDesc;
                    lstLeaveRequest = LeaveRequestList.Where(x => x.Status.ToUpper() == status.ToUpper()).ToList();
                    StateHasChanged();
                }
                else
                {
                    lstLeaveRequest = LeaveRequestList;
                }
            }
            else
            {
                LeaveRequestList = new List<Leave> { };
            }
            await UpdateLeaveList.InvokeAsync(true);
            StateHasChanged();
        }
        public void RowClick(Leave data)
        {
            selected = data;
            StateHasChanged();
        }
        public void onStatusChange(int StatusID)
        {
            string status;
            if (StatusID != 0)
            {
                DefaultStatusId = StatusID;
                status = StatusList.Find(x => x.ListValueID == StatusID).ValueDesc;
                loadList(status);
                StateHasChanged();
            }
            else
            {
                DefaultStatusId = StatusID;
                lstLeaveRequest = LeaveRequestList;
                StateHasChanged();
            }
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
        public void EditMobile(ILT.IHR.DTO.Leave data)
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
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
    }
}
