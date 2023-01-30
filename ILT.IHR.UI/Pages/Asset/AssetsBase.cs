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
using ILT.IHR.UI.Pages.Asset.AssetChangesSet;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Asset
{
    public class AssetsBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize {get; set;}
       
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service        
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IAssetService AssetService { get; set; } //Service

        public List<ILT.IHR.DTO.Asset> AssetsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Asset> lstAssetsList { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

        public AddEditAssetBase AddEditAssetModal { get; set; }
        public AssetChangeSetBase AssetChangeSetModal { get; set; }
        public List<AssetYear> LeaveYearList { get; set; }
        protected List<IDropDownList> lstYear { get; set; }
        protected int yearId { get; set; }

        protected ILT.IHR.DTO.Asset selected;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission AssetRolePermission;
        protected DTO.User user { get; set; }
        public List<ListValue> AssetTypeList { get; set; }  // Table APi Data

        public List<IMultiSelectDropDownList> lstAssetType { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketStatus { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> employeeTypeList { get; set; }
        public List<IMultiSelectDropDownList> ListAssetStatus{ get; set; }
        public List<IMultiSelectDropDownList> selectedAssetTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedAssetStatusList { get; set; }

        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public string AssetType { get; set; }
        public string TicketStatus { get; set; }
        public List<ListValue> AssetStatusList { get; set; }  // Table APi Data



        protected async override Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            lstAssetType = new List<IMultiSelectDropDownList>();
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            AssetsList = new List<ILT.IHR.DTO.Asset> { };
            lstTicketStatus = new List<IMultiSelectDropDownList>();
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {

                AssetStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETSTATUS).ToList();
                if (TicketStatus == null)
                {
                    setEmployeeType1List();
                }
                else
                {
                    lstTicketStatus = ListAssetStatus;
                }
            }
            await LoadDropDown();
            //yearId = Convert.ToInt32(LeaveYearList.Find(x => x.text.ToLower() == "Current Year".ToLower()).year);
            await LoadList();
        }
        protected void setEmployeeType1List()
        {
            lstTicketStatus.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstTicketStatus = (from employee in AssetStatusList
                               select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (TicketStatus == null)
            {
                TicketStatus = "Unassigned";
            }
            lstTicketStatus.Insert(0, ListItem);
            lstTicketStatus.ForEach(x => {
                if (x.Value.ToLower() == TicketStatus.ToLower())
                {
                    x.IsSelected = true;
                }
                if (x.Value.ToLower() == "Assigned".ToLower())
                {
                    x.IsSelected = true;
                }
                if (x.Value.ToLower() == "Assigned Temp".ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedAssetStatusList = lstTicketStatus.FindAll(x => x.IsSelected == true);
            DropDown2DefaultID = lstTicketStatus.Find(x => x.Value.ToLower() == TicketStatus.ToLower()).ID;
        }
        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                AssetTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETTYPE).ToList();
                if (AssetType == null)
                {
                    setEmployeeTypeList();
                }
                else
                {
                    lstAssetType = employeeTypeList;
                }
            }
        }

        public void onChangeAssetStatusList(List<IMultiSelectDropDownList> assetTypesList)
        {
            ListAssetStatus = assetTypesList;
            selectAssetStatusList();
            LoadList();
        }
        public void selectAssetStatusList()
        {
            selectedAssetStatusList = ListAssetStatus.FindAll(x => x.IsSelected == true);
            var selectAssetType = ListAssetStatus.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                TicketStatus = "All";
            }
            else
            {
                TicketStatus = "NotAll";
            }
        }
        public void onChangeAssetTypeList(List<IMultiSelectDropDownList> assetTypesList)
        {
            employeeTypeList = assetTypesList;
            selectAssetTypeList();
            LoadList();
        }
        public void selectAssetTypeList()
        {
            selectedAssetTypeList = employeeTypeList.FindAll(x => x.IsSelected == true);
            var selectAssetType = employeeTypeList.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                AssetType = "All";
            }
            else
            {
                AssetType = "NotAll";
            }
        }
        private async Task LoadTableConfig()
        {
            AssetRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.ASSET);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-loop",
                ActionMethod = changeLog,
                ButtonClass = "btn-primary"
            };

            RowActions = new List<IRowActions> { };
            if (AssetRolePermission != null)
            {
                if (AssetRolePermission.Update == true)
                {
                    RowActions.Add(m1);
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

            if (AssetRolePermission != null && AssetRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }

            AssetsList = new List<ILT.IHR.DTO.Asset> { };
        }

        public void Delete()
        {
            if (selected != null)
            {
                DeleteConfirmation.Show();
            }
        }
        public void changeLog()
        {
          AssetChangeSetModal.show(selected.AssetID);
        }

        public void Edit()
        {
            if (selected != null)
            {
                 AddEditAssetModal.Show(selected.AssetID);
            }
        }
        public void Add()
        {
            AddEditAssetModal.Show(0);
        }

        protected async Task LoadList()
        {

            string RoleShort = await sessionStorage.GetItemAsync<string>("RoleShort");
            await LoadTableConfig();
            var reponses = new Response<IEnumerable<DTO.Asset>>();
                reponses = (await AssetService.GetAssets());
            if (reponses.MessageType == MessageType.Success)
            {
                if (reponses.Data != null && (RoleShort.ToUpper() == UserRole.EMP ||  RoleShort.ToUpper() == UserRole.CONTRACTOR))
                {

                    AssetsList = reponses.Data.Where(x => x.AssignedToID == this.user.EmployeeID).ToList();
                    loadAssetList();
                  //  lstAssetsList = AssetsList;
                   // StateHasChanged();
                }
                else
                {
                    AssetsList = reponses.Data.ToList();
                    loadAssetList();
                    //  lstAssetsList = AssetsList;
                   // StateHasChanged();
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);

            }
        }
        public void loadAssetList()
        {
            if (AssetType != "All" && selectedAssetTypeList != null && TicketStatus != "All" && selectedAssetStatusList != null)
            {
                lstAssetsList = AssetsList.Where(x => selectedAssetTypeList.Any(s => s.ID == x.AssetTypeID) && selectedAssetStatusList.Any(t => t.ID == x.StatusID)).ToList();
            }
            else if (AssetType == "All" && TicketStatus != "All")
            {
                lstAssetsList = AssetsList.Where(x => selectedAssetStatusList.Any(t => t.ID == x.StatusID)).ToList();

            } else if (AssetType != "All" && TicketStatus == "All")
            {
                lstAssetsList = AssetsList.Where(x => selectedAssetTypeList.Any(s => s.ID == x.AssetTypeID)).ToList();
            }
            else
            {
                lstAssetsList = AssetsList;
            }
            StateHasChanged();
        }

        public void RowClick(ILT.IHR.DTO.Asset data)
        {
            selected = data;
           // StateHasChanged();
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                string RoleName = await sessionStorage.GetItemAsync<string>("RoleName");
                await AssetService.DeleteAsset(selected.AssetID); 
                var reponses = (await AssetService.GetAssets());
                if (reponses.MessageType == MessageType.Success)
                {

                    //if (reponses.Data != null && RoleName.ToUpper() == "EMPLOYEE")
                    //{

                    //    AssetsList = reponses.Data.Where(x => x.AssignedToID == this.user.EmployeeID).ToList();
                    //    lstAssetsList = AssetsList;
                    //    StateHasChanged();
                    //}
                    //else
                    //{
                        AssetsList = reponses.Data.ToList();
                        lstAssetsList = AssetsList;
                    //}
                   
                   
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
                toastService.ShowSuccess("Asset Deleted successfully", "");
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
        public class AssetYear
        {
            public string year { get; set; }
            public string text { get; set; }
        }

 

        public int DropDown2DefaultID { get; set; }
        protected void setEmployeeTypeList()
        {
            lstAssetType.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstAssetType = (from employee in AssetTypeList
                               select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (AssetType == null)
            {
                AssetType = "All";
            }
            lstAssetType.Insert(0, ListItem);
            lstAssetType.ForEach(x => {
                   x.IsSelected = true;
            });
            selectedAssetTypeList = lstAssetType.FindAll(x => x.IsSelected == true);
            DropDown2DefaultID = lstAssetType.Find(x => x.Value.ToLower() == AssetType.ToLower()).ID;
        }

        protected void SetYearList()
        {
            lstYear = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstYear = (from lookupItem in LeaveYearList
                       select new IDropDownList { ID = Convert.ToInt32(lookupItem.year), Value = lookupItem.text }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "Select";
            lstYear.Insert(0, ListItem);
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
        public void EditMobile(ILT.IHR.DTO.Asset data)
        {
            selected = data;
            Edit();
        }

        public void OnMultiDropDownChange(ChangeEventArgs e, int key)
        {
            var index = lstAssetType.FindIndex(x => x.ID == key);
            var userRole = lstAssetType.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstAssetType.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstAssetType.FindIndex(x => x.ID == 0);
                if (lstAssetType[isAllSelectOrNot].IsSelected == true)
                {
                    lstAssetType[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstAssetType[index] = userRole;
            onChangeAssetTypeList(lstAssetType);
        }
        protected string getSelectedRoles()
        {
            string roles = "";
            if (lstAssetType != null)
            {
                if (lstAssetType.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstAssetType)
                    {

                        if (string.IsNullOrEmpty(roles) && item.IsSelected)
                        {
                            roles = item.Value;

                        }
                        else if (item.IsSelected)
                        {
                            roles = roles + ", " + item.Value;
                        }
                    }
                }
            }
            return roles;
        }
        protected void OnMultiDropDown1Change(ChangeEventArgs e, int key)
        {
            var index = lstTicketStatus.FindIndex(x => x.ID == key);
            var userRole = lstTicketStatus.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstTicketStatus.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstTicketStatus.FindIndex(x => x.ID == 0);
                if (lstTicketStatus[isAllSelectOrNot].IsSelected == true)
                {
                    lstTicketStatus[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstTicketStatus[index] = userRole;
            onChangeAssetStatusList(lstTicketStatus);
        }

        protected string getSelectedRoles1()
        {
            string roles = "";
            if (lstTicketStatus != null)
            {
                if (lstTicketStatus.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstTicketStatus)
                    {

                        if (string.IsNullOrEmpty(roles) && item.IsSelected)
                        {
                            roles = item.Value;

                        }
                        else if (item.IsSelected)
                        {
                            roles = roles + ", " + item.Value;
                        }
                    }
                }
            }
            return roles;
        }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
    }
}
