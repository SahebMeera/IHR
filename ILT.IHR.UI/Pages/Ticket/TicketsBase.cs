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
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Ticket
{
    public class TicketsBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize {get; set;}
       
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service        
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ITicketService TicketService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public DataProvider dataProvider { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public List<ListValue> TicketStatusList { get; set; }  // Table APi Data

        public List<ILT.IHR.DTO.Ticket> TicketsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Ticket> lstTicketsList { get; set; }  // Table APi Data
        public List<IMultiSelectDropDownList> lstTicketAssignedTO { get; set; } //Drop Down Api Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

       public AddEditTicketBase AddEditTicketModal { get; set; }
        protected ILT.IHR.DTO.Ticket selected;
        private int EmployeeID { get; set; }

        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission TicketRolePermission { get; set; }
        public List<IMultiSelectDropDownList> lstTicketStatus { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketAssignedTo { get; set; } //Drop Down Api Data
        public IEnumerable<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data  
        public IEnumerable<ILT.IHR.DTO.Employee> lstAssignedList { get; set; } //Drop Down Api Data  
        public string TicketStatus { get; set; }
        public string TicketAssignedTo { get; set; }
        public List<IMultiSelectDropDownList> employeeTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedempTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedAssignedToList { get; set; }
        public string currentLoginUserRole { get; set; }
        public string UserName { get; set; }
        protected DTO.User user { get; set; }
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            TicketsList = new List<ILT.IHR.DTO.Ticket> { };
            lstTicketAssignedTo = new List<IMultiSelectDropDownList>();
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            UserName = user.FirstName + " " + user.LastName;
            currentLoginUserRole = await sessionStorage.GetItemAsync<string>("RoleName");
            lstTicketStatus = new List<IMultiSelectDropDownList>();
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            //loadYearDropdown();
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                TicketStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TICKETSTATUS).ToList();
                if (TicketStatus == null)
                {
                    setEmployeeTypeList();
                }
                else
                {
                    lstTicketStatus = employeeTypeList;
                }
            }
            var respEmployees = (await EmployeeService.GetEmployees());
            if (respEmployees.MessageType == MessageType.Success)
            {
               // Employees = respEmployees.Data;
                Response<IEnumerable<DTO.Ticket>> reponses = new Response<IEnumerable<ILT.IHR.DTO.Ticket>> { };
                if (currentLoginUserRole.ToUpper() == UserRole.ADMIN)
                {
                    reponses = (await TicketService.GetTickets());
                }
                else
                {
                    reponses = (await TicketService.GetTicketsList(EmployeeID, EmployeeID));
                }

                if (reponses != null && reponses.MessageType == MessageType.Success)
                {
                    if (reponses.Data != null && reponses.Data.ToList().Count > 0 && respEmployees.Data != null && respEmployees.Data.ToList().Count > 0)
                    {
                        var TicketReponses = reponses.Data.ToList();
                        if (currentLoginUserRole.ToUpper() == UserRole.ADMIN)
                        {
                            Employees = respEmployees.Data.Where(x => TicketReponses.Any(s => s.AssignedToID == x.EmployeeID)).ToList();
                        }
                        else
                        {
                            Employees = respEmployees.Data.Where(x => user.EmployeeID == x.EmployeeID).ToList();
                        }
                        if (Employees != null)
                        {
                            loadAssignedTo();
                        }
                    } else
                    {
                        Employees = respEmployees.Data;
                    }
                }
            }
            await LoadList();
        }
        protected void loadAssignedTo()
        {
            lstTicketAssignedTo.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstTicketAssignedTo = (from employee in Employees
                               select new IMultiSelectDropDownList { ID = employee.EmployeeID, Value = employee.TermDate == null || DateTime.Now <= employee.TermDate ? employee.EmployeeName : employee.EmployeeName + " *", IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (TicketAssignedTo == null)
            {
                TicketAssignedTo = UserName;
            }
            lstTicketAssignedTo.Insert(0, ListItem);
            lstTicketAssignedTo.ForEach(x => {
                if (x.ID == user.EmployeeID)
                {
                    x.IsSelected = true;
                }
              
            });
            selectedAssignedToList = lstTicketAssignedTo.FindAll(x => x.IsSelected == true);
            DropDown2DefaultID = lstTicketAssignedTo.Find(x => x.Value.ToLower() == TicketAssignedTo.ToLower()).ID;
        }
        public List<IMultiSelectDropDownList> ListAssetStatus { get; set; }

        public void onChangeAssignedToList(List<IMultiSelectDropDownList> assetTypesList)
        {
            ListAssetStatus = assetTypesList;
            selectAssignedToList();
            LoadList();
        }
        public void selectAssignedToList()
        {
            selectedAssignedToList = ListAssetStatus.FindAll(x => x.IsSelected == true);
            var selectAssetType = ListAssetStatus.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                TicketAssignedTo = "All";
            }
            else
            {
                TicketAssignedTo = "NotAll";
            }
        }



        private async Task LoadTableConfig()
        {
            TicketRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.TICKET);
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
            if (TicketRolePermission != null)
            {
                if (TicketRolePermission.Update == true)
                {
                    RowActions.Add(m1);
                }
                if (TicketRolePermission.Delete == true)
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

            if (TicketRolePermission != null && TicketRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }

            TicketsList = new List<ILT.IHR.DTO.Ticket> { };
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
               AddEditTicketModal.Show(selected.TicketID, EmployeeID, 0);
            }
        }
        public void Add()
        {
           AddEditTicketModal.Show(0, EmployeeID, 0);
        }

        protected async Task LoadList()
        {
            await LoadTableConfig();
            Response<IEnumerable<DTO.Ticket>> reponses  = new Response<IEnumerable<ILT.IHR.DTO.Ticket>> { };
            if (currentLoginUserRole.ToUpper() == UserRole.ADMIN)
            {
                reponses = (await TicketService.GetTickets());
            }else
            {
               reponses = (await TicketService.GetTicketsList(EmployeeID, EmployeeID));
            }

            if (reponses.MessageType == MessageType.Success)
            {
                TicketsList = reponses.Data.ToList();
                if(TicketsList != null)
                {
                    loadTicketList();
                } else
                {
                    lstTicketsList = TicketsList;
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);

            }
        }
        public void loadTicketList()
        {
            if (TicketStatus != "All" && selectedempTypeList != null && TicketAssignedTo != "All" && selectedAssignedToList != null)
            {
                if (currentLoginUserRole.ToUpper() == UserRole.ADMIN || currentLoginUserRole.ToUpper() == UserRole.FINADMIN || currentLoginUserRole.ToUpper() == UserRole.ITADMIN || currentLoginUserRole.ToUpper() == UserRole.OPSADMIN)
                {
                    lstTicketsList = TicketsList.Where(x => selectedempTypeList.Any(s => s.ID == x.StatusID) && selectedAssignedToList.Any(t =>  t.ID == x.AssignedToID || x.AssignedToID == null)).ToList();
                }
                else
                {
                    lstTicketsList = TicketsList.Where(x => selectedempTypeList.Any(s => s.ID == x.StatusID) && selectedAssignedToList.Any(t => t.ID == x.AssignedToID)).ToList();
                }
            }
            else if (TicketStatus == "All" && TicketAssignedTo != "All")
            {
                if (currentLoginUserRole.ToUpper() == UserRole.ADMIN || currentLoginUserRole.ToUpper() == UserRole.FINADMIN || currentLoginUserRole.ToUpper() == UserRole.ITADMIN || currentLoginUserRole.ToUpper() == UserRole.OPSADMIN)
                {
                    lstTicketsList = TicketsList.Where(x => selectedAssignedToList.Any(t => t.ID == x.AssignedToID || x.AssignedToID == null)).ToList();
                }
                else
                {
                    lstTicketsList = TicketsList.Where(x => selectedAssignedToList.Any(t => t.ID == x.AssignedToID)).ToList();
                }
            }
            else if (TicketStatus != "All" && TicketAssignedTo == "All")
            {
                lstTicketsList = TicketsList.Where(x => selectedempTypeList.Any(s => s.ID == x.StatusID)).ToList();
            }
            else
            {
                lstTicketsList = TicketsList;
            }
            StateHasChanged();
        }


        public void RowClick(ILT.IHR.DTO.Ticket data)
        {
            selected = data;
            StateHasChanged();
        }

        public void EditMobile(ILT.IHR.DTO.Ticket data)
        {
            selected = data;
            Edit();
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await TicketService.DeleteTicket(selected.TicketID); 
                var reponses = (await TicketService.GetTickets());
                if (reponses.MessageType == MessageType.Success)
                {
                    TicketsList = reponses.Data.ToList();
                    lstTicketsList = TicketsList;
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
                toastService.ShowSuccess("Ticket Deleted successfully", "");
            }
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }

        public void onChangeEmployeeTypeList(List<IMultiSelectDropDownList> empTypeList)
        {
            employeeTypeList = empTypeList;
            selectEmployeeTypeList();
            LoadList();
        }
        public void selectEmployeeTypeList()
        {
            selectedempTypeList = employeeTypeList.FindAll(x => x.IsSelected == true);
            var selectEmpType = employeeTypeList.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectEmpType != null && selectEmpType.Value.ToUpper() == "All".ToUpper())
            {
                //lstEmployeeType.ForEach(x => x.IsSelected = true);
                TicketStatus = "All";
            }
            else
            {
                TicketStatus = "NotAll";
            }
        }
        public int DropDown2DefaultID { get; set; }
        protected void setEmployeeTypeList()
        {
            lstTicketStatus.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstTicketStatus = (from employee in TicketStatusList
                               select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (TicketStatus == null)
            {
                TicketStatus = "New";
            }
            lstTicketStatus.Insert(0, ListItem);
            lstTicketStatus.ForEach(x => {
                if (x.Value.ToLower() == TicketStatus.ToLower())
                {
                    x.IsSelected = true;
                }
                if (x.Value.ToLower() == "Assigned".ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedempTypeList = lstTicketStatus.FindAll(x => x.IsSelected == true);
            DropDown2DefaultID = lstTicketStatus.Find(x => x.Value.ToLower() == TicketStatus.ToLower()).ID;
        }

        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }

        public void OnMultiDropDownChange(ChangeEventArgs e, int key)
        {
            var index = lstTicketStatus.FindIndex(x => x.ID == key);
            var userRole = lstTicketStatus.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstTicketStatus.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstTicketStatus.FindIndex(x => x.ID == 0);
                if (lstTicketStatus[isAllSelectOrNot].IsSelected == true)
                {
                    lstTicketStatus[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstTicketStatus[index] = userRole;
            onChangeEmployeeTypeList(lstTicketStatus);
        }
        protected string getSelectedRoles()
        {
            string roles = "";
            if (lstTicketStatus != null)
            {
                if (lstTicketStatus.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstTicketStatus)
                    {

                        if (string.IsNullOrEmpty(roles) && item.IsSelected)
                        {
                            roles = item.Value;

                        }
                        else if (item.IsSelected)
                        {
                            roles = roles + ", " + item.Value;
                        }
                    }
                }
            }
            return roles;
        }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public async Task SearchFuntion()
        {
            await JSRuntime.InvokeAsync<string>("SearchFunction");
        }
    }
}
