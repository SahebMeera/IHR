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

namespace ILT.IHR.UI.Pages.Employee.EmployeeDependent
{
    public class AddEditDependentBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service  
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IDependentService DependentService { get; set; } //Service
        public List<ListValue> VisaTypeList { get; set; }  // Table APi Data
        public List<ListValue> RelationList { get; set; }  // Table APi Data
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }

        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        private int DependentId { get; set; }
        protected bool isShow { get; set; }
        public Dependent Dependent = new Dependent();
        public ILT.IHR.DTO.User user;
        public RolePermission EmployeeInfoRolePermission { get; set; }
        public List<RolePermission> RolePermissions;
        public bool isSaveButtonDisabled { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            EmployeeInfoRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEEINFO);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
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
        public void Show(int Id, int employeeId)
        {
            DependentId = Id;
            Dependent.EmployeeID = employeeId;
            ResetDialog();
            if (DependentId != 0)
            {
                isShow = EmployeeInfoRolePermission.Update;
                GetDetails(DependentId);
            }
            else
            {
                isShow = EmployeeInfoRolePermission.Add;
                Title = "Add";
                Dependent.BirthDate = DateTime.Now;
                Dependent.EmployeeID = employeeId;
                loadEmployeeData(Dependent.EmployeeID);
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
                VisaTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VISATYPE).ToList();
                RelationList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.RELATION).ToList();

            }
        }

        protected async Task loadEmployeeData(int employeeId)
        {
            if (employeeId != 0)
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await EmployeeService.GetEmployeeByIdAsync(employeeId) as Response<ILT.IHR.DTO.Employee>;
                if (resp.MessageType == MessageType.Success)
                {
                        if(RelationList.Count != 0)
                        {
                            if (resp.Data.Country.ToUpper() == Countries.UNITEDSTATES)
                            {
                               var Data =  RelationList.Where(x => x.ValueDesc.ToUpper() != "Mother".ToUpper() && x.ValueDesc.ToUpper() != "Father".ToUpper());
                                RelationList = new List<ListValue>();
                                RelationList = Data.ToList();
                            }
                        }
                }
                StateHasChanged();
            }
        }

        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.Dependent> resp = new Response<ILT.IHR.DTO.Dependent>();
            resp = await DependentService.GetDependentByIdAsync(Id);   
            if(resp.MessageType == MessageType.Success)
            Dependent = resp.Data;
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }
        protected async Task SaveDependent()
        {
            Dependent.EmployeeName = EmployeeName;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (DependentId == 0)
            {
                Dependent.CreatedBy = user.FirstName + " " + user.LastName;
                //   var abc = Newtonsoft.Json.JsonConvert.SerializeObject(Dependent);
                var result = await DependentService.SaveDependent(Dependent);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Dependent saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (DependentId != 0)
            {
                Dependent.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await DependentService.UpdateDependent(DependentId, Dependent);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Dependent saved successfully", "");
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
            Dependent = new Dependent { };
        }

        protected async Task onRelationChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && RelationList != null)
            {
                var relation = Convert.ToInt32(e.Value);
                Dependent.Relation = RelationList.Find(x => x.ListValueID == relation).ValueDesc;
            }
        }

        protected async Task onVisaTypeChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && VisaTypeList != null)
            {
                var visaType = Convert.ToInt32(e.Value);
                Dependent.VisaType = VisaTypeList.Find(x => x.ListValueID == visaType).ValueDesc;
            }
        }
    }
}
