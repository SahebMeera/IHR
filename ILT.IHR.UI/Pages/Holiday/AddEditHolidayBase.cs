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


namespace ILT.IHR.UI.Pages.Holiday
{
    public class AddEditHolidayBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public IHolidayService HolidayService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.Holiday> Holidays { get; set; } //Drop Down Api Data       
        private int HolidayId { get; set; }
        [Parameter]
        public EventCallback<bool> HolidayUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        protected string Title = "Add";
        public ILT.IHR.DTO.Holiday holiday = new ILT.IHR.DTO.Holiday();
        public bool ShowDialog { get; set; }
        public List<Country> CountryList { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;
        public ILT.IHR.DTO.User user;

      

        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }

        protected async Task SaveHoliday()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (HolidayId == 0)
                {
                holiday.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await HolidayService.SaveHoliday(holiday);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Holiday saved successfully", "");
                        HolidayUpdated.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                else if (HolidayId > 0)
                {
                    holiday.ModifiedBy = user.FirstName + " " + user.LastName;
                    var result = await HolidayService.UpdateHoliday(HolidayId, holiday);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Holiday saved successfully", "");
                        HolidayUpdated.InvokeAsync(true);
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
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
            var reponses = (await HolidayService.GetHolidays());
            if (reponses.MessageType == MessageType.Success)
            {
                Holidays = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }           
        }
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.Holiday> resp = new Response<ILT.IHR.DTO.Holiday>();
            resp = await HolidayService.GetHolidayByIdAsync(Id) as Response<ILT.IHR.DTO.Holiday>;
            if(resp.MessageType == MessageType.Success)
            {
                holiday = resp.Data;
            }
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            HolidayId = -1;
            ShowDialog = false;
            // UserUpdated.InvokeAsync(true);
            StateHasChanged();

        }
        public async void Show(int Id)
        {
            HolidayId = Id;
            ResetDialog();
            if (HolidayId != 0)
            {               
                GetDetails(HolidayId);
            }
            else
            {
                Title = "Add";
                holiday.StartDate = DateTime.Now;
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
            holiday = new ILT.IHR.DTO.Holiday { };
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
