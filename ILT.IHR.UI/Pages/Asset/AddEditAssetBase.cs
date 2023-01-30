using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;


namespace ILT.IHR.UI.Pages.Asset
{
    public class AddEditAssetBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public IAssetService AssetService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.Asset> Assets { get; set; } //Drop Down Api Data       
        private int AssetId { get; set; }
        [Parameter]
        public EventCallback<bool> AssetUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        protected string Title = "Add";
        public ILT.IHR.DTO.Asset Asset = new ILT.IHR.DTO.Asset();
        public bool ShowDialog { get; set; }
        public bool disabledvalue { get; set; }
        public bool isAssignedTemp { get; set; }
        public List<Country> CountryList { get; set; }
        public ILT.IHR.DTO.User user;
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public List<ListValue> AssetTypeList { get; set; }  // Table APi Data
        public List<ListValue> AssetStatusList { get; set; }  // Table APi Data
        public List<ListValue> AssetStatus2List { get; set; }  // Table APi Data
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service 
        public IEnumerable<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data  
        public IEnumerable<ILT.IHR.DTO.Asset> AssetList { get; set; }  // Table APi Data
        public bool isSaveButtonDisabled { get; set; } = false;


        protected override async Task OnInitializedAsync()
        {
            AssetList = new List<ILT.IHR.DTO.Asset> { };
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
            LoadList();
        }
        protected async Task LoadList()
        {

            var reponses = (await AssetService.GetAssets());
            if (reponses.MessageType == MessageType.Success)
            {
                AssetList = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

        }

        public string ErrorMessage;
        protected async Task checkTagExist()
        {
            ErrorMessage = "";
            if (AssetId != 0)
            {
                if (!String.IsNullOrEmpty(Asset.Tag))
                {
                    if ((AssetList.ToList().FindAll(x => x.AssetID != Asset.AssetID && x.Tag.ToUpper() == Asset.Tag.Trim().ToUpper()).Count > 0))
                    {
                        ErrorMessage = "Asset Tag already exists in the system";
                    }
                    else
                    {
                        SaveAsset();
                    }
                }
            }
            else
            {
                if ((AssetList.ToList().FindAll(x => x.Tag.ToUpper() == Asset.Tag.Trim().ToUpper()).Count > 0))
                {
                    ErrorMessage = "Asset Tag already exists in the system";
                }
                else
                {
                    SaveAsset();
                }
            }

        }

        protected async Task SaveAsset()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (AssetId == 0)
                {
                Asset.CreatedBy = user.FirstName + " " + user.LastName;
                if (Asset.AssignedToID == 0)
                {
                    Asset.AssignedToID = null;
                }
                var result = await AssetService.SaveAsset(Asset);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Asset saved successfully", "");
                        AssetUpdated.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                else if (AssetId > 0)
                {
                    Asset.ModifiedBy = user.FirstName + " " + user.LastName;
                if (Asset.AssignedToID == 0)
                {
                    Asset.AssignedToID = null;
                    Asset.AssignedTo = "";
                }
                    var result = await AssetService.UpdateAsset(AssetId, Asset);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Asset saved successfully", "");
                        AssetUpdated.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
               isSaveButtonDisabled = false;

        }

        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());

            if (resp.MessageType == MessageType.Success)
            {
                AssetTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETTYPE).ToList();
                AssetStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETSTATUS).ToList();
                AssetStatus2List = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETSTATUS).ToList();
            }
            var respEmployees = (await EmployeeService.GetEmployees());
            if (respEmployees.MessageType == MessageType.Success)
            {
                Employees = respEmployees.Data;
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.Asset> resp = new Response<ILT.IHR.DTO.Asset>();
            resp = await AssetService.GetAssetByIdAsync(Id) as Response<ILT.IHR.DTO.Asset>;
            if(resp.MessageType == MessageType.Success)
            {
                Asset = resp.Data;
                isAssignedRequired = false;
                isAssignedTempRequired = false;
                    if (Asset.StatusID != null)
                    {
                        var AssetStatus = AssetStatus2List.Find(x => x.ListValueID == Asset.StatusID);
                        if (AssetStatus != null && AssetStatus.Value.ToUpper() == AssetStatusConstants.ASSIGNEDTEMP)
                        {
                            isAssignedTemp = true;
                        }
                        else
                        {
                            isAssignedTemp = false;
                        if (AssetStatus != null && AssetStatus.Value.ToUpper() == AssetStatusConstants.ASSIGNED)
                            {
                              AssetStatusList = AssetStatus2List.Where(x => x.Value.ToUpper() != AssetStatusConstants.DECOMMISSIONED && x.Value.ToUpper() != AssetStatusConstants.UNASSIGNED).ToList();
                            }
                            else
                            {
                               AssetStatusList = AssetStatus2List.Where(x => x.Value.ToUpper() != AssetStatusConstants.ASSIGNED).ToList();
                            }
                        }
                    }
            }
            Title = "Edit";
            ErrorMessage = "";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            AssetId = -1;
            ShowDialog = false;
            // UserUpdated.InvokeAsync(true);
            StateHasChanged();

        }
        public async void Show(int Id)
        {
            AssetId = Id;
            ResetDialog();
            if (AssetId != 0)
            {
                disabledvalue = true;
                GetDetails(AssetId);
            }
            else
            {
                Title = "Add";
                disabledvalue = false;
                Asset.PurchaseDate = DateTime.Now;
                isfirstElementFocus = true;
                isAssignedTemp = false;
                isAssignedRequired = false;
                if (AssetStatus2List != null)
                {
                    AssetStatusList = AssetStatus2List.Where(x => x.Value.ToUpper() != AssetStatusConstants.ASSIGNED).ToList();
                    Asset.StatusID = AssetStatusList.Find(x => x.ValueDesc.ToUpper() == AssetStatusConstants.UNASSIGNED).ListValueID;
                }
                ErrorMessage = "";
                ShowDialog = true;
                StateHasChanged();
            }
        }
       
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        private void ResetDialog()
        {
            Asset = new ILT.IHR.DTO.Asset { };
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }
        protected async Task onAssignedChange(ChangeEventArgs e)
        {
            ErrorMessage = "";
            if(Convert.ToInt32(e.Value) != 0)
            {
                if (Employees.ToList().Find(x => x.EmployeeID == Convert.ToInt32(e.Value)).TermDate != null)
                {
                    isSaveButtonDisabled = true;
                    ErrorMessage = "Please select active employee.";
                }
                else
                {
                    isSaveButtonDisabled = false;
                    onAssetStatus(Convert.ToInt32(e.Value));
                }
            } else
            {
                isSaveButtonDisabled = false;
                onAssetStatus(Convert.ToInt32(e.Value));
            }
        }
        protected void onAssignedTempChange()
        {
            var AssignedTo = Asset.AssignedTo;
            if (!string.IsNullOrEmpty(AssignedTo))
            {
                isAssignedTempRequired = false;
            }
            else
            {
                isAssignedTempRequired = true;
            }
        }

        protected async Task onAssetStatus(int? status)
        {
            if (Convert.ToInt32(status) != 0 && status != null)
            {
                isAssignedRequired = false;
                AssetStatusList = AssetStatus2List.Where(x => x.Value.ToUpper() != AssetStatusConstants.DECOMMISSIONED && x.Value.ToUpper() != AssetStatusConstants.UNASSIGNED).ToList();
                Asset.StatusID = AssetStatusList.Find(x => x.ValueDesc.ToUpper() == AssetStatusConstants.ASSIGNED).ListValueID;
            }
            else
            {
                AssetStatusList = AssetStatus2List.Where(x => x.Value.ToUpper() != AssetStatusConstants.ASSIGNED).ToList();
                Asset.StatusID = AssetStatusList.Find(x => x.ValueDesc.ToUpper() == AssetStatusConstants.UNASSIGNED).ListValueID;
            }
        }
        public bool isAssignedRequired { get; set; }
        public bool isAssignedTempRequired { get; set; }
        protected async Task onStatusChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null)
               {
                var status = Convert.ToInt32(e.Value);
                var AssetStatus = AssetStatusList.Find(x => x.ListValueID == status);
                if (AssetStatus != null && AssetStatus.Value.ToUpper() == AssetStatusConstants.ASSIGNEDTEMP)
                {
                    isAssignedRequired = false;
                    isAssignedTemp = true;
                    isAssignedTempRequired = true;
                    Asset.AssignedTo = "";
                    Asset.AssignedToID = null;
                }
                else
                {
                    if (AssetStatus != null && AssetStatus.Value.ToUpper() == AssetStatusConstants.ASSIGNED && Asset.AssignedToID == null)
                    {
                        isAssignedRequired = true;
                        Asset.AssignedTo = "";
                        isAssignedTemp = false;
                        isAssignedTempRequired = false;
                    } else
                    {
                        Asset.AssignedTo = "";
                        isAssignedTemp = false;
                        isAssignedRequired = false;
                        isAssignedTempRequired = false;
                    }
                }
            }
            else
            {
                isAssignedTemp = false;
                isAssignedTempRequired = false;
            }
        }
        

    }
}
