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
using Blazored.SessionStorage;
using System.Text.RegularExpressions;
using ILT.IHR.UI.Pages.Ticket;

namespace ILT.IHR.UI.Shared
{
    public class WizardFlowDataBase : ComponentBase
    {
        [Parameter]
        public string WizardName { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; } //Service 
        [Inject]
        public IToastService toastService { get; set; } //Service        
        [Inject]
        public IWizardService WizardService { get; set; } //Service
        [Inject]
        public IWizardDataService WizardDataService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public IEmailApprovalService emailApprovalService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; }
        public bool ShowDialog { get; set; }
        public List<string> steps;
        public List<ProcessWizard> wizards { get; set; }
        public ProcessWizard wizard { get; set; }
        public List<Element> Elements { get; set; }
        public Element DataContext { get; set; } = new Element();
        public DTO.User user { get; set; }
        public ProcessData wizarddata = new ProcessData();
        public List<ProcessDataTicket> wizardDataTickets = new List<ProcessDataTicket>();
        public List<ProcessData> wizardDatas { get; set; }
        public List<Country> CountryList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public List<State> StateList { get; set; }
        public List<ListValue> TitleList { get; set; }
        public List<ListValue> InvoicingPeriodList { get; set; }  // Table APi Data
        public List<ListValue> PaymentTermList { get; set; }  // Table APi Data
        public List<ListValue> GenderList { get; set; }
        public List<ListValue> WorkAuthorizationList { get; set; }  // Table APi Data
        public List<ListValue> MaritalStautsList { get; set; }  // Table APi Data
        public List<ListValue> PaymentTypeList { get; set; }
        public List<ListValue> EmployMentList { get; set; }
        public List<ListValue> WizardStatusList { get; set; }
        protected ILT.IHR.UI.Pages.Ticket.AddEditTicketBase TicketModal { get; set; }
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase DeleteConfirmation { get; set; }
        public int DefaultPageSize { get; set; }
        public string LoginMessage;
        public bool isControlDisable = false;
        public bool isProcessBtnClick = false;
        public bool isShowTickets = false;
        public bool isSaveProcessBtnDisable = false;
        public string TicketsFor;
        [Parameter]
        public EventCallback<bool> WizardDataUpdated { get; set; }
        public async void Show(int wizardDataID, bool isDisable)
        {
            isShowTickets = false;
            isControlDisable = false;
            isProcessBtnClick = false;
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
            var respEmployee = (await EmployeeService.GetEmployees());
            if (respEmployee.MessageType == MessageType.Success)
            {
                EmployeeList = respEmployee.Data.ToList();
            }
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                EmployMentList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.EMPLOYMENTTYPE).ToList();
                TitleList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TITLE).ToList();
                InvoicingPeriodList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.INVOICINGPERIOD).ToList();
                PaymentTermList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.PAYMENTTERM).ToList();
                GenderList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.GENDER).ToList();
                WorkAuthorizationList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WORKAUTHORIZATION).ToList();
                MaritalStautsList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.MARITALSTATUS).ToList();
                PaymentTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.PAYMENTTYPE).ToList();
            }

            var reponsesData = (await WizardDataService.GetWizardDataByIdAsync(wizardDataID));
            if (reponsesData.MessageType == MessageType.Success)
            {
                wizarddata = reponsesData.Data;
                WizardName = wizarddata.Process;
                if (isDisable)
                {
                    isControlDisable = true;
                    isProcessBtnClick = true;
                }
                if(wizarddata.Status.ToUpper() != ListTypeConstants.WizardStatusConstants.PENDING)
                {
                    isControlDisable = true;
                    isSaveProcessBtnDisable = true;
                } else
                {
                    isSaveProcessBtnDisable = false;
                }

               
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNodeList xmlnode;
                    xmldoc.LoadXml(wizarddata.Data);
                    xmlnode = xmldoc.GetElementsByTagName("WizardData");
                    Elements = new List<Element>();
                    steps = new List<string>();
                    foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
                    {
                        if (!steps.Contains(node.Attributes["name"].InnerText))
                        {
                            steps.Add(node.Attributes["name"].InnerText);
                        }
                        foreach (XmlNode nodeField in node.ChildNodes)
                        {
                            if (nodeField.Name.ToUpper() != "EMPLOYEEID")
                            {
                                Element element = new Element();
                                element.Position = node.Attributes["name"].InnerText;
                                element.Label = nodeField.Attributes["label"].InnerText;
                                element.Name = nodeField.Name;
                                element.ElementType = nodeField.Attributes["elementtype"].InnerText;
                                element.GridDisplay = nodeField.Attributes["griddisplay"].InnerText;
                                element.Required = nodeField.Attributes["required"].InnerText;
                                if (nodeField.Attributes["fullwidth"] != null)
                                {
                                    element.FullWidth = nodeField.Attributes["fullwidth"].InnerText;
                                }
                                else
                                {
                                    element.FullWidth = "";
                                }
                                element.Value = nodeField.InnerText;
                                Elements.Add(element);

                                if (element.Name.ToUpper() == "COUNTRY")
                                {
                                    await GetStates(element.Value);
                                }
                            }
                        }                        
                    }

               

                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Error occured", "");
            }

            ShowDialog = true;
            StateHasChanged();
        }
        public int WizardDataID { get; set; }
        public async void showTickets(int wizardDataID)
        {
            WizardDataID = wizardDataID;
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            ShowDialog = true;
            isShowTickets = true;
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            var reponsesData = (await WizardDataService.GetWizardDataByIdAsync(wizardDataID));
            if (reponsesData.MessageType == MessageType.Success)
            {
                wizarddata = reponsesData.Data;
                wizardDataTickets = wizarddata.ProcessDataTickets;

                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnode;
                xmldoc.LoadXml(wizarddata.Data);
                xmlnode = xmldoc.GetElementsByTagName("WizardData");
                Elements = new List<Element>();
                foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
                {
                    foreach (XmlNode nodeField in node.ChildNodes)
                    {
                        Element element = new Element();
                        element.Name = nodeField.Name;
                        element.Value = nodeField.InnerText;
                        Elements.Add(element);
                    }
                }
                TicketsFor = string.Empty;
                string empElement = string.Empty;
               
                empElement = Elements.Where(x => x.Name == "Employee").FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(empElement))
                {
                    empElement = Elements.Where(x => x.Name == "FirstName").FirstOrDefault()?.Value;
                    empElement = empElement + " " + Elements.Where(x => x.Name == "LastName").FirstOrDefault()?.Value;
                }

                if (empElement != null)
                {
                    TicketsFor = empElement;
                }
                StateHasChanged();
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

        protected async Task onChange(ChangeEventArgs e, string elementName)
        {
            string changeValue = Convert.ToString(e.Value);
            if (elementName.ToUpper() == "COUNTRY")
            {
                if (!String.IsNullOrEmpty(changeValue))
                {
                    await GetStates(changeValue);
                }
                else
                {
                    StateList.Clear();
                }
            }
        }

        private async Task GetStates(string country)
        {
            int countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                StateList = (await CountryService.GetCountryByIdAsync(countryId)).Data.States;
            }
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        public bool ValidateWizardData()
        {
            LoginMessage = "";
            foreach (var element in Elements)
            {
                if ((element.Value == "" || element.Value == null) && element.Required.ToUpper() == "TRUE")
                {
                    LoginMessage = element.Label + " Cannot be blank";
                    return true;
                }
                else if (element.ElementType == "EmailInput" && element.Required.ToUpper() == "TRUE")
                {
                    string email = element.Value;
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(email);
                    if (!match.Success)
                    {
                        LoginMessage = element.Label + " not valid";
                        return true;
                    }
                }
                else if (element.ElementType == "DecimalInput" && element.Required.ToUpper() == "TRUE")
                {
                    string decimalvalue = element.Value;
                    Regex regex = new Regex(@"^[1-9]\d*(\.\d+)?$");
                    Match match = regex.Match(decimalvalue);
                    if (!match.Success)
                    {
                        LoginMessage = element.Label + " not valid";
                        return true;
                    }
                }
            }
            return false;
        }

        public async void SaveWizardData(bool isProcessedbtnClick)
        {
            string employeeIdTag = string.Empty;
            if (ValidateWizardData() == false)
            {                
                var xmlData = "<WizardData>";
                foreach (var step in steps)
                {
                    xmlData = xmlData + "<" + step.Replace(" ", "") + " name='" + step + "'>";
                    foreach (var element in Elements)
                    {
                        if (step == element.Position)
                        {
                            xmlData = xmlData + "<" + element.Name + " label='" + element.Label + "' elementtype='" + element.ElementType + "' griddisplay ='" + element.GridDisplay + "' required='" + element.Required + "' fullwidth ='" + element.FullWidth + "'>" + element.Value + "</" + element.Name + ">";

                            if (element.Name.ToUpper() == "EMPLOYEE")
                            {
                                if (EmployeeList.Count > 0)
                                {
                                    var empId = EmployeeList.Where(x => x.EmployeeName == element.Value).FirstOrDefault()?.EmployeeID;
                                    if (empId != null) { employeeIdTag = "<EmployeeID>" + empId + "</EmployeeID>"; }
                                }
                                else
                                {
                                    var respEmployee = (await EmployeeService.GetEmployees());
                                    if (respEmployee.MessageType == MessageType.Success)
                                    {

                                        EmployeeList = respEmployee.Data.ToList();
                                        var empId = EmployeeList.Where(x => x.EmployeeName == element.Value).FirstOrDefault()?.EmployeeID;
                                        if (empId != null) { employeeIdTag = "<EmployeeID>" + empId + "</EmployeeID>"; }
                                    }
                                }
                                xmlData = xmlData + employeeIdTag;
                                employeeIdTag = string.Empty;
                            }

                        }
                    }
                    xmlData = xmlData + "</" + step.Replace(" ", "") + ">";
                }
                xmlData = xmlData + "</WizardData>";
                Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
                if (resp.MessageType == MessageType.Success)
                {
                    WizardStatusList = resp.Data.Where(x => x.Type.ToUpper() == Constants.WIZARDSTATUS).ToList();
                }
                if (isProcessedbtnClick)
                {
                    wizarddata.StatusId = WizardStatusList.SingleOrDefault(x => x.Value.ToUpper() == ListTypeConstants.WizardStatusConstants.INPROCESS).ListValueID;
                }
                else
                {
                    wizarddata.StatusId = WizardStatusList.SingleOrDefault(x => x.Value.ToUpper() == ListTypeConstants.WizardStatusConstants.PENDING).ListValueID;
                }
                wizarddata.EmailApprovalValidity = Convert.ToInt32(Configuration["EmailApprovalValidity"]);
                string EmailTo = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":ChangeNotification"];
                wizarddata.ChangeNotificationEmailId = EmailTo.Replace(',', ';');
                wizarddata.Data = xmlData;
                wizarddata.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await WizardDataService.SaveWizardData(wizarddata);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Process Data Saved Successfully", "");
                    if (isProcessedbtnClick)
                    {
                        await emailApprovalService.GetEmailApprovals();
                    }
                }
                else
                {
                    toastService.ShowError("Error occured", "");
                }
                await WizardDataUpdated.InvokeAsync(true);
                Close();
            }            
        }

        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }

        public void Cancel()
        {            
            ShowDialog = false;            
            StateHasChanged();

        }

        public void ProcessTicket(int ticketId)
        {
            TicketModal.Show(ticketId, Convert.ToInt32(user.EmployeeID), WizardDataID);
        }
        protected async Task  LoadTicketList(int WizardDataID)
        {
            showTickets(WizardDataID);
        }
    }
}
