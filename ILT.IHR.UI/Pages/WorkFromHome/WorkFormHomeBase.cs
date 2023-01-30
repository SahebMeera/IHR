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
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.WorkFromHome
{
    public class WorkFromHomeBase : ComponentBase
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
        public IWorkFromHomeService WorkFromHomeService { get; set; }
        public List<WFH> WFHList { get; set; }  // Table APi Data
        public List<WFH> lstOfWFH { get; set; }  // Table APi Data
        protected List<ListValue> StatusList { get; set; }
        protected List<IDropDownList> lstStatus { get; set; }
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        protected WFH selected { get; set; }
        private int EmployeeID { get; set; }
        protected int statusId { get; set; }
        protected int DefaultStatusID { get; set; }
        public List<RolePermission> RolePermissions;
        public RolePermission WFHRolePermission;
        public string status { get; set; }
        public AddEditWFHBase AddEditWFHModal { get; set; }
        public TabControl WFHTabControl { get; set; }
        [Inject]
        public DataProvider dataProvider { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            DefaultStatusID = 0;
            WFHList = new List<WFH> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            var user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WFHSTATUS).ToList();
                setStatusList();
                LoadTableConfig(0);
            }
            await LoadWFHRequest();
            if (dataProvider != null && dataProvider.TabIndex == 1)
            {
                WFHTabControl.ActivePageByIndex(1);
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
            WFHRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.WFHREQUEST);
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
            if (WFHRolePermission != null)
            {
                if (WFHRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (WFHRolePermission.Delete == true)
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
            if (WFHRolePermission != null && WFHRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }
        }

        protected async Task LoadWFHRequest()
        {
            lstOfWFH = null;
            statusId = 0;
            LoadTableConfig(statusId);
            var respWFHRequest = (await WorkFromHomeService.GetWFH("EmployeeID", EmployeeID));
            if (respWFHRequest.MessageType == MessageType.Success)
            {
                WFHList = respWFHRequest.Data.ToList();
                lstOfWFH = WFHList;
            }
            else
            {
                WFHList = new List<WFH> { };
                lstOfWFH = WFHList;
            }
            StateHasChanged();
        }

        protected async Task loadList(int StatusID)
        {
            //LoadTableConfig(StatusID);
            if (StatusID != 0)
            {
                status = StatusList.Find(x => x.ListValueID == StatusID).ValueDesc;
                lstOfWFH = WFHList.Where(x => x.Status.ToUpper() == status.ToUpper()).ToList();
            }
            else
            {
                lstOfWFH = WFHList;
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
            AddEditWFHModal.Show(0, EmployeeID);
        }        
        private void Edit()
        {
            AddEditWFHModal.Show(selected.WFHID, EmployeeID);
        }
        private void Cancel()
        {
            AddEditWFHModal.Show(selected.WFHID, EmployeeID, true);
        }
        private async Task CancelLeave()
        {
            selected.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
            var result = await WorkFromHomeService.UpdateWFH(selected.WFHID, selected);
            if (result.MessageType == MessageType.Success)
            {
                lstOfWFH = new List<WFH> { };
                await LoadWFHRequest();
                await loadList(0);
                toastService.ShowSuccess("WorkFormHome Request cancelled successfully", "");
                StateHasChanged();
            }
        }
        protected async Task RefreshList()
        {
            await LoadWFHRequest();
            await loadList(statusId);
            StateHasChanged();
        }
        public void RowClick(WFH data)
        {
            selected = data;
            StateHasChanged();
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
    }
}
