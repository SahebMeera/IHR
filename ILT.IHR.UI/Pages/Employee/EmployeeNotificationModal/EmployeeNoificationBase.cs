using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using System.Linq;
using BlazorTable;

namespace ILT.IHR.UI.Pages.Employee.EmployeeNotificationModal
{
    public class EmployeeNoificationBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IEmployeeChangesetService EmployeeChangesetService { get; set; } //Service
        [Inject]
        public INotificationService NotificationService { get; set; } //Service
        public List<EmployeeChangeSet> employeeChangeSet { get; set; }
        private int ListValueId { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Parameter]
        public int empID { get; set; }
        [Parameter]
        public EventCallback<bool> EmployeeUpdated { get; set; }
        protected string Title = "Detail";
        public bool ShowDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //IHeaderActions m3 = new IHeaderActions
            //{
            //    IconClass = " ",
            //    ActionMethod = Acknowledge,
            //    ActionText = "Acknowledge"
            //};
            //HeaderAction = new List<IHeaderActions> { m3 };
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        public void Acknowledge()
        {

            Notification notification = new Notification();
            notification.TableName = "Employee";
            notification.RecordID = this.empID;
            NotificationService.SaveNotification(0, notification);
            ShowDialog = false;
            this.empID = 0;
            StateHasChanged();
            EmployeeUpdated.InvokeAsync(true);
        }

        public async Task show(int employeeid)
        {
            ShowDialog = true;
            this.empID = employeeid;
            Response<IEnumerable<EmployeeChangeSet>> result = await EmployeeChangesetService.GetEmployeeChangeset(employeeid);
            if(result.MessageType == MessageType.Success)
            employeeChangeSet = result.Data.ToList();
            StateHasChanged();
        }

    }
}
