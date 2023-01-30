using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILT.IHR.Factory
{
    public abstract class AbstractFactory1
    {
        // private bool isInTransaction = false;
        // private ArrayList _auditTrailFields = new ArrayList();
        // protected System.Data.SqlClient.SqlDataReader dr;
        // protected System.Data.DataSet ds;
        // protected QueryType queryType = QueryType.StoredProcedure;
        // private SQLServer sqlServerConnObj;
        // protected string sqlQuery;
        // protected TransactionManager transactionManager;
        // protected AbstractDataObject _originalObject;
        // protected bool IsInTransaction
        // {
        //     get { return isInTransaction; }
        //     set { isInTransaction = value; }
        // }

        private string connectionString = string.Empty;
        protected string getStoredProc;
        string insAuditLogProc = "usp_InsAuditLog";
        string insEmailApproavlProc = "usp_InsUpdEmailApproval";
        protected string selectStoredProc { get; set; }
        protected DynamicParameters parms;
        protected CommandType commandType = CommandType.StoredProcedure;
        SqlMapper.GridReader gridReader;
        private IConfiguration _config;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IDbConnection connection
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }

        public AbstractFactory1(string ConnectionString, IConfiguration config)
        {
            this.connectionString = ConnectionString;
            parms = new DynamicParameters();
            _config = config;
        }

        public abstract Response<List<T>> GetList<T>(T obj);
        public abstract Response<T> GetByID<T>(T obj);
        public abstract Response<T> GetRelatedObjectsByID<T>(T obj);
        public abstract Response<T> Save<T>(T obj);
        public abstract Response<T> Delete<T>(T obj);
        protected abstract T LoadRelatedObjects<T>(SqlMapper.GridReader reader);

        protected Response<List<T>> GetList<T>()
        {
            Response<List<T>> response = new Response<List<T>>();
            try
            {
                using (IDbConnection db = connection)
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    response.Data = db.Query<T>(this.getStoredProc, this.parms, commandType: this.commandType).ToList();
                    response.MessageType = MessageType.Success;

                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                    ex.Message,
                    ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
            }

            return response;
        }

        protected Response<T> GetByID<T>()
        {
            Response<T> response = new Response<T>();
            try
            {
                using (IDbConnection db = connection)
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    response.Data = db.Query<T>(this.getStoredProc, this.parms, commandType: this.commandType).FirstOrDefault();

                    response.MessageType = MessageType.Success;

                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}",ex.Message, ex.ToString());
            }
            return response;
        }

        private Response<T> GetByID<T>(int Id)
        {
            Response<T> response = new Response<T>();
            try
            {
                using (IDbConnection db = connection)
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var paramName = this.GetType().Name.Replace("Factory", "ID");
                    paramName = "@"+paramName;
                    //SqlParameter sqlParam1 = new SqlParameter(paramName, Id);

                    var parameters = new DynamicParameters();
                    parameters.Add("@"+paramName, Id);

                    response.Data = db.Query<T>(this.selectStoredProc, parameters, commandType: this.commandType).FirstOrDefault();

                    response.MessageType = MessageType.Success;

                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
            }
            return response;
        }
        public void SaveAuditLogEntries<T>(T objCurrent, T objBefore)
        {
            this.parms = new DynamicParameters();

            string tableName = "";
            string action = "";
            string createdBy = "";
            int recordId = 0;

            StringBuilder values = new StringBuilder();
            string formatedValues = string.Empty;

            if (objCurrent != null) tableName = objCurrent.GetType().Name;
            if (objBefore != null) tableName = objBefore.GetType().Name;

            if (objCurrent != null && objBefore != null) //update
            {
                Type typeCurrent = objCurrent.GetType();
                PropertyInfo[] propsCurrent1 = typeCurrent.GetProperties();
                System.Collections.Generic.List<T> list;

                PropertyInfo[] propsCurrent = typeCurrent.GetProperties()
                    .Where(a => a.PropertyType.FullName.Contains("List") == false 
                    && a.Name.Contains("Password") == false
                    && a.Name.Contains("CreatedBy") == false
                    && a.Name.Contains("CreatedDate") == false
                    && a.Name.Contains("ModifiedDate") == false).ToArray();

                recordId = (objCurrent as AbstractDataObject).RecordID;
                action = "Update";
                Type typeBefore = objBefore.GetType();
                PropertyInfo[] propsBefore = typeBefore.GetProperties();

                foreach (var propCurrent in propsCurrent)
                {
                    var propBeforeVal = propsBefore.Where(x => x.Name == propCurrent.Name).FirstOrDefault().GetValue(objBefore);
                    if (propBeforeVal == null) propBeforeVal = "";
                    var propCurrentVal = propCurrent.GetValue(objCurrent);
                    if (propCurrentVal == null) propCurrentVal = "";

                    if (propCurrent.Name == "ModifiedBy")
                    {
                        createdBy = propCurrentVal.ToString();
                    }
                    else if (propCurrentVal.ToString() != propBeforeVal.ToString())
                    {
                        //if ((propCurrent.Name.EndsWith("ID") || propCurrent.Name.EndsWith("Id")) == false)
                        values.Append(string.Format("{0}:{1},{2} | ", propCurrent.Name, propBeforeVal.ToString(), propCurrentVal.ToString()));
                    }
                }

                if (values.Length > 0)
                    formatedValues = values.ToString().Remove(values.Length - 2, 2);

            }
            else if (objCurrent == null && objBefore != null) //delete
            {
                action = "Delete";
                Type typeBefore = objBefore.GetType();
                PropertyInfo[] propsBefore = typeBefore.GetProperties().Where(a => a.PropertyType.FullName.Contains("List") == false).ToArray(); 
                
                recordId = (objBefore as AbstractDataObject).RecordID;

                foreach (var propBefore in propsBefore)
                {
                    var propBeforeVal = propBefore.GetValue(objBefore);
                    if (propBeforeVal == null) propBeforeVal = "";


                    if (propBefore.Name == "ModifiedBy")
                    {
                        createdBy = propBeforeVal.ToString();
                    }

                    //if ((propCurrent.Name.EndsWith("ID") || propCurrent.Name.EndsWith("Id")) == false)
                    values.Append(string.Format("{0}:{1} | ", propBefore.Name, propBeforeVal.ToString()));
                }

                if (values.Length > 0)
                    formatedValues = values.ToString().Remove(values.Length - 2, 2);
            }


            if (action.ToLower() != "update" && action.ToLower() != "delete")
            {
                return;
            }

            if (formatedValues.Length > 0)
                LogAudit(action, tableName, recordId, createdBy, formatedValues);
        }


        public void SaveEmailNotification<T>(T objCurrent, T objBefore)
        {
            this.parms = new DynamicParameters();
            EmailApproval emailapproval = new EmailApproval();
            List<string> propChanges = new List<string>();

            string tableName = "";
            string action = "";
            string createdBy = "";
            int recordId = 0;


            if (objCurrent != null) tableName = objCurrent.GetType().Name;
            if (objBefore != null) tableName = objBefore.GetType().Name;

            if (objCurrent != null && objBefore != null) //update
            {
                Type typeCurrent = objCurrent.GetType();

                PropertyInfo[] propsCurrent = typeCurrent.GetProperties()
                    .Where(a => a.PropertyType.FullName.Contains("List") == false
                    && a.Name.Contains("Password") == false
                    && a.Name.Contains("CreatedBy") == false
                    && a.Name.Contains("CreatedDate") == false
                    && a.Name.Contains("ModifiedDate") == false
                    && a.Name.Contains("ID") == false
                    && a.Name.Contains("IsDeleted") == false
                    && a.Name.Contains("HasChange") == false
                    && a.Name.Contains("MyProperty") == false
                    && a.Name.Contains("ModifiedBy") == false
                    && a.Name.Contains("TimeStamp") == false).ToArray();

                recordId = (objCurrent as AbstractDataObject).RecordID;
                action = "Update";

                Type typeBefore = objBefore.GetType();
                PropertyInfo[] propsBefore = typeBefore.GetProperties();
                foreach (var propCurrent in propsCurrent)
                {
                    var propBeforeVal = propsBefore.Where(x => x.Name == propCurrent.Name).FirstOrDefault().GetValue(objBefore);
                    if (propBeforeVal == null) 
                    { 
                        propBeforeVal = ""; 
                    }
                    var propCurrentVal = propCurrent.GetValue(objCurrent);
                    if (propCurrentVal == null) 
                    { 
                        propCurrentVal = ""; 
                    }

                    if (propBeforeVal.ToString() != propCurrentVal.ToString() && propCurrent.Name.ToLower() != "employeename")
                    {
                        if(propCurrent.Name.ToLower() == "ssn")
                        {
                            if(propBeforeVal != "" && propBeforeVal != null)
                            {
                                propBeforeVal = string.Format("{0:***-**-0000}", Convert.ToInt64(propBeforeVal.ToString().Substring(propBeforeVal.ToString().Length - 4, 4))); ;
                            }
                            if (propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal =  string.Format("{0:***-**-0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        }
                        if (propCurrent.Name.ToLower() == "pan")
                        {
                            if (propBeforeVal != "" && propBeforeVal != null)
                            {
                                propBeforeVal = String.Format("{0:******0000}", Convert.ToInt64(propBeforeVal.ToString().Substring(propBeforeVal.ToString().Length - 4, 4)));
                            }
                            if (propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal = String.Format("{0:******0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        }
                        if (propCurrent.Name.ToLower() == "aadharnumber")
                        {
                            if (propBeforeVal != "" && propBeforeVal != null)
                            {
                                propBeforeVal = String.Format("{0:********0000}", Convert.ToInt64(propBeforeVal.ToString().Substring(propBeforeVal.ToString().Length - 4, 4)));
                            }
                            if (propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal = String.Format("{0:********0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        }
                        propChanges.Add(propCurrent.Name + "," + propBeforeVal + "," + propCurrentVal);
                    }

                    if (propCurrent.Name == "ModifiedBy")
                    {
                        createdBy = propCurrentVal.ToString();
                    }
                }

                string emailSubject = tableName;
                if (tableName.ToUpper() == "EMPLOYEE")
                {
                    emailSubject += " updated: ";
                    emailSubject += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                }
                else if ((tableName.ToUpper() == "EMPLOYEEADDRESS" ||
                    tableName.ToUpper() == "DEPENDENT" || tableName.ToUpper() == "SALARY" || tableName.ToUpper() == "ASSIGNMENT"
                    || tableName.ToUpper() == "DEPARTMENT" || tableName.ToUpper() == "EMPLOYEEW4" || tableName.ToUpper() == "FORMI9"
                    || tableName.ToUpper() == "CONTACT"))
                {
                    emailSubject += " updated for ";
                    emailSubject += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                }
                else if (tableName.ToUpper() == "COMPANY")
                {
                    emailSubject += " updated: ";
                    emailSubject += propsCurrent.Where(x => x.Name == "Name").FirstOrDefault().GetValue(objCurrent);
                }

                EmailFields emailFields = new EmailFields();
                emailFields.EmailSubject = emailSubject;

                if (tableName.ToUpper() == "EMPLOYEE")
                {
                    emailFields.EmailBody += "<h5>" + tableName + " ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been updated as below</h5>";
                }
                else if ((tableName.ToUpper() == "EMPLOYEEADDRESS" ||
                    tableName.ToUpper() == "DEPENDENT" || tableName.ToUpper() == "SALARY" || tableName.ToUpper() == "ASSIGNMENT"
                    || tableName.ToUpper() == "DEPARTMENT" || tableName.ToUpper() == "EMPLOYEEW4" || tableName.ToUpper() == "FORMI9"
                    || tableName.ToUpper() == "CONTACT"))
                {
                    emailFields.EmailBody += "<h5>" + tableName + " for ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been updated as below</h5>";
                }
                else if (tableName.ToUpper() == "COMPANY")
                {
                    emailFields.EmailBody += "<h5>" + tableName + " ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "Name").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been updated as below</h5>";
                }


                emailFields.EmailBody += "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>";
                emailFields.EmailBody += "<tr><th style='border: 1px solid black;'></th><th style='border: 1px solid black;'>Previous Value</th>" +
                    "<th style='border: 1px solid black;'>Current Value</th></tr>";
                foreach (var changes in propChanges)
                {
                   string[] changesArr= changes.Split(',').ToArray();
                    emailFields.EmailBody += "<tr><td style='border: 1px solid black;' width='30%'>&nbsp;<b>" + changesArr[0]  + "</b></td>"+
                        "<td style='border: 1px solid black;' width='35%'>&nbsp;" + changesArr[1] + "</td>" +
                        "<td style='border: 1px solid black;' width='35%'>&nbsp;" + changesArr[2] + "</td></tr>";
                }
                emailFields.EmailBody += "</table>";
                emailFields.EmailFrom = _config["EmailNotifications:" + GetClientID(connectionString) + ":FromEmail"];
                emailFields.EmailTo = _config["EmailNotifications:" + GetClientID(connectionString) + ":ChangeNotification"];
                emailFields.EmailTo = emailFields.EmailTo.Replace(',', ';');
                emailapproval.EmailApprovalID = 0;
                emailapproval.ModuleID = 0;
                emailapproval.ID = recordId;
                emailapproval.ValidTime = DateTime.Now.AddDays(_config["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(_config["EmailApprovalValidity"]));
                emailapproval.LinkID = Guid.Empty;
                emailapproval.CreatedBy = createdBy;
                emailapproval.EmailSubject = emailFields.EmailSubject;
                emailapproval.EmailBody = emailFields.EmailBody;
                emailapproval.EmailFrom = emailFields.EmailFrom;
                emailapproval.EmailTo = emailFields.EmailTo;
                emailapproval.IsActive = true;
            }

            if (objCurrent != null && objBefore == null) // create
            {
                action = "create";
                Type typeCurrent = objCurrent.GetType();

                PropertyInfo[] propsCurrent = typeCurrent.GetProperties()
                    .Where(a => a.PropertyType.FullName.Contains("List") == false
                    && a.Name.Contains("Password") == false
                    && a.Name.Contains("CreatedBy") == false
                    && a.Name.Contains("CreatedDate") == false
                    && a.Name.Contains("ModifiedDate") == false
                    && a.Name.Contains("ID") == false
                    && a.Name.Contains("IsDeleted") == false
                    && a.Name.Contains("HasChange") == false
                    && a.Name.Contains("MyProperty") == false
                    && a.Name.Contains("ModifiedBy") == false
                    && a.Name.Contains("TimeStamp") == false).ToArray();

                foreach (var propCurrent in propsCurrent)
                {
                    var propCurrentVal = propCurrent.GetValue(objCurrent);
                    if (propCurrentVal == null)
                    {
                        propCurrentVal = "";
                    }
                    if (propCurrent.Name.ToLower() != "employeename")
                    {
                        if(propCurrent.Name.ToLower() == "ssn")
                        {
                            if(propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal = string.Format("{0:***-**-0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        } 
                        if(propCurrent.Name.ToLower() == "pan")
                        {
                            if(propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal = string.Format("{0:******0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        } 
                        if(propCurrent.Name.ToLower() == "aadharnumber")
                        {
                            if(propCurrentVal != "" && propCurrentVal != null)
                            {
                                propCurrentVal = string.Format("{0:********0000}", Convert.ToInt64(propCurrentVal.ToString().Substring(propCurrentVal.ToString().Length - 4, 4)));
                            }
                        }
                        propChanges.Add(propCurrent.Name + "," + propCurrentVal);
                    }
                       
                }

                string emailSubject = tableName;
                if (tableName.ToUpper() == "EMPLOYEE")
                {
                    emailSubject += " added: ";
                    emailSubject += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                }
                else if (tableName.ToUpper() == "EMPLOYEEADDRESS" ||
                    tableName.ToUpper() == "DEPENDENT" || tableName.ToUpper() == "SALARY" || tableName.ToUpper() == "ASSIGNMENT"
                    || tableName.ToUpper() == "DEPARTMENT" || tableName.ToUpper() == "EMPLOYEEW4" || tableName.ToUpper() == "FORMI9"
                    || tableName.ToUpper() == "CONTACT")
                {
                    emailSubject += " added for ";
                    emailSubject += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                }
                else if (tableName.ToUpper() == "COMPANY")
                {
                    emailSubject += " added: ";
                    emailSubject += propsCurrent.Where(x => x.Name == "Name").FirstOrDefault().GetValue(objCurrent);
                }

                EmailFields emailFields = new EmailFields();
                emailFields.EmailSubject = emailSubject;
                // emailFields.EmailBody = "<h5>" + tableName + " has been created as below</h5><br>";

                if (tableName.ToUpper() == "EMPLOYEE")
                {
                    emailFields.EmailBody += "<h5>" + tableName + " ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been added as below</h5>";
                }
                else if ((tableName.ToUpper() == "EMPLOYEEADDRESS" ||
                    tableName.ToUpper() == "DEPENDENT" || tableName.ToUpper() == "SALARY" || tableName.ToUpper() == "ASSIGNMENT"
                    || tableName.ToUpper() == "DEPARTMENT" || tableName.ToUpper() == "EMPLOYEEW4" || tableName.ToUpper() == "FORMI9"
                    || tableName.ToUpper() == "CONTACT"))
                {
                    emailFields.EmailBody += "<h5>" + tableName + " for ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "EmployeeName").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been added as below</h5>";
                }
                else if (tableName.ToUpper() == "COMPANY")
                {
                    emailFields.EmailBody += "<h5>" + tableName + " ";
                    emailFields.EmailBody += propsCurrent.Where(x => x.Name == "Name").FirstOrDefault().GetValue(objCurrent);
                    emailFields.EmailBody += " has been added as below</h5>";
                }


                emailFields.EmailBody += "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>";
                emailFields.EmailBody += "<tr><th style='border: 1px solid black;'></th><th style='border: 1px solid black;'>Current Value</th></tr>"; 
                foreach (var changes in propChanges)
                {
                    string[] changesArr = changes.Split(',').ToArray();
                    emailFields.EmailBody += "<tr><td style='border: 1px solid black;' width='30%'>&nbsp;<b>" + changesArr[0] + "</b></td>" +
                        "<td style='border: 1px solid black;' width='50%'>&nbsp;" + changesArr[1] + "</td></tr>";
                }
                emailFields.EmailBody += "</table>";
                emailFields.EmailFrom = _config["EmailNotifications:" + GetClientID(connectionString) + ":FromEmail"];
                emailFields.EmailTo = _config["EmailNotifications:" + GetClientID(connectionString) + ":ChangeNotification"];
                emailFields.EmailTo = emailFields.EmailTo.Replace(',', ';');
                emailapproval.EmailApprovalID = 0;
                emailapproval.ModuleID = 0;
                emailapproval.ID = 0;
                emailapproval.ValidTime = DateTime.Now.AddDays(_config["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(_config["EmailApprovalValidity"]));
                emailapproval.LinkID = Guid.Empty;
                emailapproval.CreatedBy = createdBy;
                emailapproval.EmailSubject = emailFields.EmailSubject;
                emailapproval.EmailBody = emailFields.EmailBody;
                emailapproval.EmailFrom = emailFields.EmailFrom;
                emailapproval.EmailTo = emailFields.EmailTo;
                emailapproval.IsActive = true;
            }

            if ((action.ToLower() == "update" || action.ToLower() == "create") && propChanges.Count() > 0)
            {
                saveNotifications(emailapproval);
                TriggerWebJob();
                //return;
            }


            //if (propChanges.Count > 0)
            //{
            //    saveNotifications(emailapproval);
            //    TriggerWebJob();
            //}
        }

        private async void TriggerWebJob()
        {
            string ApiBaseAddress = _config["AzureWebJobSettings:ApiBaseAddress"];
            string UserId = _config["AzureWebJobSettings:UserId"];
            string Password = _config["AzureWebJobSettings:Password"];
            string WebJobPath = _config["AzureWebJobSettings:WebJobPath"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseAddress);
            var byteArray = Encoding.ASCII.GetBytes(UserId + ":" + Password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var responseData = await client.PostAsync(WebJobPath + "run", null);
        }

        private string GetClientID(string conString)
        {
            List<string> conList  = conString.Split(';').ToList<string>();
            string intialCatalog = conList.Find(x => x.Contains("Initial Catalog"));
            string clientID = intialCatalog.Split('=').ToArray()[1].Trim(' ').Split("_").ToArray()[1].Trim(' ');
            return clientID;
        }

        public void LogAudit(string action, string tableName, int recordId ,string createdBy, string values)
        {
            this.parms = new DynamicParameters();
            AuditLog auditLog = new AuditLog
            {
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                CreatedBy = createdBy,
                Values = values
            };

            this.parms.Add("@Action", auditLog.Action);
            this.parms.Add("@TableName", auditLog.TableName);
            this.parms.Add("@RecordId", auditLog.RecordId);
            this.parms.Add("@CreatedBy", auditLog.CreatedBy);
            this.parms.Add("@Values", auditLog.Values);

           
            using (IDbConnection db = connection)
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            db.Query<int>(this.insAuditLogProc, this.parms, commandType: this.commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                           
                        }
                        catch (SqlException ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Number + ":" + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
        }

        public void saveNotifications(EmailApproval emailApproval)
        {
            this.parms = new DynamicParameters();

            this.parms.Add("@EmailApprovalID", emailApproval.EmailApprovalID);
            this.parms.Add("@ModuleID", emailApproval.ModuleID);
            this.parms.Add("@ID", emailApproval.ID);
            // this.parms.Add("@UserID", obj1.UserID);
            this.parms.Add("@ValidTime", emailApproval.ValidTime);
            this.parms.Add("@IsActive", emailApproval.IsActive);
            this.parms.Add("@Value", emailApproval.Value);
            this.parms.Add("@LinkID", emailApproval.LinkID);
            this.parms.Add("@ApproverEmail", emailApproval.ApproverEmail);
            this.parms.Add("@EmailSubject", emailApproval.EmailSubject);
            this.parms.Add("@EmailBody", emailApproval.EmailBody);
            this.parms.Add("@EmailFrom", emailApproval.EmailFrom);
            this.parms.Add("@EmailTo", emailApproval.EmailTo);
            this.parms.Add("@EmailCC", emailApproval.EmailCC);
            this.parms.Add("@EmailBCC", emailApproval.EmailBCC);
            this.parms.Add("@CreatedBy", emailApproval.CreatedBy);
            this.parms.Add("@ModifiedBy", emailApproval.ModifiedBy);
            this.parms.Add("@ReturnCode", emailApproval.RecordID, direction: ParameterDirection.Output, size: sizeof(int));


            using (IDbConnection db = connection)
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            db.Query<int>(this.insEmailApproavlProc, this.parms, commandType: this.commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                        }
                        catch (SqlException ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Number + ":" + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
        }

        public Response<T> SaveInstanceReturnOutput<T>(T obj)
        {
            bool isAuditEableForCurrentObject = false;
            bool isNotificationForCurrentObject = false;

            string[] auditTableList = _config["AuditTables"] != null ? _config["AuditTables"].Split(',') : Array.Empty<string>();
            string[] NotificationTableList = _config["NotificationTables"] != null ? _config["NotificationTables"].Split(',') : Array.Empty<string>();

            string tableName = obj.GetType().Name;

            if (auditTableList.Contains(tableName))
            {
                isAuditEableForCurrentObject = true;
            }

            if (NotificationTableList.Contains(tableName))
            {
                isNotificationForCurrentObject = true;
            }

            Response<T> response = new Response<T>();
            Response<T> beforeResponse = new Response<T>();

            if (isAuditEableForCurrentObject || isNotificationForCurrentObject)
            {
                int Id = (obj as AbstractDataObject).RecordID;
            
                if (Id > 0)
                {
                    beforeResponse = this.GetByID<T>(Id);
                }
            }
           

            using (IDbConnection db = connection)
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            response.Data = obj;
                            db.Query<int>(this.getStoredProc, this.parms, commandType: this.commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                            (response.Data as AbstractDataObject).RecordID = parms.Get<int>("ReturnCode");
                            response.MessageType = MessageType.Success;
                                                       
                        }
                        catch (SqlException ex)
                        {
                            log.Error(string.Format("Error: {0}, Stack Trace: {1}", ex.Message, ex.ToString()));
                            tran.Rollback();
                            throw new Exception(ex.Number + ":" + ex.Message);
                        }
                       
                    }
                    if (isAuditEableForCurrentObject)
                        this.SaveAuditLogEntries<T>(obj, beforeResponse.Data);
                    if (isNotificationForCurrentObject)
                        this.SaveEmailNotification(obj, beforeResponse.Data);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error: {0}, Stack Trace: {1}", ex.Message, ex.ToString()));

                    response.MessageType = MessageType.Error;
                    response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            return response;
        }

        protected Response<T> DeleteInstance<T>()
        {
            bool isAuditEableForCurrentObject = false;
            string[] auditTableList = _config["AuditTables"].Split(',');
            string tableName = this.GetType().Name.Replace("Factory", "");
            Response<T> beforeResponse = new Response<T>();
            Response<T> response = new Response<T>();

            if (auditTableList.Contains(tableName))
            {
                isAuditEableForCurrentObject = true;
                var paramName = this.GetType().Name.Replace("Factory", "Id");
                var paramValue = this.parms.Get<int>("@" + paramName);
                beforeResponse = this.GetByID<T>(paramValue);
            }

            try
            {
                using (IDbConnection db = connection)
                {
                   db.Query<T>(this.getStoredProc, this.parms, commandType: this.commandType);
                   response.MessageType = MessageType.Success;
                }

                if (isAuditEableForCurrentObject)
                {
                    Response<T> currentResponse = new Response<T>();
                    this.SaveAuditLogEntries<T>(currentResponse.Data, beforeResponse.Data);
                }
                
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
            }

            return response;
        }
        
        protected Response<T> GetRelatedObjectsByID<T>()
        {
            Response<T> response = new Response<T>();
            try
            {
                using (IDbConnection db = connection)
                {
                    SqlMapper.GridReader gridReader =  db.QueryMultiple(this.getStoredProc, this.parms, commandType: this.commandType);
                    response.Data = LoadRelatedObjects<T>(gridReader);
                    response.MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
            }
            return response;
        }


        protected Response<T> GetRelatedObjectsByIDTemp<T>()
        {
            Response<T> response = new Response<T>();
            try
            {
                using (IDbConnection db = connection)
                {
                    SqlMapper.GridReader gridReader = db.QueryMultiple(this.getStoredProc, this.parms, commandType: this.commandType);
                    response.Data = LoadRelatedObjects<T>(gridReader);
                    response.MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}, Stack Trace: {1}",
                   ex.Message,
                   ex.ToString()));

                response.MessageType = MessageType.Error;
                response.Message = string.Format("Error: {0} \n {1}", ex.Message, ex.ToString());
            }
            return response;
        }

    }
}
