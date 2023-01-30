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

namespace ILT.IHR.UI.Pages.Holiday
{
    public class HolidaysBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize {get; set;}
       
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service        
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IHolidayService HolidayService { get; set; } //Service

        public IEnumerable<ILT.IHR.DTO.Holiday> HolidaysList { get; set; }  // Table APi Data
        public IEnumerable<ILT.IHR.DTO.Holiday> lstHolidaysList { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

        public AddEditHolidayBase AddEditHolidayModal { get; set; }
        public List<HolidayYear> LeaveYearList { get; set; }
        protected List<IDropDownList> lstYear { get; set; }
        protected int yearId { get; set; }

        protected ILT.IHR.DTO.Holiday selected;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission HolidayRolePermission;

        
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            HolidaysList = new List<ILT.IHR.DTO.Holiday> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            loadYearDropdown();
            yearId = Convert.ToInt32(LeaveYearList.Find(x => x.text.ToLower() == "Current Year".ToLower()).year);
            await LoadList();
        }

        private async Task LoadTableConfig()
        {
            HolidayRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.HOLIDAY);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger"
            };

            RowActions = new List<IRowActions> { };
            if (HolidayRolePermission != null)
            {
                if (HolidayRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (HolidayRolePermission.Delete == true)
                {
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

            if (HolidayRolePermission != null && HolidayRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }

            HolidaysList = new List<ILT.IHR.DTO.Holiday> { };
        }

        public void Delete()
        {
            if (selected != null)
            {
                DeleteConfirmation.Show();
            }
        }

        public void Edit()
        {
            if (selected != null)
            {
                AddEditHolidayModal.Show(selected.HolidayID);
            }
        }
        public void Add()
        {
            AddEditHolidayModal.Show(0);
        }

        protected async Task LoadList()
        {
            await LoadTableConfig();
            var reponses = (await HolidayService.GetHolidays());
          
            if (reponses.MessageType == MessageType.Success)
            {
                HolidaysList = reponses.Data;
                lstHolidaysList = HolidaysList;
                if (yearId != 0)
                {
                    onYearChange(yearId);
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);

            }
        }

        public void RowClick(ILT.IHR.DTO.Holiday data)
        {
            selected = data;
            StateHasChanged();
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await HolidayService.DeleteHoliday(selected.HolidayID); 
                var reponses = (await HolidayService.GetHolidays());
                if (reponses.MessageType == MessageType.Success)
                {
                    HolidaysList = reponses.Data;
                    lstHolidaysList = HolidaysList;
                    if (yearId != 0)
                    {
                        onYearChange(yearId);
                    }
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
                toastService.ShowSuccess("Holiday Deleted successfully", "");
            }
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dddd, MMMM dd, yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
           public class HolidayYear
        {
            public string year { get; set; }
            public string text { get; set; }
        }
     
        protected void onYearChange(int Year)
        {
            yearId = Year;
            if (Year != 0)
                lstHolidaysList = HolidaysList.Where(x => x.StartDate.Year == Year).ToList(); 
            else
                lstHolidaysList = HolidaysList;
            StateHasChanged();
        }

        public void loadYearDropdown()
        {
            HolidayYear m1 = new HolidayYear
            {
                year = DateTime.Now.Year.ToString(),
                text = "Current Year"
            };
            HolidayYear m2 = new HolidayYear
            {
                year = (DateTime.Now.Year - 1).ToString(),
                text = "Previous Year"
            };
            LeaveYearList = new List<HolidayYear> { m1, m2 };
            SetYearList();
          lstHolidaysList = HolidaysList.Where(x => x.StartDate.Year.ToString() == DateTime.Now.Year.ToString()).ToList();
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
    }
}
