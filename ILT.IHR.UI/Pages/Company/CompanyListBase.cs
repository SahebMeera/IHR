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
using Microsoft.AspNetCore.Hosting;
using ILT.IHR.UI.Shared;

namespace ILT.IHR.UI.Pages.Company
{
    public class CompanyListBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get;set; }

        [Inject]
        public IWebHostEnvironment env { get; set; }

        public int DefaultPageSize { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.EndClient> EndClientList { get; set; }  // Table APi Data
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions


        public AddEditCompanyBase AddEditCompanyModal { get; set; }
        public AddEditEndClientBase AddEditEndClientModal { get; set; }

        protected ILT.IHR.DTO.Company selected;
        protected ILT.IHR.DTO.EndClient selectedEndClient;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission CompanyRolePermission;
        protected List<IDropDownList> lstType { get; set; }
        public List<IDropDownList> lstTypes { get; set; } //Grid Drop Down Data
        public int TypeId { get; set; }  //DropdownID

        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            TypeId = 1;
            CompanyList = new List<ILT.IHR.DTO.Company> { };
            EndClientList = new List<ILT.IHR.DTO.EndClient> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            IsChangeLog = false;
            await LoadList();
            await LoadEndClientList();
            await LoadTableChange();
            await LoadDropDown();
            await LoadTypeList(1);
        }

        protected void SetTypesList()
        {
            lstTypes = new List<IDropDownList> { }; 
            IDropDownList ListItem1 = new IDropDownList();
            ListItem1.ID = 1;
            ListItem1.Value = "Company";
            lstTypes.Insert(0, ListItem1);

            IDropDownList ListItem2 = new IDropDownList();
            ListItem2.ID = 0;
            ListItem2.Value = "End Client";
            lstTypes.Insert(1, ListItem2);
        }

        protected void onChangeType(int ID)
        {
            LoadTypeList(ID);
        }

        private async Task LoadDropDown()
        {            
            SetTypesList();
        }

        protected async Task LoadTypeList(int ID)
        {
            TypeId = ID;
            await LoadTableConfig();
            if (TypeId == 0)
            {
                var reponses = (await EndClientService.GetEndClients());
                if (reponses.MessageType == MessageType.Success)
                {
                    EndClientList = reponses.Data;
                    CompanyList = null;
                    StateHasChanged();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }                
            }
            else
            {
                var reponses = (await CompanyService.GetCompanies());
                
                if (reponses.MessageType == MessageType.Success)
                {
                    CompanyList = reponses.Data;
                    EndClientList = null;
                    StateHasChanged();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            StateHasChanged();

        }

        public IEnumerable<IHeaderActions> ChangeHeaderActions { get; set; } //Header Actions
        private async Task LoadTableConfig()
        {
            CompanyRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.COMPANY);
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
            IRowActions m4 = new IRowActions
            {
                IconClass = "oi oi-loop",
                ActionMethod = changeLog,
                ButtonClass = "btn-primary"
            };

            RowActions = new List<IRowActions> { };
            if (CompanyRolePermission != null)
            {
                if (CompanyRolePermission.View == true)
                {
                    RowActions.Add(m1);
                }
                if (CompanyRolePermission.Delete == true)
                {
                    RowActions.Add(m2);
                }
                // RowActions.Add(m4);
            }
            /*RowActions = new List<IRowActions> { m1, m2, m4 };*/            
            IHeaderActions m5 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-height",
                ActionMethod = AddEndClient,
                ActionText = "END CLIENT"
            };
            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-height",
                ActionMethod = Add,
                ActionText = "COMPANY"
            };
            IHeaderActions c1 = new IHeaderActions
            {
                IconClass = "oi oi-check",
                ActionMethod = Approve,
                ActionText = "Approve"
            };
            IHeaderActions c2 = new IHeaderActions
            {
                IconClass = "oi oi-circle-x",
                ActionMethod = Cancel,
                ActionText = "Cancel",
                ButtonClass = "btn-danger"
            };
            if (CompanyRolePermission != null && CompanyRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m5, m3 };
            }
            /*HeaderAction = new List<IHeaderActions> { m3 };*/
            ChangeHeaderActions = new List<IHeaderActions> { c1, c2 };
            CompanyList = new List<ILT.IHR.DTO.Company> { };
            EndClientList = new List<ILT.IHR.DTO.EndClient> { };
        }
        protected async Task LoadList()
        {
            
            await LoadTableConfig();
            var reponses = (await CompanyService.GetCompanies());
            if (reponses.MessageType == MessageType.Success)
            {
                CompanyList = reponses.Data;
                EndClientList = null;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

        }

        protected async Task LoadEndClientList()
        {

            await LoadTableConfig();
            var reponses = (await EndClientService.GetEndClients());
            if (reponses.MessageType == MessageType.Success)
            {
                EndClientList = reponses.Data;
                CompanyList = null;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }

        }

        public void Delete()
        {
            if (TypeId == 0)
            {
                if (selectedEndClient != null)
                {
                    DeleteConfirmation.Show();
                }
            }
            else 
            {
                if (selected != null)
                {
                    DeleteConfirmation.Show();
                }
            }
            
        }

        public void Edit()
        {
            if (TypeId == 0)
            {
                if (selectedEndClient != null)
                {
                    AddEditEndClientModal.Show(selectedEndClient.EndClientID);

                }
            }
            else
            {
                if (selected != null)
                {
                    AddEditCompanyModal.Show(selected.CompanyID);

                }
            }            
        }
        public void Add()
        {
            AddEditCompanyModal.Show(0);
        }
        public void AddEndClient()
        {
            AddEditEndClientModal.Show(0);
        }
        public void RowClick(ILT.IHR.DTO.Company data)
        {
            selected = data;
            StateHasChanged();
        }
        public void RowClickEndClient(ILT.IHR.DTO.EndClient data)
        {
            selectedEndClient = data;
            StateHasChanged();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                // await LookupService.DeleteListValue(selected.ListValueID);
                // await LoadList();
                // toastService.ShowSuccess("Lookup Delete successfully", "");

            }
        }
        public bool IsChangeLog { get; set; }
        public void changeLog()
        {
            IsChangeLog = true;
        }
        public partial class ChangeLog : AbstractDataObject
        {
            public string FieldName { get; set; }
            public string NewValue { get; set; }
            public string OldValue { get; set; }
        }
        public List<ChangeLog> ChangeList { get; set; } //Drop Down Api Data

        private async Task LoadTableChange()
        {
            await Task.Delay(500);
            ChangeLog m1 = new ChangeLog
            {
                FieldName = "Name",
                NewValue = "ILT",
                OldValue = "Info",
            };
            ChangeLog m2 = new ChangeLog
            {
                FieldName = "PaymentTerm",
                NewValue = "Payment1",
                OldValue = "Payment2",
            };
            ChangeLog m3 = new ChangeLog
            {
                FieldName = "CompanyType",
                NewValue = "Client",
                OldValue = "Vender",
            };
            ChangeLog m4 = new ChangeLog
            {
                FieldName = "City",
                NewValue = "Hyderabad",
                OldValue = "Vijayawada",
            };
            ChangeList = new List<ChangeLog> { m1, m2, m3, m4 };
        }
        public void Cancel()
        {
            IsChangeLog = false;
        }
        public void Approve()
        {

        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
