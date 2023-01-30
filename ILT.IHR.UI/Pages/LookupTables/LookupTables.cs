using ILT.IHR.UI.Service;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.LookupTables
{
    public class LookupTable : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public int LookupId { get; set; }  //DropdownID
        public List<ListValue> LookupsList { get; set; }  // Table APi Data
        public List<ListType> Lookups { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstLookupType { get; set; } //Drop Down Api Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }

        public AddEditLookupBase AddEditLookupModal { get; set; }
        
        protected ILT.IHR.DTO.ListValue selected;

        public List<RolePermission> RolePermissions;
        public RolePermission LookupRolePermission;
        protected int DefaultTypeID { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            lstLookupType = new List<IDropDownList> ();
            DefaultTypeID = 0;
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            await LoadTableConfig();
            await LoadDropDown();            
        }

        private async Task LoadTableConfig()
        {
            LookupRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.LOOKUP);
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
            /*RowActions = new List<IRowActions> { m1, m2 };*/
            RowActions = new List<IRowActions> { };
            if (LookupRolePermission != null)
            {
                if (LookupRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (LookupRolePermission.Delete == true)
                {
                    RowActions.Add(m2);
                }
            }
            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsDisabled = LookupId != 0 ? false : true
            };
            /*HeaderAction = new List<IHeaderActions> { m3 };*/
            if (LookupRolePermission != null && LookupRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }
            LookupsList = new List<ListValue> { };
        }

        protected void setLookupList()
        {
            lstLookupType.Clear();
            IDropDownList ListItem = new IDropDownList();

            if (Lookups != null)
            {
                lstLookupType = (from lookupItem in Lookups
                                 select new IDropDownList { ID = lookupItem.ListTypeID, Value = lookupItem.TypeDesc }).ToList();

                ListItem.ID = 0;
                ListItem.Value = "Select";
                lstLookupType.Insert(0, ListItem);
            }
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
            if(selected != null)
            {
                AddEditLookupModal.Show(selected.ListValueID, LookupId, "");
            }
        }
        public void Add()
        {
            var lookType = Lookups.Find(x => x.ListTypeID == LookupId).Type;
          //  AddEditLookupModal.lookup.ListType = LookupId
           AddEditLookupModal.Show(0, LookupId, lookType);
        }

        private async Task LoadDropDown()
        {
            var resp = (await LookupService.GetLookups());
            if (resp.MessageType == MessageType.Success)
            {
                if (resp != null && resp.Data != null)
                    Lookups = resp.Data.ToList();
            }

            setLookupList();
        }
        protected async Task LoadLookupList(int TypeID)
        {
            LookupId = TypeID;
            LoadTableConfig();
            if (LookupId != 0)
            {
                var resp = (await LookupService.GetLookupByIdAsync(LookupId));
                if(resp.MessageType == MessageType.Success)
                {
                    LookupsList = resp.Data.ListValues;
                }
            } else
            {
                LookupsList = new List<ListValue> { };
            }
            StateHasChanged();
        }
        protected async Task LoadList()
        {
            LoadTableConfig();
            if (LookupId != 0)
            {
                var resp = (await LookupService.GetLookupByIdAsync(LookupId));
                if (resp.MessageType == MessageType.Success)
                {
                    LookupsList = resp.Data.ListValues;
                }
            }
        }
            public void RowClick(ILT.IHR.DTO.ListValue data)
        {
            selected = data;
            StateHasChanged();
        }

        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

      

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await LookupService.DeleteListValue(selected.ListValueID);
                toastService.ShowSuccess("Lookup Delete successfully", "");
                LoadList();
            }
        }
        public void OnLookupTypeChange(int TypeID)
        {
            DefaultTypeID = TypeID;
            LoadLookupList(TypeID);
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}

