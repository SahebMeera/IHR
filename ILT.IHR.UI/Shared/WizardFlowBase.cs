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

namespace ILT.IHR.UI.Shared
{
    public class WizardFlowBase : ComponentBase
    {
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
        public ILookupService LookupService { get; set; }
        public bool ShowDialog { get; set; }
        public List<string> steps;
        public List<ProcessWizard> wizards { get; set; }
        public ProcessWizard wizard { get; set; }
        public List<Element> Elements { get; set; }
        public Element DataContext { get; set; } = new Element();
        public DTO.User user { get; set; }
        public ILT.IHR.DTO.ProcessData wizarddata = new ILT.IHR.DTO.ProcessData();
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
        public string LoginMessage;
        [Parameter]
        public EventCallback<bool> WizardDataUpdated { get; set; }
        public async void Show(int processId)
        {
            LoginMessage = "";
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
           
            var reponses = (await WizardService.GetWizards());
            if (reponses.MessageType == MessageType.Success)
            {
                wizards = reponses.Data.ToList();
                wizard = wizards.Find(x => x.ProcessWizardID == processId);
                wizard.CreatedBy = user.FirstName + " " + user.LastName;               
                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnode;
                xmldoc.LoadXml(wizard.Elements);
                xmlnode = xmldoc.GetElementsByTagName("WizardSteps");
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
                        Element element = new Element();
                        element.Position = node.Attributes["name"].InnerText;
                        element.ElementType = nodeField.ChildNodes.Item(0).InnerText;
                        element.Name = nodeField.ChildNodes.Item(1).InnerText;
                        element.Label = nodeField.ChildNodes.Item(2).InnerText;
                        element.GridDisplay = nodeField.ChildNodes.Item(3).InnerText;
                        element.Required = nodeField.ChildNodes.Item(4).InnerText;
                        if (nodeField.ChildNodes.Item(5) != null && nodeField.ChildNodes.Item(5).Name.ToUpper() == "FULLWIDTH")
                        {
                            element.FullWidth = nodeField.ChildNodes.Item(5).InnerText;
                        }
                        else
                        {
                            element.FullWidth = "";
                        }
                        
                        Elements.Add(element);
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

        public bool ValidateWizardData(string ActiveStepName)
        {
            LoginMessage = "";
            foreach (var element in Elements)
            {
                if (element.Position == ActiveStepName && element.Required.ToUpper() == "TRUE" && (element.Value == "" || element.Value == null))
                {
                    LoginMessage = element.Label + " Cannot be blank";
                    return true;
                }
                else if (element.Position == ActiveStepName && element.ElementType == "EmailInput" && element.Required.ToUpper() == "TRUE")
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
                else if (element.Position == ActiveStepName && element.ElementType == "DecimalInput" && element.Required.ToUpper() == "TRUE")
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
                else if (element.Position == ActiveStepName && element.ElementType == "DateInput" && element.Required.ToUpper() == "TRUE")
                {
                    string datevalue = element.Value;
                    if (DateTime.TryParse(datevalue, out DateTime dateTime) == false)
                    {
                        LoginMessage = element.Label + " not valid";
                        return true;
                    }
                }
            }
            return false;
        }

        public async void SaveWizardData()
        {
            var wizardID = wizard.ProcessWizardID;
            var username = wizard.CreatedBy;
            string employeeIdTag = string.Empty;
            var xmlData = "<WizardData>";
            foreach(var step in steps)
            {
                xmlData = xmlData + "<" + step.Replace(" ", "") + " name='" + step + "'>";
                foreach (var element in Elements)
                {
                    if (step == element.Position)
                    {
                        xmlData = xmlData + "<" + element.Name + " label='" + element.Label + "' elementtype='" + element.ElementType + "' griddisplay ='" + element.GridDisplay  + "' required ='" + element.Required + "' fullwidth ='" + element.FullWidth + "'>" + element.Value + "</" + element.Name + ">";
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
            wizarddata.StatusId = WizardStatusList.SingleOrDefault(x => x.Value.ToUpper() == ListTypeConstants.WizardStatusConstants.PENDING).ListValueID;
            wizarddata.ProcessWizardID = wizardID;
            wizarddata.Data = xmlData;
            wizarddata.CreatedBy = username;
            var result = await WizardDataService.SaveWizardData(wizarddata);
            if (result.MessageType == MessageType.Success)
            {
                toastService.ShowSuccess("Process Data Saved Successfully", "");
            }
            else
            {
                toastService.ShowError("Error occured", "");
            }
            await WizardDataUpdated.InvokeAsync(true);
            Close();
        }
    }
}
