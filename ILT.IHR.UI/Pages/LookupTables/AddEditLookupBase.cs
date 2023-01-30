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

namespace ILT.IHR.UI.Pages.LookupTables
{
    public class AddEditLookupBase : ComponentBase
    {

        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public List<ListType> Lookups { get; set; } //Drop Down Api Data
        private int ListValueId { get; set; }
        [Parameter]
        public EventCallback<bool> ListValueUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        [Parameter]
        public string empID { get; set; }

        protected string Title = "Add";
        public ListValue lookup = new ListValue();
        public bool ShowDialog { get;set; }
        public ILT.IHR.DTO.User user;
        public bool isSaveButtonDisabled { get; set; } = false;



        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }

        
        protected async Task SaveLookup()
        {
           if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (ListValueId == 0)
            {
                lookup.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await LookupService.SaveListValue(lookup);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(lookup);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Lookup saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    ShowDialog = false;
                    StateHasChanged();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (ListValueId != 0)
            {
                lookup.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await LookupService.UpdateLookupValue(ListValueId, lookup);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Lookup saved successfully", "");
                    ListValueUpdated.InvokeAsync(true);
                    ShowDialog = false;
                    StateHasChanged();
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
            var resp = (await LookupService.GetLookups());
            if(resp.MessageType == MessageType.Success)
            {
                Lookups = resp.Data.ToList();
            }

        }
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.ListValue> resp = new Response<ILT.IHR.DTO.ListValue>();
            resp =  await LookupService.GetLookupValueByIdAsync(Id) as Response<ILT.IHR.DTO.ListValue>;
            if (resp.MessageType == MessageType.Success)
            {
                lookup = resp.Data;
            }
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
          
        }
        protected void Cancel()
        {
            ShowDialog = false;
            StateHasChanged();

        }
        public void Show(int Id, int lookId, string lookupType)
        {
            ListValueId = Id;
            ResetDialog();
            if (ListValueId != 0)
            {
                GetDetails(ListValueId);
            }else
            {
                Title = "Add";
                lookup.IsActive = true;
                lookup.ListTypeID = lookId;
                lookup.Type = lookupType;
                isfirstElementFocus = true;
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
            lookup = new ListValue { };
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
    }
}
