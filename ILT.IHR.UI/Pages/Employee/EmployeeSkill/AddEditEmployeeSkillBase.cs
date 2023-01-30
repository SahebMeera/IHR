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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Web;

namespace ILT.IHR.UI.Pages.Employee.EmployeeSkill
{
    public class AddEditEmployeeSkillBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service   
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IEmployeeSkillService EmployeeSkillService { get; set; } //Service
        public List<ListValue> SkillTypeList { get; set; }  // Table APi Data
        public List<ListValue> W4TypeList { get; set; }  // Table APi Data
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int EmployeeSkillID { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public DTO.EmployeeSkill EmployeeSkill = new DTO.EmployeeSkill();
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
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SKILL);
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
        public async Task Keypress(KeyboardEventArgs e)
        {
            JSRuntime.InvokeVoidAsync("SkillValidation");
        }
        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                SkillTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.SKILLTYPE).ToList();
            }
        }
        public void Show(int Id, int employeeId)

        {
            EmployeeSkillID = Id;
            EmployeeSkill.EmployeeID = employeeId;
            ResetDialog();
            if (EmployeeSkillID != 0)
            {
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(EmployeeSkillID, employeeId);
                
            }
            else
            {
                isShow = EmployeeInfoRolePermission.Add;
                Title = "Add";
                EmployeeSkill.EmployeeID = employeeId;
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
        }
      

     
   

        private async Task GetDetails(int Id, int employeeId)
        {
            Response<DTO.EmployeeSkill> resp = new Response<DTO.EmployeeSkill>();
            resp = await EmployeeSkillService.GetEmployeeSkillByIdAsync(Id);
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeSkill = resp.Data;
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
            EmployeeSkill = new DTO.EmployeeSkill { };
        }

       

        protected async Task SaveEmployeeSkill()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (EmployeeSkillID == 0)
            {
                EmployeeSkill.CreatedBy = user.FirstName + " " + user.LastName;
                // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(Contact);
                var result = await EmployeeSkillService.SaveEmployeeSkill(EmployeeSkill);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("EmployeeSkill saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (EmployeeSkillID != 0)
            {
                EmployeeSkill.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await EmployeeSkillService.UpdateEmployeeSkill(EmployeeSkillID, EmployeeSkill);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("EmployeeSkill saved successfully", "");
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
    }
}
