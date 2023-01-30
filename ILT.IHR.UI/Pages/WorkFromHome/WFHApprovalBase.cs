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

namespace ILT.IHR.UI.Pages.WorkFromHome
{
    public class WFHApprovalBase : ComponentBase
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
        public List<WFH> lstWFHRequest { get; set; }  // Table APi Data
        public List<WFH> WFHRequestList { get; set; }  // Table APi Data
        [Inject]
        public IWorkFromHomeService WorkFromHomeService { get; set; }
        public List<IRowActions> RowActions { get; set; } //Row Actions
        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateWFHList { get; set; }
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected WFH selected;
        protected WFH WFH = new WFH();
        private int EmployeeID { get; set; }
        protected int DefaultStatusId { get; set; }
        private DTO.User user { get; set; }
        protected ApproveDenyWFHBase ApproveDenyWFHModal { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            await LoadTableConfig();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WFHSTATUS).ToList();
                DefaultStatusId = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
            }
            setStatusList();
            await LoadWFHRequest();
            StateHasChanged();
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

        protected async Task LoadWFHRequest()
        {
            var respWFHRequest = (await WorkFromHomeService.GetWFH("ApproverID", EmployeeID));
            if (respWFHRequest.MessageType == MessageType.Success)
            {
                WFHRequestList = respWFHRequest.Data.ToList();
                loadList("PENDING");
                DefaultStatusId = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
                StateHasChanged();
            }
            else
            {
                WFHRequestList = new List<WFH> { };
            }
        }
        protected void loadList(string Status)
        {
            lstWFHRequest = WFHRequestList.Where(x => x.Status.ToUpper() == Status.ToUpper()).ToList();
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
                lstWFHRequest = WFHRequestList;
            }
        }

        private void Edit()
        {
            ApproveDenyWFHModal.Show(selected.WFHID);
        }

        protected async Task RefreshList()
        {
            var respWFHRequest = (await WorkFromHomeService.GetWFH("ApproverID", EmployeeID));
            if (respWFHRequest.MessageType == MessageType.Success)
            {
                WFHRequestList = respWFHRequest.Data.ToList();
                if(DefaultStatusId != 0)
                {
                    string status = StatusList.Find(x => x.ListValueID == DefaultStatusId).ValueDesc;
                    lstWFHRequest = WFHRequestList.Where(x => x.Status.ToUpper() == status.ToUpper()).ToList();
                    StateHasChanged();
                }
                else
                {
                    lstWFHRequest = WFHRequestList;
                }
            }
            else
            {
                WFHRequestList = new List<WFH> { };
            }
            await UpdateWFHList.InvokeAsync(true);
            StateHasChanged();
        }
        public void RowClick(WFH data)
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
                lstWFHRequest = WFHRequestList;
                StateHasChanged();
            }
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
