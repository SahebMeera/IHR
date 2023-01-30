using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using Blazored.SessionStorage;
using ILT.IHR.UI.Service;
using BlazorInputFile;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Blazored.Toast.Services;
using Microsoft.JSInterop;
using BlazorDownloadFile;

namespace ILT.IHR.UI.Pages.Expenses
{
    public class AddEditExpenseBase : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public IConfiguration Configuration { get; set; } //Service  
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IExpenseService ExpenseService { get; set; } //Service 
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service
        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        [Parameter]
        public EventCallback<bool> ReloadGrid { get; set; }
        public Expense expense { get; set; }
        public List<DTO.Employee> EmployeeList { get; set; }
        public DTO.User user { get; set; }
        public string Title { get; set; }
        public bool ShowDialog { get; set; }
        public List<ListValue> ExpenseTypeList { get; set; }
        public List<ListValue> StatusList { get; set; }
        public List<Module> Modules { get; set; } //Drop Down Api Data
        protected int defaultExpenseTypeID { get; set; }
        protected int submittedStatusID { get; set; }
        protected int approveStatusID { get; set; }
        protected int denyStatusID { get; set; }
        protected bool IsApprover { get; set; }
        public bool isPaymentDateInValid { get; set; }
        public bool isPaymentAmountInValid { get; set; }
        public bool isPaymentCommnetInValid { get; set; }
        public Stream imageStream { get; set; }
        public string ImageFileName { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;
        public bool isApproveButtonDisabled { get; set; } = false;
        public bool isRejectButtonDisabled { get; set; } = false;

        //file
        protected IFileListEntry file;
        //protected IFileListEntry[] files;
        protected List<IFileListEntry> files;
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        protected string ExpenseEmail { get; set; }

        protected override async Task OnInitializedAsync()
        {
            files = null;
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            string RoleShort = await sessionStorage.GetItemAsync<string>("RoleShort");

            if (RoleShort.ToUpper() == UserRole.ADMIN || RoleShort.ToUpper() == UserRole.FINADMIN)
            {
                IsApprover = true;
            }
            await LoadDropDown();
        }
        protected async Task LoadDropDown()
        {
            ExpenseEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":Expense"];
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                ExpenseTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ExpenseType).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ExpenseStatus).ToList();
                defaultExpenseTypeID = ExpenseTypeList.Find(x => x.Value == "MISC").ListValueID;
                submittedStatusID = StatusList.Find(x => x.Value == "SUBMITTED").ListValueID;
                approveStatusID = StatusList.Find(x => x.Value == "APPROVED").ListValueID;
                denyStatusID = StatusList.Find(x => x.Value == "REJECTED").ListValueID;
            }
            var respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
            {
                EmployeeList = respEmployees.Data.ToList();
            }
            Modules = (await RoleService.GetModules()).ToList();
        }
        public void Show(int ExpenseID){
            file = null;
            ResetDialog();
            if(ExpenseID != 0)
            {
                Title = "Edit";
                GetExpense(ExpenseID);
            }
            else
            {
                Title = "Add";
                files = null;
                expense = new Expense { };
                expense.SubmissionDate = DateTime.Now;
                expense.EmployeeID = user.EmployeeID;
                expense.ExpenseTypeID = defaultExpenseTypeID;
                expense.StatusID = submittedStatusID;
                expense.FileName = "";
            }
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }
        public async Task GetExpense(int ExpenseID)
        {
            var resp = (await ExpenseService.GetExpenseByIdAsync(ExpenseID));
            if(resp.MessageType == MessageType.Success)
            {
                expense = resp.Data;
                StateHasChanged();
            }
        }
        public string Src { get; set; }
        protected async Task SaveExpense()
        {
            string fileName = "";
            //if (!string.IsNullOrEmpty(ImageFileName))
            //{
            //    var empName = EmployeeList.Find(x => x.EmployeeID == user.EmployeeID).EmployeeName;
            //    Src = await JSRuntime.InvokeAsync<string>("getFileDetails", ImageFileName);
            //    var bytes = Convert.FromBase64String(Src);// without data:image/jpeg;base64 prefix, just base64 string
            //    imageStream = new MemoryStream(bytes);
            //    fileName = await UploadFile(imageStream, empName);
            //    expense.FileName = fileName;
            //}
            if (EmployeeList != null)
            {
              string employeeName = EmployeeList.Find(x => x.EmployeeID == user.EmployeeID).EmployeeName;

                foreach (var file in files)
                {
                    fileName += await UploadFile(file, employeeName);
                    fileName += ",";
                }
                if (expense.FileName == null)
                {
                    expense.FileName = "";
                }
                expense.FileName += fileName.TrimEnd(',');

                if (isSaveButtonDisabled)
                    return;
                isSaveButtonDisabled = true;
                if (expense.ExpenseID == 0)
                {
                    expense.CreatedBy = user.FirstName + " " + user.LastName;
                    var result = await ExpenseService.SaveExpense(expense);
                    if (result.MessageType == MessageType.Success)
                    {
                        var expenseResp = await ExpenseService.GetExpenseByIdAsync(result.Data.RecordID) as Response<ILT.IHR.DTO.Expense>;

                        expense = expenseResp.Data;
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = Modules.Find(m => m.ModuleName.ToUpper() == "EXPENSES").ModuleID;
                        emailapproval.ID = expense.ExpenseID;
                        emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                        emailapproval.Value = expense.Status;
                        emailapproval.LinkID = Guid.NewGuid();
                        // emailapproval.ApproverEmail = EmployeeAssignment.TSApproverEmail;
                        emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                        EmailFields emailFields = await prepareExpensetMail();
                        emailapproval.EmailSubject = emailFields.EmailSubject;
                        emailapproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        emailapproval.EmailTo = emailFields.EmailTo;
                        emailapproval.EmailCC = emailFields.EmailCC;
                        emailapproval.IsActive = true;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("Expenses submitted successfully", "");
                        Close();
                        await ReloadGrid.InvokeAsync(true);
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                else
                {
                    expense.ModifiedBy = user.FirstName + " " + user.LastName;
                    var result = await ExpenseService.UpdateExpense(expense.ExpenseID, expense);
                    if (result.MessageType == MessageType.Success)
                    {
                        var expenseResp = await ExpenseService.GetExpenseByIdAsync(result.Data.RecordID) as Response<ILT.IHR.DTO.Expense>;
                        expense = expenseResp.Data;
                        EmailFields emailFields = new EmailFields();
                        Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                        if (expense.LinkID != Guid.Empty)
                        {
                            respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(expense.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                            emailapproval = respEmailApproval.Data;
                            emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                            emailapproval.SentCount = emailapproval.SentCount > 0 ? emailapproval.SentCount - 1 : 0;
                        }
                        else
                        {
                            emailapproval.EmailApprovalID = 0;
                            emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "EXPENSES").ModuleID;
                            emailapproval.ID = expense.ExpenseID;
                            emailapproval.IsActive = true;
                            emailapproval.LinkID = Guid.NewGuid();
                            // emailapproval.ApproverEmail = employeeDetails.ManagerEmail;
                            emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                        }
                        emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                        emailFields = await prepareUpdatedExpenseMail();
                        emailapproval.EmailSubject = emailFields.EmailSubject;
                        emailapproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        emailapproval.EmailTo = emailFields.EmailTo;
                        emailapproval.EmailCC = emailFields.EmailCC;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);

                        toastService.ShowSuccess("Expense saved successfully", "");
                        Close();
                        await ReloadGrid.InvokeAsync(true);
                    }
                }
            }
            isSaveButtonDisabled = false;
        }

        protected async Task Approve()
        {
            if (isApproveButtonDisabled)
                return;
            isApproveButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            if (expense != null)
            {
                if (isValidForApprove())
                {
                    expense.StatusID = approveStatusID;
                    expense.ModifiedBy = user.FirstName + " " + user.LastName;
                    var result = await ExpenseService.UpdateExpense(expense.ExpenseID, expense);
                    if (result.MessageType == MessageType.Success)
                    {
                        var expenseResp = await ExpenseService.GetExpenseByIdAsync(expense.ExpenseID) as Response<ILT.IHR.DTO.Expense>;
                        if (expenseResp.MessageType == MessageType.Success)
                        {
                            expense = expenseResp.Data;
                            Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                            DTO.EmailApproval emailApproval = new DTO.EmailApproval();
                            if (expense.LinkID != Guid.Empty)
                            {
                                respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(expense.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                                emailApproval = respEmailApproval.Data;
                                emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                                await EmailApprovalService.EamilApprovalAction(ClientID, expense.LinkID, "APPROVED", "EXPENSES");
                            }
                            else
                            {
                                emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "EXPENSES").ModuleID;
                                emailApproval.ID = expense.ExpenseID;
                                emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                                emailApproval.IsActive = true;
                                emailApproval.LinkID = Guid.NewGuid();
                                //emailApproval.ApproverEmail = await GetEmail(WFH.ApproverID);
                                emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                            }
                            EmailFields emailFields = await prepareApproveMail();
                            emailApproval.EmailApprovalID = 0;
                            emailApproval.LinkID = Guid.Empty;
                            emailApproval.EmailSubject = emailFields.EmailSubject;
                            emailApproval.EmailBody = emailFields.EmailBody;
                            emailApproval.EmailFrom = emailFields.EmailFrom;
                            emailApproval.EmailTo = emailFields.EmailTo;
                            emailApproval.Value = null;
                            emailApproval.IsActive = true;
                            await EmailApprovalService.SaveEmailApproval(emailApproval);
                            toastService.ShowSuccess("Expense Approved successfully", "");
                        }
                        else
                        {
                            toastService.ShowError(ErrorMsg.ERRORMSG);
                        }
                        Close();
                        ReloadGrid.InvokeAsync(true);
                    }
                }
                isApproveButtonDisabled = false;
            }
        }

        protected async Task Deny()
        {
            if (isRejectButtonDisabled)
                return;
            isRejectButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            if (expense != null)
            {
                if (isValidForDeny())
                {
                    expense.StatusID = denyStatusID;
                    expense.ModifiedBy = user.FirstName + " " + user.LastName;
                    var result = await ExpenseService.UpdateExpense(expense.ExpenseID, expense);
                    if (result.MessageType == MessageType.Success)
                    {
                        var expenseResp = await ExpenseService.GetExpenseByIdAsync(expense.ExpenseID) as Response<ILT.IHR.DTO.Expense>;
                        if (expenseResp.MessageType == MessageType.Success)
                        {
                            expense = expenseResp.Data;
                            Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                            DTO.EmailApproval emailApproval = new DTO.EmailApproval();
                            if (expense.LinkID != Guid.Empty)
                            {
                                respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(expense.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                                emailApproval = respEmailApproval.Data;
                                emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                                await EmailApprovalService.EamilApprovalAction(ClientID, expense.LinkID, "REJECTED", "EXPENSES");
                            }
                            else
                            {
                                emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "EXPENSES").ModuleID;
                                emailApproval.ID = expense.ExpenseID;
                                emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                                emailApproval.IsActive = true;
                                emailApproval.LinkID = Guid.NewGuid();
                                emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                            }
                            EmailFields emailFields = await prepareDenyMail();
                            emailApproval.EmailApprovalID = 0;
                            emailApproval.LinkID = Guid.Empty;
                            emailApproval.EmailSubject = emailFields.EmailSubject;
                            emailApproval.EmailBody = emailFields.EmailBody;
                            emailApproval.EmailFrom = emailFields.EmailFrom;
                            emailApproval.EmailTo = emailFields.EmailTo;
                            emailApproval.Value = null;
                            emailApproval.IsActive = true;
                            await EmailApprovalService.SaveEmailApproval(emailApproval);
                            toastService.ShowSuccess("Expense Denied successfully", "");
                        }
                        else
                        {
                            toastService.ShowError(ErrorMsg.ERRORMSG);
                        }
                        Close();
                        ReloadGrid.InvokeAsync(true);
                    }
                }
                isRejectButtonDisabled = false;
            }
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        private void ResetDialog()
        {
            expense = new Expense { };
        }

        //image Capture and retrive functionality
        // public string GetBlobContentPath(string fileName)
        //{
        //    string baseUri = Configuration["ApiUrl"];
        //    return baseUri + "Expense/DownloadImage?Client=" + user.ClientID+ "&FileName=" + fileName;
        //}

        public async Task GetBlobContentPath(string fileName)
        {
            var fileDownLoadresp = await ExpenseService.DownloadFile(user.ClientID, fileName);
            if (String.IsNullOrEmpty(fileDownLoadresp.Data.ErrorMessage))
            {
                var task = await BlazorDownloadFileService.DownloadFile(fileDownLoadresp.Data.FileName, fileDownLoadresp.Data.memoryStream, "application/octet-stream");
                if (task.Succeeded)
                {
                    toastService.ShowSuccess("Successful download!");
                }
                else
                {
                    toastService.ShowError(task.ErrorMessage);
                }
            }
            else
            {
                toastService.ShowError(fileDownLoadresp.Data.ErrorMessage);
            }
        }
        //private async Task<string> UploadFile(Stream fileStream, string employeeName)
        //{
        //    if (employeeName == null)
        //        employeeName = "";
        //    string fileName = "";
        //    if (fileStream != null)
        //    {
        //        BlobServiceClient _blobServiceClient;
        //        string blobContainerName = Configuration["ReimbursementBlobContainer:" + this.user.ClientID.ToUpper()];
        //        Stream content = fileStream; // System.IO.File.OpenRead(filePath); //From the OpenDialog box                
        //        string fileExtension = Path.GetExtension(".jpeg");
        //        employeeName = employeeName.Replace(" ", "");
        //        fileName = "EXP" + (employeeName.Length <= 15 ? employeeName : employeeName.Substring(0, 15)) + DateTime.Now.ToString("yyyyMMddss") + fileExtension;
        //        string contentType = content.GetType().ToString();
        //        _blobServiceClient = new BlobServiceClient(Configuration["AzureBlobConnectionString"]); //Azure ConnectionString
        //        var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        //        var blobClient = containerClient.GetBlobClient(fileName);
        //        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        //    }
        //    return fileName;
        //}

        public void HandleFileSelected(IFileListEntry[] fileList)
        {
            // files = new List<IFileListEntry>();
            files = fileList.ToList();
            fileList = null;
            // file = files.FirstOrDefault();
        }

        private async Task<string> UploadFile(IFileListEntry file, string employeeName)
        {
            if (employeeName == null)
                employeeName = "";
            string fileName = "";
            string filePath = file.Name;
            if (filePath != "")
            {
                BlobServiceClient _blobServiceClient;
                string blobContainerName = Configuration["ReimbursementBlobContainer:" + this.user.ClientID.ToUpper()];
                Stream content = file.Data; // System.IO.File.OpenRead(filePath); //From the OpenDialog box
                string fileExtension = Path.GetExtension(filePath);
                employeeName = employeeName.Replace(" ", "");
                fileName = "EXP" + (employeeName.Length <= 15 ? employeeName : employeeName.Substring(0, 15)) + DateTime.Now.ToString("yyyyMMddss") + fileExtension;
                string contentType = content.GetType().ToString();

                _blobServiceClient = new BlobServiceClient(Configuration["AzureBlobConnectionString"]); //Azure ConnectionString
                var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
            }
            return fileName;
        }

        public void removeAttachment()
        {
            files.RemoveAt(0);
             // this.files.Except(new IFileListEntry[] { 0 }).ToArray();
            // ImageFileName = "";
            //imageStream = null;
        }
        protected async Task onFileChange()
        {
            ImageFileName = await JSRuntime.InvokeAsync<string>("process");
        }

        protected bool isValidForApprove()
        {
            isPaymentDateInValid = false;
            isPaymentAmountInValid = false;
            isPaymentCommnetInValid = false;
            if (expense.PaymentDate == null && expense.AmountPaid == null)
            {
                isPaymentAmountInValid = true;
                isPaymentDateInValid = true;
                return false;
            }
            else if (expense.PaymentDate == null && expense.AmountPaid != null)
            {
                isPaymentDateInValid = true;
                isPaymentAmountInValid = false;
                return false;
            }
            else if (expense.PaymentDate != null && expense.AmountPaid == null)
            {
                isPaymentAmountInValid = true;
                isPaymentDateInValid = false;
                return false;
            }
            else if(expense.PaymentComment == null || expense.PaymentComment.Trim() == "")
            {
                isPaymentCommnetInValid = true;
                return false;
            }
            return true;
        }
        protected bool isValidForDeny()
        {
            isPaymentCommnetInValid = false;
            if (expense.PaymentComment == null || expense.PaymentComment.Trim() == "")
            {
                isPaymentCommnetInValid = true;
                return false;
            }

            return true;
        }

        protected void onPaymentDateChange()
        {
            var paymentDate = expense.PaymentDate;
            if (paymentDate != null)
            {
                isPaymentDateInValid = false;
            }
            else
            {
                isPaymentDateInValid = true;
            }
        }
        protected void onPaymentAmountChange()
        {
            var paymentAmount = expense.AmountPaid;
            if (paymentAmount != null)
            {
                isPaymentAmountInValid = false;
            }
            else
            {
                isPaymentAmountInValid = true;
            }
        }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }

        protected async Task onExpensesChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && ExpenseTypeList != null)
            {
                var expenseType = Convert.ToInt32(e.Value);
                expense.ExpenseType = ExpenseTypeList.Find(x => x.ListValueID == expenseType).ValueDesc;
            }
        }

        public async Task<EmailFields> prepareExpensetMail()
        {
            EmailFields emailFields = new EmailFields();
            string RequesterEmail = await GetEmail(expense.EmployeeID);
            emailFields.EmailTo = ExpenseEmail;
            emailFields.EmailCC = RequesterEmail;
            emailFields.EmailSubject = "Expense submitted for " + expense.EmployeeName;
            emailFields.EmailBody = "Expense #" + expense.ExpenseID + " has been submitted " +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + expense.ExpenseType + "<br/>" +
                "</li><li>Requester: " + expense.EmployeeName +
                "</li><li>Amount: " + expense.Amount +
                "</li><li>Description: " + expense.SubmissionComment +
                "</li><li>Submitted Date: " + FormatDate(expense.CreatedDate) +
                "</li></li></ul>";
            return emailFields;
        }

        public async Task<EmailFields> prepareUpdatedExpenseMail()
        {
            EmailFields emailFields = new EmailFields();
            string RequesterEmail = await GetEmail(expense.EmployeeID);
            emailFields.EmailTo = ExpenseEmail;
            emailFields.EmailCC = RequesterEmail;
            emailFields.EmailSubject = "Expense submitted for "+ expense.EmployeeName ;
            emailFields.EmailBody = "Expense #" + expense.ExpenseID + " has been submitted " +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + expense.ExpenseType + "<br/>" +
                "</li><li>Requester: " + expense.EmployeeName +
                "</li><li>Amount: " + expense.Amount +
                "</li><li>Description: " + expense.SubmissionComment +
                "</li><li>Submitted Date: " + FormatDate(expense.CreatedDate) +
                "</li><li>Updated Date: " + FormatDate(expense.ModifiedDate) +
                "</li></li></ul>";
            return emailFields;
        }

        public async Task<EmailFields> prepareApproveMail()
        {
            EmailFields emailFields = new EmailFields();
            string RequesterEmail = await GetEmail(expense.EmployeeID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailSubject = "Expense " + expense.Status.ToLower() + " for " + expense.EmployeeName ;
            emailFields.EmailBody = "Expense #" + expense.ExpenseID + " has been " + expense.Status +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + expense.ExpenseType + "<br/>" +
                "</li><li>Requester: " + expense.EmployeeName +
                "</li><li>Amount: " + expense.Amount +
                "</li><li>Description: " + expense.SubmissionComment +
                "</li><li>Payment Date: " + FormatDate(expense.PaymentDate) +
                "</li><li>Payment Comment: " + expense.PaymentComment +
                "</li><li>Amount Paid: " + expense.AmountPaid +
                "</li><li>Submitted Date: " + FormatDate(expense.CreatedDate) +
                "</li>" +
                "</ul>";
            return emailFields;
        }

        public async Task<EmailFields> prepareDenyMail()
        {
            EmailFields emailFields = new EmailFields();
            string RequesterEmail = await GetEmail(expense.EmployeeID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailSubject = "Expense " + expense.Status.ToLower() + " for " + expense.EmployeeName;
            emailFields.EmailBody = "Expense #" + expense.ExpenseID + " has been " + expense.Status +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + expense.ExpenseType + "<br/>" +
                "</li><li>Requester: " + expense.EmployeeName +
                "</li><li>Amount: " + expense.Amount +
                "</li><li>Description: " + expense.SubmissionComment +
                "</li><li>Payment Comment: " + expense.PaymentComment +
                "</li><li>Submitted Date: " + FormatDate(expense.CreatedDate) +
                "</li>" +
                "</ul>";
            return emailFields;
        }

        protected async Task<string> GetEmail(int? EmployeeID)
        {
            DTO.Employee emp = EmployeeList.ToList().Find(x => x.EmployeeID == EmployeeID);
            return !string.IsNullOrEmpty(emp.LoginEmail) ? emp.LoginEmail : emp.Email;
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dd MMM yyy HH:mm:ss") + " GMT";
                formattedDate = date;
            }
            return formattedDate;
        }
    }
}
