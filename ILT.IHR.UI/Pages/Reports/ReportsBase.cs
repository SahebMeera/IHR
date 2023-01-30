using BlazorDownloadFile;
using Blazored.SessionStorage;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ILT.IHR.UI.Pages.Reports

{
    public class ReportsBase: ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IWebHostEnvironment _webHostEnvironment { get; set; }
        [Inject]
        public ILookupService lookupService { get; set; }
        [Inject]
        public ICountryService CountryService  { get; set; }
        [Inject]
        public ILeaveBalanceService leaveBalanceService { get; set; }
        [Inject]
        public IAuditLogService auditlogService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject] 
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        public List<DTO.ListValue> reportTypes { get; set; }
        public DTO.User user { get; set; }
        public List<IMultiSelectDropDownList> lstAssetType { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketStatus { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstAssetChangeSetsType { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketChangeSetsStatus { get; set; } //Drop Down Api Data
        public List<Country> CountryList { get; set; }
        public string pdfContent { get; set; }
        //report Parameters
        public string reportType { get; set; }
        public string country { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool isCountryHidden { get; set; }
        public bool isShowEmployeeDetail { get; set; }
        public bool isShowAssetReport { get; set; }
        public bool isDateHidden { get; set; }
        public List<ListValue> EmployMentList { get; set; }
        public List<IDropDownList> lstCountry { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstStatus { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstLeaveStatus { get; set; } //Drop Down Api Data
        public string employeeType { get; set; }
        [Inject]
        public ILeaveService leaveService { get; set; }
        public List<IMultiSelectDropDownList> lstEmployeeType { get; set; } //Drop Down Api Data
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service

        protected List<RolePermission> RolePermissions;
        protected RolePermission ReportRolePermission;

        // Validate missing for required fields boolean
        public bool isStartDateRequired { get; set; } = false;
        public bool isEndDateRequired { get; set; } = false;
        public bool isReportTypeRequired { get; set; } = false;
        public bool isShowAssetChangeSets { get; set; } = false;
        public bool isShowI9ExpiryForm { get; set; } = false;
        public List<IMultiSelectDropDownList> selectedAssetTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedAssetChangeSetsTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedAssetStatusList { get; set; }
        public List<IMultiSelectDropDownList> selectedAssetChangeSetsStatusList { get; set; }
        public List<ListValue> AssetStatusList { get; set; }  // Table APi Data
        public List<ListValue> AssetTypeList { get; set; }  // Table APi Data
        public string AssetType { get; set; }
        public string TicketStatus { get; set; }
        public List<IMultiSelectDropDownList> ListAssetStatus { get; set; }
        public List<IMultiSelectDropDownList> ListAssetChangeSetsStatus { get; set; }
        public List<ILT.IHR.DTO.Asset> AssetsList { get; set; }  
        public List<ILT.IHR.DTO.Asset> lstAssetsList { get; set; }
        public List<DTO.AssetChangeSet> assetChangeSet { get; set; }
        public List<DTO.AssetChangeSet> lstAssetChangeSet { get; set; }
        public List<DTO.FormI9> FormI9List { get; set; }
        protected async override Task OnInitializedAsync()
        {
            lstCountry = new List<IDropDownList>();
            lstStatus = new List<IDropDownList>();
            lstLeaveStatus = new List<IDropDownList>();
            lstEmployeeType = new List<IMultiSelectDropDownList>();
            lstAssetType = new List<IMultiSelectDropDownList>();
            lstAssetChangeSetsType = new List<IMultiSelectDropDownList>();
            lstTicketStatus = new List<IMultiSelectDropDownList>();
            lstTicketChangeSetsStatus = new List<IMultiSelectDropDownList>();
            EmployeesList = new List<ILT.IHR.DTO.Employee> { };
            assetChangeSet = new List<ILT.IHR.DTO.AssetChangeSet> { };
            lstAssetChangeSet = assetChangeSet;
            lstEmployees = EmployeesList;
            AssetsList = new List<ILT.IHR.DTO.Asset> { };
            FormI9List = new List<DTO.FormI9> { };
            lstAssetsList = AssetsList;
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            ReportRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.REPORTS);
            await loadCountry();
            if(CountryList != null)
            {
                setCountryList();
            }
            loadDaysDropdown();
            setStatusList();
            Response<IEnumerable<ListValue>> resp = (await lookupService.GetListValues());
            if(resp.MessageType == MessageType.Success)
            {
                reportTypes = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.REPORTTYPE).ToList();
                EmployMentList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.EMPLOYMENTTYPE).ToList();
                AssetTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETTYPE).ToList();
                AssetStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETSTATUS).ToList();
                ListValue reportItem = new ListValue();
                reportItem.ValueDesc = "Select";
                setLeaveStatusList();
                setEmployeeTypeList();
                setAssetTypeList();
                setAssetChangeSetsTypeList();
                setAssetStatusList();
                setAssetChangeSetsStatusList();
                reportTypes.Insert(0,reportItem);
            }
            await loadEmployeeList();
            await loadAssetList();
            await loadAssetChangesList();
        }

        public async Task loadCountry()
        {
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
                country = CountryList[1].CountryDesc;
            }
        }
        public bool showSpinner { get; set; } = false;
        public async Task generateReport()
        {
            pdfContent = "";
            var report = new LocalReport();
            if (!string.IsNullOrEmpty(reportType))
            {
                switch (reportType)
                {
                    case "LEAVESUMMARY": report = await getLeaveReport();
                                         break;
                    case "LEAVEDETAIL": report = await getLeaveDetailReport();
                        break;
                    case "AUDITLOG": report = await getAuditLogReport();
                        break;
                    case "EMPLOYEEDETAIL":
                        report = await getEmployeeDetailReport();
                        break; 
                    case "PENDINGLEAVE":
                        report = await getPendingLeavesReport();
                        break;
                    case "ASSET":
                        report = await getAssetReport();
                        break;
                    case "ASSETHISTORY":
                        report = await getAssetChangeSetsReport();
                        break;
                    case "I9EXPIRYFORM":
                        report = await getI9ExpiryDatesForm();
                        break;
                }
                if(report != null)
                {
                   // showSpinner = true;
                    openHTML(report);
                }
            } else
            {
                isReportTypeRequired = true;
                isStartDateRequired = true;
                isEndDateRequired = true;
            }
        }

        public async Task exportReport()
        {
            var report = new LocalReport();
            string FileName = "";
            if (!string.IsNullOrEmpty(reportType))
            {
                switch (reportType)
                {
                    case "LEAVESUMMARY":
                        report = await getLeaveReport();
                        FileName = "Leave Summary";
                        break;
                    case "LEAVEDETAIL":
                        report = await getLeaveDetailReport();
                        FileName = "Leave Detail";
                        break;
                    case "AUDITLOG":
                        report = await getAuditLogReport();
                        FileName = "Audit Log";
                        break;
                    case "EMPLOYEEDETAIL":
                        report = await getEmployeeDetailReport();
                        FileName = "Employee Detail";
                        break;
                    case "PENDINGLEAVE":
                        report = await getPendingLeavesReport();
                        FileName = "Pending Leave";
                        break;
                    case "ASSET":
                        report = await getAssetReport();
                        FileName = "Asset";
                        break;
                    case "ASSETHISTORY":
                        report = await getAssetChangeSetsReport();
                        FileName = "AssetHistory";
                        break;
                    case "I9EXPIRYFORM":
                        report = await getI9ExpiryDatesForm();
                        FileName = "I9Expiry Form";
                        break;
                }
                if (report != null)
                {
                    await downloadExcel(report, FileName);
                }

            } else
            {
                isReportTypeRequired = true;
                isStartDateRequired = true;
                isEndDateRequired = true;
            }
        }

        public async Task<LocalReport> getLeaveReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            reportReq.Country = country;
            reportReq.StartDate = StartDate;
            reportReq.EndDate = EndDate;
            if (reportReq.StartDate != null && reportReq.EndDate != null)
            {
                isStartDateRequired = false;
                isStartDateRequired = false;
                showSpinner = true;
                dt = (await leaveBalanceService.GetReportLeaveInfo(reportReq, LeaveDetailStatus));

                var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Clear();
                var dtcommon = new DataTable();
                dtcommon.Columns.Add("header1");
                dtcommon.Columns.Add("header2");
                dtcommon.Rows.Add("Leave Summary", "(" + FormatDate(StartDate) + " - " + FormatDate(EndDate) + ")");
                using var report = new LocalReport();
                report.DataSources.Add(new ReportDataSource("dsLeaveInfo", dt));
                report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
                report.ReportPath = path;
                return report;
            }
            else
            {
                checkDatesFieldsRequired();
                return null;
            }
        }

        public async Task<LocalReport> getLeaveDetailReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            var data = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            reportReq.Country = country;
            reportReq.StartDate = StartDate;
            reportReq.EndDate = EndDate;
            //reportReq.LeaveStatus = LeaveDetailStatus;
            if (reportReq.StartDate != null && reportReq.EndDate != null)
            {
                isStartDateRequired = false;
                isStartDateRequired = false;
                showSpinner = true;
                dt = (await leaveBalanceService.GetReportLeaveDetailInfo(reportReq, LeaveDetailStatus));
              
                var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Clear();
                var dtcommon = new DataTable();
                dtcommon.Columns.Add("header1");
                dtcommon.Columns.Add("header2");
                dtcommon.Rows.Add("Leave Detail", "(" + FormatDate(StartDate) + " - " + FormatDate(EndDate) + ")");
                using var report = new LocalReport();
                report.DataSources.Add(new ReportDataSource("dsLeaveInfo", dt));
                report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
                report.ReportPath = path;
                return report;
            } else
            {
                checkDatesFieldsRequired();
                return null;
            }
        }
        
        protected void setLeaveStatusList()
        {
            lstLeaveStatus.Clear();
            IDropDownList ListItem = new IDropDownList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstLeaveStatus.Insert(0, ListItem);
            IDropDownList ListItem1 = new IDropDownList();
            ListItem1.ID = 1;
            ListItem1.Value = "Active";
            lstLeaveStatus.Insert(1, ListItem1);
            IDropDownList ListItem2 = new IDropDownList();
            ListItem2.ID = 2;
            ListItem2.Value = "Termed";
            lstLeaveStatus.Insert(2, ListItem2);
            LeaveDetailStatus = "Active";
            DefaultLeaveStatusID = lstLeaveStatus.Find(x => x.Value.ToLower() == "Active".ToLower()).ID;
        }
        public void OnLeaveStatusChange(ChangeEventArgs e)
        {
            DefaultLeaveStatusID = Convert.ToInt32(e.Value);
            LeaveDetailStatus = lstLeaveStatus.Find(x => x.ID == DefaultLeaveStatusID).Value;
            // LoadEmployees();
        }
        public async Task<LocalReport> getAuditLogReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            reportReq.StartDate = StartDate;
            reportReq.EndDate = EndDate;
            if(reportReq.StartDate != null && reportReq.EndDate != null)
            {
                isStartDateRequired = false;
                isStartDateRequired = false;
                showSpinner = true;
                dt = (await auditlogService.GetReportAuditLogInfo(reportReq));
                var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Clear();
                var dtcommon = new DataTable();
                dtcommon.Columns.Add("header1");
                dtcommon.Columns.Add("header2");
                dtcommon.Rows.Add("Audit Log", "(" + FormatDate(StartDate) + " - " + FormatDate(EndDate) + ")");
                using var report = new LocalReport();
                report.DataSources.Add(new ReportDataSource("dsAuditInfo", dt));
                report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
                report.ReportPath = path;
                return report;
            } else {
                checkDatesFieldsRequired();
                //using var report = new LocalReport();
                return null;
            }
        }

        public async Task checkDatesFieldsRequired()
        {
            DTO.Report reportReq = new DTO.Report();
            if (reportReq.StartDate != null && reportReq.EndDate == null)
            {
                isStartDateRequired = false;
                isStartDateRequired = true;
            }
            else if (reportReq.StartDate == null && reportReq.EndDate != null)
            {
                isStartDateRequired = true;
                isStartDateRequired = false;
            }
            else
            {
                isStartDateRequired = true;
                isStartDateRequired = true;
            }
        }
            public async Task<LocalReport> getEmployeeDetailReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            showSpinner = true;
            dt = await loadEmployeeList();
            var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Clear();
            var dtcommon = new DataTable();
            dtcommon.Columns.Add("header1");
            dtcommon.Columns.Add("header2");
            dtcommon.Rows.Add("Employee Detail", "(" + FormatDate(DateTime.Now) + " - " + FormatDate(DateTime.Now) + ")");
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("dsEmployeeDetailInfo", dt));
            report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
            report.ReportPath = path;
            return report;
        }
        public async Task<LocalReport> getAssetReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            showSpinner = true;
            dt = await loadAssetList();
            var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Clear();
            var dtcommon = new DataTable();
            dtcommon.Columns.Add("header1");
            dtcommon.Columns.Add("header2");
            dtcommon.Rows.Add("Asset");
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("dsAssetInfo", dt));
            report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
            report.ReportPath = path;
            return report;
        }
        public async Task<LocalReport> getAssetChangeSetsReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            showSpinner = true;
            dt = await loadAssetChangesList();
            var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\AssetchangeSets.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Clear();
            var dtcommon = new DataTable();
            dtcommon.Columns.Add("header1");
            dtcommon.Columns.Add("header2");
            dtcommon.Rows.Add("Asset History");
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("dsAssetchangeSetsInfo", dt));
            report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
            report.ReportPath = path;
            return report;
        }
        public async Task<LocalReport> getPendingLeavesReport()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            DTO.Report reportReq = new DTO.Report();
            reportReq.Country = country;
            reportReq.StartDate = StartDate;
            reportReq.EndDate = EndDate;
            showSpinner = true;
            var resp = (await leaveService.GetLeave(""));
            List<DTO.Leave> leaveInfo = new List<DTO.Leave>();
            if (resp.MessageType == MessageType.Success)
            {
                leaveInfo = resp.Data.Where(a => a.StatusValue == LeaveStatus.PENDING && a.Country == reportReq.Country ).ToList();
                string json = JsonConvert.SerializeObject(leaveInfo);
                dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            }
            var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Clear();
            var dtcommon = new DataTable();
            dtcommon.Columns.Add("header1");
            dtcommon.Columns.Add("header2");
            dtcommon.Rows.Add("Pending Leave");
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("dsPendingLeavesInfo", dt));
            report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
            report.ReportPath = path;
            return report;
        }
        [Inject]
        public IFormI9Service FormI9Service { get; set; } //Service
        public async Task<LocalReport> getI9ExpiryDatesForm()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dt = new DataTable();
            //DTO.Report reportReq = new DTO.Report();
            //reportReq.Country = country;
            //reportReq.IExpiryDate = StartDate;
            //reportReq.EndDate = EndDate;
            showSpinner = true;
            DateTime expirydate = await getI9FormExpiryDate(dayID);
            dt  = (await FormI9Service.GetI9ExpiryForm(expirydate));
            var path = $"{this._webHostEnvironment.WebRootPath}\\reports\\{reportType}.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Clear();
            var dtcommon = new DataTable();
            dtcommon.Columns.Add("header1");
            dtcommon.Columns.Add("header2");
            dtcommon.Rows.Add("I9 Documents Expiry", "(Expiry Date <= " + FormatDate(expirydate) + ")");
          //  dtcommon.Rows.Add("I9 Documents Expiry");
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("dsI9ExpiryFormInfo", dt));
            report.DataSources.Add(new ReportDataSource("dsCommonInfo", dtcommon));
            report.ReportPath = path;
            return report;
        }
        public async Task<DateTime> getI9FormExpiryDate(int day)
        {
            DateTime dateTime = DateTime.Now;
            if (day != null)
            {
                DateTime otherDateTime = dateTime.AddDays(+day);
                return otherDateTime;
            } else
            {
                return dateTime;
            }
        }

        public async Task openPDF(LocalReport report)
        {
            var pdf = report.Render("PDF");
            string base64String = Convert.ToBase64String(pdf);
            await JSRuntime.InvokeAsync<string>("FileSaveAs", base64String);
            //pdfContent = "data:image/jpeg;base64," + base64String;
            StateHasChanged();
        }

        public void openHTML(LocalReport report)
        {
                
                var html = report.Render("HTML5");
                pdfContent = System.Text.Encoding.Default.GetString(html);
                showSpinner = false;
                StateHasChanged();
        }
        public async Task downloadExcel(LocalReport report, string reportType)
        {
            string fileName = "";
            if (StartDate != null)
            {
                 fileName = reportType + " " + FormatDate(StartDate) + " - " + FormatDate(EndDate) + "";
            } else
            {
                 fileName = reportType;
            }
            var word = report.Render("EXCELOPENXML");
            MemoryStream memorystream = new MemoryStream(word);
            memorystream.Position = 0;
            await BlazorDownloadFileService.DownloadFile(fileName+".xlsx", memorystream, "application/octet-stream");
            showSpinner = false;
            //string base64String = Convert.ToBase64String(word);
            //await JSRuntime.InvokeAsync<string>("FileSaveAs", base64String);
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dd MMM yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
        public void onReportChange(ChangeEventArgs e)
        {
            isReportTypeRequired = false;
            isShowAssetReport = false;
            isShowAssetChangeSets = false;
            isShowI9ExpiryForm = false;
            if (e.Value != null && e.Value != "")
            {
                isStartDateRequired = false;
                isStartDateRequired = false;
                isReportTypeRequired = false;
                isShowAssetReport = false;
                isShowAssetChangeSets = false;
                isShowI9ExpiryForm = false;
                var value = e.Value.ToString();
                if(value.ToUpper() != "SELECT")
                {
                    setLeaveStatusList();
                    if (value == "AUDITLOG")
                    {
                        isCountryHidden = true;
                        isShowEmployeeDetail = false;
                        isDateHidden = false;
                        isShowAssetReport = false;
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = false;
                    }
                    else if (value == "EMPLOYEEDETAIL")
                    {
                        setCountryList();
                        setStatusList();
                        setEmployeeTypeList();
                        isShowEmployeeDetail = true;
                        isDateHidden = true;
                        isShowAssetReport = false;
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = false;
                    }
                    else if (value == "PENDINGLEAVE")
                    {
                        if(CountryList != null)
                        {
                            country = CountryList[0].CountryDesc;
                        }
                        isDateHidden = true;
                        isCountryHidden = false;
                        isShowEmployeeDetail = false;
                        isShowAssetReport = false;
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = false;
                    }
                    else if (value == "ASSET")
                    {
                        isDateHidden = true;
                        TicketStatus = null;
                        AssetType = null;
                        setAssetTypeList();
                        setAssetStatusList();
                        isCountryHidden = true;
                        isShowAssetReport = true;
                        isShowEmployeeDetail = false;
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = false;
                    }
                    else if (value == "ASSETHISTORY")
                    {
                        isDateHidden = true;
                        isCountryHidden = true;
                        isShowAssetReport = false;
                        isShowEmployeeDetail = false;
                        AssetChangeSetsType = null;
                        TicketChangeSetsStatus = null;
                        setAssetChangeSetsTypeList();
                        setAssetChangeSetsStatusList();
                        isShowAssetChangeSets = true;
                        isShowI9ExpiryForm = false;
                    }
                    else if (value == "I9EXPIRYFORM")
                    {
                        if (I9FormDaysList != null)
                        {
                            dayID = Convert.ToInt32(I9FormDaysList.Find(x => x.Day == 180).Day);
                        }
                        isDateHidden = true;
                        isCountryHidden = true;
                        isShowAssetReport = false;
                        isShowEmployeeDetail = false;
                        AssetChangeSetsType = null;
                        TicketChangeSetsStatus = null;
                        setAssetChangeSetsTypeList();
                        setAssetChangeSetsStatusList();
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = true;
                    }
                    else
                    {
                        isDateHidden = false;
                        isCountryHidden = false;
                        isShowEmployeeDetail = false;
                        isShowAssetReport = false;
                        isShowAssetChangeSets = false;
                        isShowI9ExpiryForm = false;
                    }
                } else
                {
                    isReportTypeRequired = true;
                }
               
            } else
            {
                isReportTypeRequired = true;
            }
           
            
        }
        public class IDropDownList
        {
            public int ID { get; set; }
            public string Value { get; set; }

        }
        public int DefaultTypeID { get; set; }
        public int DefaultStatusID { get; set; }
        public int DefaultLeaveStatusID { get; set; }
        public int DefaultCountryID { get; set; }
        public string EmployeeCountry { get; set; }
        public string EmployeeStatus { get; set; }
        public string LeaveDetailStatus { get; set; }
        public List<ILT.IHR.DTO.Employee> EmployeesList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Employee> lstEmployees { get; set; }  // Table APi Data
        public List<IMultiSelectDropDownList> employeeTypeList { get; set; }
        public List<IMultiSelectDropDownList> selectedempTypeList { get; set; }
        [Inject]
        public IAssetChangesetService AssetChangesetService { get; set; } //Service
        public void onChangeEmployeeTypeList(List<IMultiSelectDropDownList> empTypeList)
        {
            employeeTypeList = empTypeList;
            selectEmployeeTypeList();
           // LoadEmployees();
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

        protected void setCountryList()
        {
            lstCountry.Clear();
            IDropDownList ListItem = new IDropDownList();
            lstCountry = (from country in CountryList
                          select new IDropDownList { ID = country.CountryID, Value = country.CountryDesc }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            lstCountry.Insert(0, ListItem);
            EmployeeCountry = "United States";
            DefaultCountryID = lstCountry.Find(x => x.Value.ToLower() == "United States".ToLower()).ID;
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
            EmployeeStatus = "Active";
            DefaultStatusID = lstStatus.Find(x => x.Value.ToLower() == "Active".ToLower()).ID;
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
                employeeType = "Fulltime Salaried";
            }
            lstEmployeeType.Insert(0, ListItem);
            lstEmployeeType.ForEach(x => {
                if (x.Value.ToLower() == employeeType.ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedempTypeList = lstEmployeeType.FindAll(x => x.IsSelected == true);
            DefaultTypeID = lstEmployeeType.Find(x => x.Value.ToLower() == employeeType.ToLower()).ID;
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
        public void OnCountryChange(ChangeEventArgs e)
        {
            DefaultCountryID = Convert.ToInt32(e.Value); ;
            EmployeeCountry = lstCountry.Find(x => x.ID == DefaultCountryID).Value;
           // LoadEmployees();
        }
        public void OnStatusChange(ChangeEventArgs e)
        {
             DefaultStatusID = Convert.ToInt32(e.Value);
            EmployeeStatus = lstStatus.Find(x => x.ID == DefaultStatusID).Value;
           // LoadEmployees();
        }

        public async Task<DataTable> loadEmployeeList()
        {
            EmployeesList = new List<ILT.IHR.DTO.Employee> { };
            lstEmployees = EmployeesList;
            var respEmployee = (await EmployeeService.GetEmployeeInfo());
            if (respEmployee.MessageType == MessageType.Success)
            {
                EmployeesList = respEmployee.Data.ToList();
                if (EmployeeCountry != "All" && employeeType != "All" && selectedempTypeList != null && EmployeeStatus != "All")
                {
                    if (EmployeeStatus == "Active")
                    {
                        lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry && (x.TermDate == null || x.TermDate > DateTime.Now) && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                    }
                    else
                    {
                        lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry && x.TermDate != null && x.TermDate <= DateTime.Now && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                    }

                }
                else if (EmployeeCountry == "All" && employeeType != "All" && EmployeeStatus != "All")
                {
                    if (EmployeeStatus == "Active")
                    {
                        lstEmployees = EmployeesList.Where(x => (x.TermDate == null || x.TermDate > DateTime.Now) && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                    }
                    else
                    {
                        lstEmployees = EmployeesList.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                    }
                }
                else if (EmployeeCountry != "All" && employeeType == "All" && EmployeeStatus != "All")
                {
                    if (EmployeeStatus == "Active")
                    {
                        lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry && (x.TermDate == null || x.TermDate > DateTime.Now)).ToList();
                    }
                    else
                    {
                        lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry && x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                    }
                }
                else if (EmployeeCountry != "All" && employeeType != "All" && EmployeeStatus == "All")
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry && selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
                else if (EmployeeCountry == "All" && employeeType == "All" && EmployeeStatus != "All")
                {
                    if (EmployeeStatus == "Active")
                    {
                        lstEmployees = EmployeesList.Where(x => x.TermDate == null || x.TermDate > DateTime.Now).ToList();
                    }
                    else
                    {
                        lstEmployees = EmployeesList.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
                    }
                }
                else if (EmployeeCountry != "All" && employeeType == "All" && EmployeeStatus == "All")
                {
                    lstEmployees = EmployeesList.Where(x => x.Country == EmployeeCountry).ToList();
                }
                else if (EmployeeCountry == "All" && employeeType != "All" && EmployeeStatus == "All")
                {
                    lstEmployees = EmployeesList.Where(x => selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
                }
                else
                {
                    lstEmployees = EmployeesList;
                }
               }
                string json = JsonConvert.SerializeObject(lstEmployees);
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                return dt;
            }


        // Asset Report Information
        protected void setAssetStatusList()
        {
            lstTicketStatus.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstTicketStatus = (from employee in AssetStatusList
                               select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (TicketStatus == null)
            {
                TicketStatus = "Unassigned";
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
                if (x.Value.ToLower() == "Assigned Temp".ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedAssetStatusList = lstTicketStatus.FindAll(x => x.IsSelected == true);
            //DropDown2DefaultID = lstTicketStatus.Find(x => x.Value.ToLower() == TicketStatus.ToLower()).ID;
        }
        protected string getSelectedAssetStatus()
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
        protected void OnMultiDropDownForAssetStatusChange(ChangeEventArgs e, int key)
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
            onChangeAssetStatusList(lstTicketStatus);
        }
        public void onChangeAssetStatusList(List<IMultiSelectDropDownList> assetTypesList)
        {
            ListAssetStatus = assetTypesList;
            selectAssetStatusList();
            loadAssetList();
        }
        public void selectAssetStatusList()
        {
            selectedAssetStatusList = ListAssetStatus.FindAll(x => x.IsSelected == true);
            var selectAssetType = ListAssetStatus.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                TicketStatus = "All";
            }
            else
            {
                TicketStatus = "NotAll";
            }
        }
        protected void setAssetTypeList()
        {
            lstAssetType.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstAssetType = (from employee in AssetTypeList
                            select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;
            if (AssetType == null)
            {
                AssetType = "All";
            }
            lstAssetType.Insert(0, ListItem);
            lstAssetType.ForEach(x => {
                x.IsSelected = true;
            });
            selectedAssetTypeList = lstAssetType.FindAll(x => x.IsSelected == true);
            //DropDown2DefaultID = lstAssetType.Find(x => x.Value.ToLower() == AssetType.ToLower()).ID;
        }
        protected string getSelectedAssetTypes()
        {
            string roles = "";
            if (lstAssetType != null)
            {
                if (lstAssetType.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstAssetType)
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
        public void OnMultiDropDownAssetTypeChange(ChangeEventArgs e, int key)
        {
            var index = lstAssetType.FindIndex(x => x.ID == key);
            var userRole = lstAssetType.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstAssetType.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstAssetType.FindIndex(x => x.ID == 0);
                if (lstAssetType[isAllSelectOrNot].IsSelected == true)
                {
                    lstAssetType[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstAssetType[index] = userRole;
            onChangeAssetTypeList(lstAssetType);
        }
        public void onChangeAssetTypeList(List<IMultiSelectDropDownList> assetTypesList)
        {
            assetTypeSelectedList = assetTypesList;
            selectAssetTypeList();
            loadAssetList();
        }
        public List<IMultiSelectDropDownList> assetTypeSelectedList { get; set; }
        public List<IMultiSelectDropDownList> assetChangeSetsTypeSelectedList { get; set; }
        public void selectAssetTypeList()
        {
            selectedAssetTypeList = assetTypeSelectedList.FindAll(x => x.IsSelected == true);
            var selectAssetType = assetTypeSelectedList.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                AssetType = "All";
            }
            else
            {
                AssetType = "NotAll";
            }
        }

        [Inject]
        public IAssetService AssetService { get; set; } //Service

        protected async Task<DataTable> loadAssetList()
        {
            var reponses = new Response<IEnumerable<DTO.Asset>>();
            reponses = (await AssetService.GetAssets());
            if (reponses.MessageType == MessageType.Success)
            {
                AssetsList = reponses.Data.ToList();
                if (AssetType != "All" && selectedAssetTypeList != null && TicketStatus != "All" && selectedAssetStatusList != null)
                {
                    lstAssetsList = AssetsList.Where(x => selectedAssetTypeList.Any(s => s.ID == x.AssetTypeID) && selectedAssetStatusList.Any(t => t.ID == x.StatusID)).ToList();
                }
                else if (AssetType == "All" && TicketStatus != "All")
                {
                    lstAssetsList = AssetsList.Where(x => selectedAssetStatusList.Any(t => t.ID == x.StatusID)).ToList();

                }
                else if (AssetType != "All" && TicketStatus == "All")
                {
                    lstAssetsList = AssetsList.Where(x => selectedAssetTypeList.Any(s => s.ID == x.AssetTypeID)).ToList();
                }
                else
                {
                    lstAssetsList = AssetsList;
                }
            }
            string json = JsonConvert.SerializeObject(lstAssetsList);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            return dt;
        }

        // AssetChangeSets
        public string AssetChangeSetsType { get; set; }
        public string TicketChangeSetsStatus { get; set; }
        protected void setAssetChangeSetsStatusList()
        {
            lstTicketChangeSetsStatus.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstTicketChangeSetsStatus = (from employee in AssetStatusList
                                         select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;

            if (TicketChangeSetsStatus == null)
            {
                TicketChangeSetsStatus = "Unassigned";
            }
            lstTicketChangeSetsStatus.Insert(0, ListItem);
            lstTicketChangeSetsStatus.ForEach(x => {
                if (x.Value.ToLower() == TicketChangeSetsStatus.ToLower())
                {
                    x.IsSelected = true;
                }
                if (x.Value.ToLower() == "Assigned".ToLower())
                {
                    x.IsSelected = true;
                }
                if (x.Value.ToLower() == "Assigned Temp".ToLower())
                {
                    x.IsSelected = true;
                }
            });
            selectedAssetChangeSetsStatusList = lstTicketChangeSetsStatus.FindAll(x => x.IsSelected == true);
            //DropDown2DefaultID = lstTicketStatus.Find(x => x.Value.ToLower() == TicketStatus.ToLower()).ID;
        }
        protected string getSelectedAssetChangeSetsStatus()
        {
            string roles = "";
            if (lstTicketChangeSetsStatus != null)
            {
                if (lstTicketChangeSetsStatus.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstTicketChangeSetsStatus)
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
        protected void OnMultiDropDownForAssetChangeSetsStatusChange(ChangeEventArgs e, int key)
        {
            var index = lstTicketChangeSetsStatus.FindIndex(x => x.ID == key);
            var userRole = lstTicketChangeSetsStatus.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstTicketChangeSetsStatus.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstTicketChangeSetsStatus.FindIndex(x => x.ID == 0);
                if (lstTicketChangeSetsStatus[isAllSelectOrNot].IsSelected == true)
                {
                    lstTicketChangeSetsStatus[isAllSelectOrNot].IsSelected = false;
                }
            }
            lstTicketChangeSetsStatus[index] = userRole;
            onChangeAssetChangeSetsStatusList(lstTicketChangeSetsStatus);
        }
        public void onChangeAssetChangeSetsStatusList(List<IMultiSelectDropDownList> assetTypesList)
        {
            ListAssetChangeSetsStatus = assetTypesList;
            selectAssetChangeSetsStatusList();
            loadAssetChangesList();

        }
        public void selectAssetChangeSetsStatusList()
        {
            selectedAssetChangeSetsStatusList = ListAssetChangeSetsStatus.FindAll(x => x.IsSelected == true);
            var selectAssetType = ListAssetChangeSetsStatus.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                TicketChangeSetsStatus = "All";
            }
            else
            {
                TicketChangeSetsStatus = "NotAll";
            }
        }
        protected void setAssetChangeSetsTypeList()
        {
            lstAssetChangeSetsType.Clear();
            IMultiSelectDropDownList ListItem = new IMultiSelectDropDownList();
            lstAssetChangeSetsType = (from employee in AssetTypeList
                            select new IMultiSelectDropDownList { ID = employee.ListValueID, Value = employee.ValueDesc, IsSelected = false }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            ListItem.IsSelected = false;
            if (AssetChangeSetsType == null)
            {
                AssetChangeSetsType = "All";
            }
            lstAssetChangeSetsType.Insert(0, ListItem);
            lstAssetChangeSetsType.ForEach(x => {
                x.IsSelected = true;
            });
            selectedAssetChangeSetsTypeList = lstAssetChangeSetsType.FindAll(x => x.IsSelected == true);
            //DropDown2DefaultID = lstAssetType.Find(x => x.Value.ToLower() == AssetType.ToLower()).ID;
        }
        protected string getSelectedAssetChangesTypes()
        {
            string roles = "";
            if (lstAssetChangeSetsType != null)
            {
                if (lstAssetChangeSetsType.FindIndex(x => x.IsSelected == true) == -1)
                {
                    return "select";
                }
                else
                {
                    foreach (var item in lstAssetChangeSetsType)
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
        public void OnMultiDropDownAssetChangeSetsTypeChange(ChangeEventArgs e, int key)
        {
            var index = lstAssetChangeSetsType.FindIndex(x => x.ID == key);
            var userRole = lstAssetChangeSetsType.Find(x => x.ID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (key == 0)
            {
                lstAssetChangeSetsType.ForEach(x => x.IsSelected = userRole.IsSelected);
            }
            else
            {
                var isAllSelectOrNot = lstAssetChangeSetsType.FindIndex(x => x.ID == 0);
                if (lstAssetChangeSetsType[isAllSelectOrNot].IsSelected == true)
                {
                    lstAssetChangeSetsType[isAllSelectOrNot].IsSelected = false;
                }
            }

            lstAssetChangeSetsType[index] = userRole;
            onChangeAssetChangeSetsTypeList(lstAssetChangeSetsType);
        }
        public void onChangeAssetChangeSetsTypeList(List<IMultiSelectDropDownList> assetTypesList)
        {
            assetChangeSetsTypeSelectedList = assetTypesList;
            selectAssetChangeSetsTypeList();
            loadAssetChangesList();
        }
        public void selectAssetChangeSetsTypeList()
        {
            selectedAssetChangeSetsTypeList = assetChangeSetsTypeSelectedList.FindAll(x => x.IsSelected == true);
            var selectAssetType = assetChangeSetsTypeSelectedList.Find(x => x.IsSelected == true && x.ID == 0);

            if (selectAssetType != null && selectAssetType.Value.ToUpper() == "All".ToUpper())
            {
                AssetChangeSetsType = "All";
            }
            else
            {
                AssetChangeSetsType = "NotAll";
            }
        }
        protected async Task<DataTable> loadAssetChangesList()
        {
            int assetid = 0;
            assetChangeSet = new List<ILT.IHR.DTO.AssetChangeSet> { };
            lstAssetChangeSet = assetChangeSet;
            var reponses = new Response<IEnumerable<DTO.AssetChangeSet>>();
            reponses = (await AssetChangesetService.GetAssetChangeset(assetid));
           // Response<IEnumerable<DTO.AssetChangeSet>> result = await AssetChangesetService.GetAssetChangeset(assetid);
            if (reponses.MessageType == MessageType.Success)
            {
                 assetChangeSet = reponses.Data.ToList();
                if (AssetChangeSetsType != "All" && selectedAssetChangeSetsTypeList != null && TicketChangeSetsStatus != "All" && selectedAssetChangeSetsStatusList != null)
                {
                    lstAssetChangeSet = assetChangeSet.Where(x => selectedAssetChangeSetsTypeList.Any(s => s.ID == x.AssetTypeID) && selectedAssetChangeSetsStatusList.Any(t => t.ID == x.StatusID)).ToList();
                }
                else if (AssetChangeSetsType == "All" && TicketChangeSetsStatus != "All")
                {
                    lstAssetChangeSet = assetChangeSet.Where(x => selectedAssetChangeSetsStatusList.Any(t => t.ID == x.StatusID)).ToList();
                }
                else if (AssetChangeSetsType != "All" && TicketChangeSetsStatus == "All")
                {
                    lstAssetChangeSet = assetChangeSet.Where(x => selectedAssetChangeSetsTypeList.Any(s => s.ID == x.AssetTypeID)).ToList();
                }
                else
                {
                    lstAssetChangeSet = assetChangeSet;
                }
            }
            string json = JsonConvert.SerializeObject(lstAssetChangeSet);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            return dt;

        }
        public class I9FormDays
        {
            public int Day { get; set; }
            public string text { get; set; }
        }
        public List<I9FormDays> I9FormDaysList { get; set; }
        protected int dayID { get; set; }
        public void loadDaysDropdown()
        {
            I9FormDays m1 = new I9FormDays
            {
                Day = 15,
                text = "15 days"
            };
            I9FormDays m2 = new I9FormDays
            {
                Day = 30,
                text = "30 days"
            };
            I9FormDays m3 = new I9FormDays
            {
                Day = 60,
                text = "60 days"
            };
            I9FormDays m4 = new I9FormDays
            {
                Day = 90,
                text = "90 days"
            };
            I9FormDays m5 = new I9FormDays
            {
                Day = 180,
                text = "180 days"
            };
            I9FormDaysList = new List<I9FormDays> { m1, m2, m3, m4, m5 };
            if(I9FormDaysList !=  null)
            {
                dayID = Convert.ToInt32(I9FormDaysList.Find(x => x.Day == 180).Day);
            }
        }
    }
}