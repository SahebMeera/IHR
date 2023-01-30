using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using System.Linq;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeW4
{
    public class AddEditEmployeeW4Base : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service   
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IEmployeeW4Service EmployeeW4Service { get; set; } //Service
        public List<ListValue> withHoldingStausList { get; set; }  // Table APi Data
        public List<ListValue> W4TypeList { get; set; }  // Table APi Data
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int EmployeeW4ID { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public DTO.EmployeeW4 EmployeeW4 = new DTO.EmployeeW4();
        public ILT.IHR.DTO.User user;
        protected bool isShow { get; set; }
        public bool isAllowance { get; set; } = false;
        public bool isQualifyingChildren { get; set; } = false;
        public bool isOtherDependents { get; set; } = false;
        public bool isOtherIncome { get; set; } = false;
        public bool isDeductions { get; set; } = false;
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public RolePermission NPIPermission { get; set; }
        public List<RolePermission> RolePermissions;
        public bool isSaveButtonDisabled { get; set; } = false;
        public string SSNNumber { get; set; }
        public bool isViewPermissionForNPIRole { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.W4INFO);
            NPIPermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.NPI);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            isViewPermissionForNPIRole = !NPIPermission.View;
            await LoadDropDown();
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
        public void Show(int Id, int employeeId, string SSN)

        {
            EmployeeW4ID = Id;
            EmployeeW4.EmployeeID = employeeId;
            EmployeeW4.SSN = SSN;
            ResetDialog();
            if (EmployeeW4ID != 0)
            {
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(EmployeeW4ID, employeeId);
                
            }
            else
            {
                isShow = EmployeeInfoRolePermission.Add;
                Title = "Add";
                EmployeeW4.StartDate = DateTime.Now;
                EmployeeW4.EmployeeID = employeeId;
                EmployeeW4.SSN = SSN;
                SSNNumber =  "***-**-****";
                isMultipleJob = false;
                setW4Type("0");
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
        }
        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                withHoldingStausList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WITHHOLDINGSTATUS).ToList();
                W4TypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.W4TYPE).ToList();
            }
        }

        public bool w4TypePre { get; set; } = true;
        public bool w4TypeCurrent { get; set; } = true;
        public bool isMultipleJob { get; set; } = false;
        protected async Task onChangeW4Type(ChangeEventArgs e)
        {
            var w4Type = Convert.ToString(e.Value);
            setW4Type(w4Type);
        }
        protected async Task setW4Type(string type)
        {
            if (!String.IsNullOrEmpty(type) && type != "0")
            {

                if (W4TypeList.Find(x => x.ListValueID == Convert.ToInt32(type)).Value == "CURR")
                {
                    w4TypePre = false;
                    EmployeeW4.W4Type = W4TypeList.Find(x => x.ListValueID == Convert.ToInt32(type)).ValueDesc;
                    w4TypeCurrent = true;
                    isMultipleJob = true;
                }
                else
                {
                    w4TypePre = true;
                    EmployeeW4.W4Type = W4TypeList.Find(x => x.ListValueID == Convert.ToInt32(type)).ValueDesc;
                    w4TypeCurrent = false;
                    isMultipleJob = false;
                }
            }
            else
            {
                w4TypePre = true;
                w4TypeCurrent = true;
                isMultipleJob = false;
            }
        }

        private async Task GetDetails(int Id, int employeeId)
        {
            Response<DTO.EmployeeW4> resp = new Response<DTO.EmployeeW4>();
            resp = await EmployeeW4Service.GetEmployeeW4ByIdAsync(Id);
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeW4 = resp.Data;
                SSNNumber = NPIPermission.View == true ? EmployeeW4.SSN : "***-**-****";
                setW4Type(Convert.ToString(EmployeeW4.W4TypeID));
                Title = "Edit";
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
            StateHasChanged();
        }

        public void Cancel()
        {
            ShowDialog = false;
            // ListValueUpdated.InvokeAsync(true);
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            EmployeeW4 = new DTO.EmployeeW4 { };
        }

        protected async Task checkSaveEmployeeW4()
        {
            if (W4TypeList.Find(x => x.ListValueID == Convert.ToInt32(EmployeeW4.W4TypeID)).Value == "PRE2020")
            {
                if (isAllowance == false)
                {
                    SaveEmployeeW4();
                }
            }
            else
            {
                if (isDeductions == false && isOtherIncome == false && isQualifyingChildren == false && isOtherDependents == false)
                {
                    SaveEmployeeW4();
                }
            }
        }

        protected async Task SaveEmployeeW4()
        {
            EmployeeW4.EmployeeName = EmployeeName;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (EmployeeW4ID == 0)
            {
                EmployeeW4.CreatedBy = user.FirstName + " " + user.LastName;
                // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(Contact);
                var result = await EmployeeW4Service.SaveEmployeeW4(EmployeeW4);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("EmployeeW4 saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (EmployeeW4ID != 0)
            {
                EmployeeW4.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await EmployeeW4Service.UpdateEmployeeW4(EmployeeW4ID, EmployeeW4);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("EmployeeW4 saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            isSaveButtonDisabled = false;
        }

        protected async Task onChangeWithHoldingStatus(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && withHoldingStausList != null)
            {
                var withHoldingStatus = Convert.ToInt32(e.Value);
                EmployeeW4.WithHoldingStatus = withHoldingStausList.Find(x => x.ListValueID == withHoldingStatus).ValueDesc;
            }
        }
    }
}
