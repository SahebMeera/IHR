using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using ILT.IHR.UI.Pages.Employee.EmployeeNotificationModal;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee
{
    
    public class EmployeesBase: ComponentBase
    {
        protected ITable<DTO.Employee> table;
        protected ITable<DTO.Employee> table1;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        [Inject]
        public DataProvider dataProvider { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        public List<ILT.IHR.DTO.Employee> EmployeesList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Employee> lstEmployees { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; }
        protected ILT.IHR.DTO.Employee selected;

        public List<RolePermission> RolePermissions;
        public bool IsChangeLog { get; set; }
        public int DefaultTypeID { get; set; }
        public int DefaultStatusID { get; set; }
        public int DropDown2DefaultID { get; set; }
        public List<IDropDownList> lstCountry { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstStatus { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstEmployeeType { get; set; } //Drop Down Api Data
        public List<ListValue> EmployMentList { get; set; }
        public List<Country> CountryList { get; set; }

        public EmployeeNoificationBase EmployeeNoification { get; set; }
        protected DTO.User user { get; set; }
        protected async override Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            lstCountry = new List<IDropDownList>();
            lstStatus = new List<IDropDownList>();
            lstEmployeeType = new List<IMultiSelectDropDownList>();
            if (dataProvider.DefaultPageSize != 0)
            {
                DefaultPageSize = dataProvider.DefaultPageSize;
            }
            else
            {
                DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            }

            EmployeesList = new List<ILT.IHR.DTO.Employee> { };
            lstEmployees = EmployeesList;
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            IsChangeLog = false;
            if (dataProvider.country != null)
            {
                country = dataProvider.country;
            }
            if (dataProvider.status != null)
            {
                status = dataProvider.status;
            }
            if (dataProvider.employeeType != null)
            {
                employeeTypeList = dataProvider.employeeType;
                selectEmployeeTypeList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
                setCountryList();
                setStatusList();
            }
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                EmployMentList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.EMPLOYMENTTYPE).ToList();
                if (employeeType == null)
                {
                    setEmployeeTypeList();
                }
                else
                {
                    lstEmployeeType = employeeTypeList;
                }

            }
            await LoadTableConfig();
            await LoadEmployees();
            changeToSort();
            //StateHasChanged();

            if (dataProvider.storage != null)
            {
                DTO.Employee Empdata = (DTO.Employee)dataProvider.storage;
                if (Empdata.EmployeeID != 0)
                {
                   await EmployeeNoification.show(Empdata.EmployeeID);
                }
                dataProvider.storage = null;
            }
        }

        private async Task LoadTableConfig()
        {
            RolePermission EmployeeRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EMPLOYEE);
            IRowActions EmpEdit = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
                //IsShow = EmployeeRolePermission.Update
            };
            IRowActions EmpDelete = new IRowActions
            {
                IconClass = "oi oi-trash",
                ActionMethod = Delete,
                ButtonClass = "btn-danger",
                IsShow = EmployeeRolePermission.Delete
            };

            IRowActions EmpChangeLog = new IRowActions
            {
                IconClass = "oi oi-loop",
                ActionMethod = changeLog,
                ButtonClass = "btn-primary"
            };

            RowActions = new List<IRowActions> { EmpEdit, EmpDelete , EmpChangeLog };
            IHeaderActions EmpAdd = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsShow = EmployeeRolePermission.Add
            };
            HeaderAction = new List<IHeaderActions> { EmpAdd };
            EmployeesList = new List<ILT.IHR.DTO.Employee> { };
            lstEmployees = EmployeesList;
            // StateHasChanged();
        }
        public string country { get; set; }
        public void OnCountryChange(int countryID)
        {
            DefaultTypeID = countryID;
            country = lstCountry.Find(x => x.ID == countryID).Value;
            LoadEmployees();
        }
        public string status { get; set; }
        public void OnStatusChange(int statusID)
        {
            DefaultStatusID = statusID;
            status = lstStatus.Find(x => x.ID == statusID).Value;
            LoadEmployees();
        }
        public string employeeType { get; set; }

        public void onChangeEmployeeType(int employeeTypeId)
        {
            DropDown2DefaultID = employeeTypeId;
            employeeType = lstEmployeeType.Find(x => x.ID == employeeTypeId).Value;
            LoadEmployees();
        }
        public List<IMultiSelectDropDownList> employeeTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedempTypeList { get; set; }
        public void onChangeEmployeeTypeList(List<IMultiSelectDropDownList> empTypeList)
        {
            employeeTypeList = empTypeList;
            selectEmployeeTypeList();
            LoadEmployees();
        }
        public void selectEmployeeTypeList()
        {
            selectedempTypeList = employeeTypeList.FindAll(x => x.IsSelected == true);
            var selectEmpType = employeeTypeList.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectEmpType != null && selectEmpType.Value.ToUpper() == "All".ToUpper())
            {
                //lstEmployeeType.ForEach(x => x.IsSelected = true);
                employeeType = "All";
            }
            else
            {
                employeeType = "NotAll";
            }
        }

        protected async Task LoadEmployees()
        {
            string RoleShort = await sessionStorage.GetItemAsync<string>("RoleShort");
            var respEmployee = (await EmployeeService.GetEmployees());
            if (respEmployee.MessageType == MessageType.Success)
            {
                if (respEmployee.Data != null && (RoleShort.ToUpper() == UserRole.EMP || RoleShort.ToUpper() == UserRole.CONTRACTOR))
                {
                    lstCountry = null;
                    lstEmployeeType = null;
                    lstStatus = null;
                    EmployeesList = respEmployee.Data.Where(x => x.EmployeeID == this.user.EmployeeID).ToList();
                    lstEmployees = EmployeesList;
                    StateHasChanged();
                }
                else
                {
                    EmployeesList = respEmployee.Data.ToList();
                    loadEmployeeList();
                }
            }
            else
            {
                EmployeesList = new List<ILT.IHR.DTO.Employee> { };
                lstEmployees = EmployeesList;
                StateHasChanged();
            }
               
        }
        public void loadEmployeeList()
        {
            if (country != "All" && employeeType != "All" && selectedempTypeList != null && status != "All")
            {
                if (status == "Active")
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == country && (x.TermDate == null || x.TermDate > DateTime.Now) && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
                else
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == country && x.TermDate != null && x.TermDate <= DateTime.Now  && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
                //x.EmploymentTypeID == DropDown2DefaultID).ToList();
                // var fds = table.Columns;
            }
            else if (country == "All" && employeeType != "All" && status != "All")
            {
                //lstEmployees = EmployeesList.Where(x => selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                if (status == "Active")
                {
                    lstEmployees = EmployeesList.Where(x => (x.TermDate == null || x.TermDate > DateTime.Now) && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
                else
                {
                    lstEmployees = EmployeesList.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
            }
            else if (country != "All" && employeeType == "All" && status != "All")
            {
                // lstEmployees = EmployeesList.Where(x => x.Country == country).ToList();
                if (status == "Active")
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == country && (x.TermDate == null || x.TermDate > DateTime.Now)).ToList();
                }
                else
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == country && x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                }
            }
            else if (country != "All" && employeeType != "All" && status == "All")
            {
                lstEmployees = EmployeesList.Where(x => x.Country == country && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
            }
            else if (country == "All" && employeeType == "All" && status != "All")
            {
                // lstEmployees = EmployeesList.Where(x => x.Country == country).ToList();
                if (status == "Active")
                {
                    lstEmployees = EmployeesList.Where(x =>  x.TermDate == null || x.TermDate > DateTime.Now).ToList();
                }
                else
                {
                    lstEmployees = EmployeesList.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                }
            }
            else if (country != "All" && employeeType == "All" && status == "All")
            {
                lstEmployees = EmployeesList.Where(x => x.Country == country).ToList();
            }
            else if (country == "All" && employeeType != "All" && status == "All")
            {
                lstEmployees = EmployeesList.Where(x => selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
            }
            else
            {
                lstEmployees = EmployeesList;
            }
            StateHasChanged();
        }


        public void changeToSort()
        {
            if (dataProvider.table != null)
            {
                table = null;
                var a = lstEmployees;
                lstEmployees = null;
                table = (ITable<DTO.Employee>)dataProvider.table;
                var sortColumn = table.Columns.Find(x => x.SortColumn == true);
                var objIndex = table.Columns.IndexOf(sortColumn);
                if(table !=null && table.Columns != null)
                {
                    table.Columns.ForEach(x =>
                    {
                        if (x != null && x.Title.ToLower() == sortColumn.Title.ToLower())
                        {
                            x.SortColumn = true;
                            x.DefaultSortColumn = true;
                            x.DefaultSortDescending = x.DefaultSortDescending;
                            if (table1 != null)
                            {
                                table1.AddColumn(x);
                            }
                        }
                        else
                        {
                            x.SortColumn = false;
                            x.DefaultSortColumn = false;
                            x.DefaultSortDescending = x.DefaultSortDescending;
                            if(table1 != null)
                            {
                                table1.AddColumn(x);
                            }
                        }
                    }
                    );
                }
                table1.Update();
                lstEmployees = a;
                StateHasChanged();
            }
        }
public void Delete()
        {

        }
   
        public void Edit()
        {
            if (selected != null)
            {
                dataProvider.storage = selected;
                dataProvider.country = country;
                 dataProvider.status = status;
                dataProvider.employeeType = employeeTypeList;
                dataProvider.DefaultPageSize = DefaultPageSize;
                dataProvider.table = table;
                NavigationManager.NavigateTo($"/employeeDetails");
            }
        }
        public void EditMobile(ILT.IHR.DTO.Employee data)
        {
            selected = data;
            Edit();
        }
        public void Add()
        {
            dataProvider.storage = new DTO.Employee { };
            dataProvider.country = country;
            dataProvider.status = status;
            dataProvider.employeeType = employeeTypeList;
            dataProvider.DefaultPageSize = DefaultPageSize;
            NavigationManager.NavigateTo("/AddEmployee");
        }
        public void RowClick(ILT.IHR.DTO.Employee data)
        {
            selected = data;
            //StateHasChanged();
        }

        public void changeLog()
        {
            EmployeeNoification.show(selected.EmployeeID);
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
        protected void setCountryList()
        {
            lstCountry.Clear();
            IDropDownList ListItem = new IDropDownList();
            lstCountry = (from country in CountryList
                          select new IDropDownList { ID = country.CountryID ,Value = country.CountryDesc }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            if(country == null)
            {
                country = "United States";
            }
            lstCountry.Insert(0, ListItem);
            DefaultTypeID = lstCountry.Find(x => x.Value.ToLower() == country.ToLower()).ID;
        }

        protected void setStatusList()
        {
            lstStatus.Clear();
            IDropDownList ListItem = new IDropDownList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstStatus.Insert(0, ListItem);
            IDropDownList ListItem1 = new IDropDownList();
            ListItem1.ID = 1;
            ListItem1.Value = "Active";
            lstStatus.Insert(1, ListItem1);
            IDropDownList ListItem2 = new IDropDownList();
            ListItem2.ID = 2;
            ListItem2.Value = "Termed";
            lstStatus.Insert(2, ListItem2);
            if (status == null)
            {
                status = "Active";
            }
            DefaultStatusID = lstStatus.Find(x => x.Value.ToLower() == status.ToLower()).ID;
        }
        protected void setEmployeeTypeList()
        {
            lstEmployeeType.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstEmployeeType = (from employee in EmployMentList
                               select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (employeeType == null)
            {
                employeeType = "All";
            }
            lstEmployeeType.Insert(0, ListItem);
            lstEmployeeType.ForEach(x =>{
                if (x.Value.ToLower() == employeeType.ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedempTypeList = lstEmployeeType.FindAll(x => x.IsSelected == true);
            DropDown2DefaultID = lstEmployeeType.Find(x => x.Value.ToLower() == employeeType.ToLower()).ID;
        }
        protected void UpdatePageSize(int pageSize)
        {
            dataProvider.DefaultPageSize = pageSize;
            DefaultPageSize = pageSize;
        }


        protected void OnDrpDwnChange(ChangeEventArgs e)
        {
            DefaultTypeID = Convert.ToInt32(e.Value);
            OnCountryChange(DefaultTypeID);
            // onDropDownChange.Invoke(DefaultID);
        }
        protected void OnDrpDwn3Change(ChangeEventArgs e)
        {
            DefaultStatusID = Convert.ToInt32(e.Value);
            OnStatusChange(DefaultStatusID);
        }

        public void OnMultiDropDownChange(ChangeEventArgs e, int key)
        {
            var index = lstEmployeeType.FindIndex(x => x.ID == key);
            var userRole = lstEmployeeType.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstEmployeeType.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstEmployeeType.FindIndex(x => x.ID == 0);
                if (lstEmployeeType[isAllSelectOrNot].IsSelected == true)
                {
                    lstEmployeeType[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstEmployeeType[index] = userRole;
            onChangeEmployeeTypeList(lstEmployeeType);
        }
        protected string getSelectedRoles()
        {
            string roles = "";
            if (lstEmployeeType != null)
            {
                if (lstEmployeeType.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstEmployeeType)
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
