using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using System.Linq;
using BlazorTable;

namespace ILT.IHR.UI.Pages.Employee.FormI9.FormI9ChangesSet
{
    public class FormI9ChangeSetBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IFormI9ChangesetService FormI9ChangesetService { get; set; } //Service
        [Inject]
        public INotificationService NotificationService { get; set; } //Service
        public List<DTO.FormI9ChangeSet> formI9ChangeSet { get; set; }
        private int ListValueId { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Parameter]
        public int formi9ID { get; set; }
        [Parameter]
        public EventCallback<bool> FormI9Updated { get; set; }
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

            //Notification notification = new Notification();
            //notification.TableName = "Asset";
            //notification.RecordID = this.empID;
            //NotificationService.SaveNotification(0, notification);
            //ShowDialog = false;
            //this.empID = 0;
            //StateHasChanged();
            //AssetUpdated.InvokeAsync(true);
        }

        public async Task show(int formi9id)
        {
            ShowDialog = true;
            Response<IEnumerable<DTO.FormI9ChangeSet>> result = await FormI9ChangesetService.GetFormI9Changeset(formi9id);
            if(result.MessageType == MessageType.Success)
            formI9ChangeSet = result.Data.ToList();
            StateHasChanged();
        }

    }
}
