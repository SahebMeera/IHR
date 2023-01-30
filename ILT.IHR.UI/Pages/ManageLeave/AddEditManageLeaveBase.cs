using Blazored.SessionStorage;
using Blazored.Toast.Services;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Pages.ManageLeave
{
    public class AddEditManageLeaveBase: ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        protected ILeaveBalanceService LeaveBalanceService { get; set; }
        public LeaveBalance LeaveBalance = new LeaveBalance();
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        public bool ShowDialog { get; set; }
        private int LeaveBalanceID { get; set; }
        public ILT.IHR.DTO.User user;
        public bool isSaveButtonDisabled { get; set; } = false;



        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
        }
        public void Show(int Id)
        {
            LeaveBalanceID = Id;
            ResetDialog();
            if (LeaveBalanceID != 0)
            {
                GetDetails(LeaveBalanceID);
            }
           
        }
        private async Task GetDetails(int Id)
        {
            Response<LeaveBalance> resp = new Response<LeaveBalance>();
            resp = await LeaveBalanceService.GetLeaveBalanceById(Id);
            LeaveBalance = resp.Data;
            ShowDialog = true;
            StateHasChanged();
        }
        public void onTotalChange()
        {
           LeaveBalance.VacationBalance = LeaveBalance.VacationTotal - LeaveBalance.VacationUsed;
        }
        protected async Task SaveLeaveBalance()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            LeaveBalance.ModifiedBy = user.FirstName + " " + user.LastName;
            var result = await LeaveBalanceService.updateLeaveBalance(LeaveBalanceID, LeaveBalance);
            if (result.MessageType == MessageType.Success)
            {
                toastService.ShowSuccess("Manage Leave Saved successfully", "");
                await UpdateLeaveList.InvokeAsync(true);
                Cancel();
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            isSaveButtonDisabled = false;

        }
        public void Cancel()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        private void ResetDialog()
        {
            LeaveBalance = new LeaveBalance { };
        }

        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
        }

    }
}
