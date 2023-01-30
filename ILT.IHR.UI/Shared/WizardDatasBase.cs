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
using System.Xml;

namespace ILT.IHR.UI.Shared
{
    public class WizardDatasBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IWizardService WizardService { get; set; } //Service
        public IEnumerable<ProcessWizard> WizardList { get; set; }  // Table APi Data
        [Inject]
        public IWizardDataService WizardDataService { get; set; } //Service
        public IEnumerable<ProcessData> WizardDataList { get; set; }  // Table APi Data
        public IEnumerable<ProcessData> lstWizardDataList { get; set; }  // Table APi Data

        public List<IRowActions> RowActions { get; set; } //Row Actions

        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions

        public WizardFlowData WizardFlowDataModal { get; set; }
        public WizardFlowBase WizardFlowBaseModal { get; set; }

        protected ProcessData selected;
        protected List<IDropDownList> lstProcess { get; set; }
        public List<IDropDownList> lstProcesses { get; set; } //Grid Drop Down Data
        protected int wizardId { get; set; }
        public string wizardName { get; set; }
        public int DefaultTypeID { get; set; }

        public List<RolePermission> RolePermissions;
        [Inject]
        public IConfiguration Configuration { get; set; }
        public int DefaultPageSize { get; set; }
        public List<Element> Elements { get; set; }
        public ITable<ProcessData> Table;
        [Inject]
        public ILookupService LookupService { get; set; }
        [Inject]
        public DataProvider dataProvider { get; set; } //Service
        public List<IMultiSelectDropDownList> lstMultiStatus { get; set; }
        public string statusFilter;
        public int DropDown2DefaultID { get; set; }
        public List<IMultiSelectDropDownList> selectedStatusList { get; set; }

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
        protected async override Task OnInitializedAsync()
        {
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            lstMultiStatus = new List<IMultiSelectDropDownList>();
            List<ListValue> listValuesStatus = new List<ListValue>();
            lstProcesses = new List<IDropDownList>();
            WizardDataList = new List<ILT.IHR.DTO.ProcessData> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            Response<IEnumerable<ProcessWizard>> response = (await WizardService.GetWizards());
            if (response.MessageType == MessageType.Success)
            {
                WizardList = response.Data.ToList();
                setWizardList();
            }
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                listValuesStatus = resp.Data.Where(x => x.Type.ToUpper() == Constants.WIZARDSTATUS).ToList();
                setStatusList(listValuesStatus);
            }
            await LoadProcessList(DefaultTypeID);
        }

        public void onChangeWizardStatusList(List<IMultiSelectDropDownList> statusList)
        {
            lstMultiStatus = statusList;
            selectStatusList();
            LoadProcessList(DefaultTypeID);
        }
        public void selectStatusList()
        {
            selectedStatusList = lstMultiStatus.FindAll(x => x.IsSelected == true);
            var selectEmpType = lstMultiStatus.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectEmpType != null && selectEmpType.Value.ToUpper() == "All".ToUpper())
            {
                //lstEmployeeType.ForEach(x => x.IsSelected = true);
                statusFilter = "All";
            }
            else
            {
                statusFilter = "NotAll";
            }
        }

        protected void setStatusList(List<ListValue> lstStatus) // Multiselect
        {
            lstMultiStatus.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();

            lstMultiStatus = (from status in lstStatus
                              select new IMultiSelectDropDownList { ID = status.ListValueID, Value = status.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (statusFilter == null)
            {
                statusFilter = "NotAll";
            }
            lstMultiStatus.Insert(0, ListItem);
            lstMultiStatus.ForEach(x => {
                if (x.Value.ToUpper() == "PENDING" || x.Value.ToUpper() == "IN PROCESS")
                {
                    x.IsSelected = true;
                }
            });
            selectedStatusList = lstMultiStatus.FindAll(x => x.IsSelected == true);
            // DropDown2DefaultID = lstMultiStatus.Find(x => x.Value.ToLower() == statusFilter.ToLower()).ID;
        }

        protected void setWizardList()
        {
            lstProcesses.Clear();
            IDropDownList ListItem = new IDropDownList();
            lstProcesses = (from process in WizardList
                          select new IDropDownList { ID = process.ProcessWizardID, Value = process.Process }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstProcesses.Insert(0, ListItem);

            DefaultTypeID = lstProcesses[0].ID;
            wizardId = DefaultTypeID;
        }

        private async Task LoadTableConfig()
        {
            RolePermission WizardDataRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.PROCESSDATA);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary"
            };

            IRowActions m2 = new IRowActions
            {
                IconClass = "fas fa-file-upload",
                ActionMethod = Process,
                ButtonClass = "btn-primary"
            };
            IRowActions m4 = new IRowActions
            {
                IconClass = "fa fa-tasks",
                ActionMethod = Tickets,
                ButtonClass = "btn-primary"
            };

            RowActions = new List<IRowActions> { m1, m2,m4 };
            // RowActions.Add(m2);

            IHeaderActions m3 = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-width-height",
                ActionMethod = Add,
                ActionText = "ADD",
                IsDisabled = wizardId != 0 ? false : true
            };
            
            if (WizardDataRolePermission != null && WizardDataRolePermission.Add == true)
            {
                HeaderAction = new List<IHeaderActions> { m3 };
            }

            WizardDataList = new List<ILT.IHR.DTO.ProcessData> { };
        }        

        public void Edit()
        {
            if (selected != null)
            {
                WizardFlowDataModal.Show(selected.ProcessDataID, false);
            }
        }
        public void Process()
        {
            if (selected != null)
            {
                WizardFlowDataModal.Show(selected.ProcessDataID, true);
            }
        }

        public void Tickets()
        {
            wizardName = selected.Process; //lstProcesses.Find(x => x.ID == selected.ProcessDataID).Value;
            WizardFlowDataModal.showTickets(selected.ProcessDataID);
        }

        
        public void Add()
        {
            WizardFlowBaseModal.Show(DefaultTypeID);
        }

        protected async Task LoadList()
        {
            await LoadTableConfig();
            var reponses = (await WizardDataService.GetWizardDatas());
            if (reponses.MessageType == MessageType.Success)
            {
                WizardDataList = reponses.Data;
                lstWizardDataList = WizardDataList;                
                onProcessChange(wizardId);
                
            }
            else
            {
                toastService.ShowError("Error occured", "");
            }
        }

        protected void onProcessChange(int processId)
        {
            DefaultTypeID = processId;
            LoadProcessList(processId);
        }

        protected async Task LoadProcessList(int processId)
        {
            wizardId = processId;
            LoadTableConfig();
            var resp = (await WizardDataService.GetWizardDatas());
            if (resp.MessageType == MessageType.Success)
            {
                if (wizardId != 0)
                {
                    lstWizardDataList = resp.Data.Where(x => x.ProcessWizardID == wizardId).ToList();
                }
                else
                {
                    lstWizardDataList = resp.Data;
                }
                if (selectedStatusList != null && statusFilter != "All")
                {
                    lstWizardDataList = lstWizardDataList.Where(x => selectedStatusList.Any(s => s.ID == x.StatusId)).ToList();
                }
                Elements = new List<Element>();                
                lstWizardDataList.ToList().ForEach(wd =>
                {
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNodeList xmlnode;
                    xmldoc.LoadXml(wd.Data);
                    xmlnode = xmldoc.GetElementsByTagName("WizardData");
                    string lDataColumns = "";
                    foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
                    {
                        foreach (XmlNode nodeField in node.ChildNodes)
                        {
                            if (nodeField.Name.ToUpper() != "EMPLOYEEID")
                            {
                                if (nodeField.Attributes["griddisplay"].InnerText == "true")
                                {
                                    Element element = new Element();
                                    element.Label = nodeField.Attributes["label"].InnerText;
                                    element.Value = nodeField.InnerText;
                                    Elements.Add(element);
                                    if (lDataColumns == "")
                                    {
                                        lDataColumns = nodeField.Attributes["label"].InnerText + ": " + nodeField.InnerText;
                                    //    lDataColumns = nodeField.Attributes["label"].InnerText.ToUpper() != "EMPLOYEE NAME"? "Candidate Name: " + nodeField.InnerText : nodeField.Attributes["label"].InnerText + ": " + nodeField.InnerText;
                                    //}
                                    //else if(nodeField.Attributes["label"].InnerText.Contains("Name"))
                                    //{
                                    //    lDataColumns = nodeField.Attributes["label"].InnerText.ToUpper() != "EMPLOYEE NAME" ? lDataColumns + " " + nodeField.InnerText : lDataColumns + " " + nodeField.Attributes["label"].InnerText + ": " + nodeField.InnerText;
                                    }
                                    else
                                    {
                                        lDataColumns = lDataColumns + ", " + nodeField.Attributes["label"].InnerText + ": " + nodeField.InnerText;
                                    }

                                }
                            }
                        }                        
                    }
                    wd.DataColumns = lDataColumns;
                });
                StateHasChanged();
                //WizardDataDisplay();
            }
            else
            {
                lstWizardDataList = new List<ProcessData> { };
            }
            StateHasChanged();
        }

        public void WizardDataDisplay()
        {
            Table.Columns.Clear();          

            Elements.ForEach(el =>
            {
                var col = new Column<ProcessData>()
                {
                    Title = el.Label,
                    Field = (x) => el.Value,
                    Sortable = true,
                    Filterable = true,
                    Width = "46%"
                };

                if (Table.Columns.FindAll(tbl => tbl.Title == col.Title).Count <= 0)
                {
                    Table.AddColumn(col);
                }
            });
                
        }

        public void RowClick(ILT.IHR.DTO.ProcessData data)
        {
            selected = data;
            StateHasChanged();
        }
       
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
    }
}
