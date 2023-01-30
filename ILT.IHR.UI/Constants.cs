using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI
{
    public static class Constants
    {
        public static string DASHBOARD = "DASHBOARD";
        public static string EMPLOYEE = "EMPLOYEE";
        public static string EMPLOYEEINFO = "EMPLOYEEINFO";
        public static string COMPANY = "COMPANY"; 
        public static string USER = "USER";
        public static string LOOKUP = "LOOKUP";
        public static string TIMESHEET = "TIMESHEET";
        public static string TIMESHEETMGMT = "TIMESHEETMGMT";
        public static string PERMISSION = "PERMISSION";
        public static string HOLIDAY = "HOLIDAY";
        public static string LEAVEREQUEST = "LEAVEREQUEST";
        public static string MANAGELEAVE = "LEAVEMGMT";
        public static string ASSIGNMENT = "ASSIGNMENT";
        public static string MANAGETIMESHEET = "TIMESHEETMGMT";
        public static string W4INFO = "W4INFO";
        public static string I9INFO = "I9INFO";
        public static string SALARY = "SALARY";
        public static string SKILL = "SKILL";
        public static string EXPENSES = "EXPENSES";
        public static string WFHREQUEST = "WFHREQUEST";
        public static string PROCESSDATA = "PROCESSDATA";
        public static string ASSET = "ASSET";
        public static string TICKET = "TICKET";
        public static string REPORTS = "REPORTS";
        public static string APPRAISAL = "APPRAISAL";
        public static string WIZARDSTATUS = "WIZARDSTATUS";
        public static string NPI = "NPI";
    }
    public static class SessionConstants
    {
        public static string ROLEPERMISSION = "ROLEPERMISSION";
        public static string USERROLES = "USERROLES";        
        public static string LEAVEACCRUAL = "LeaveAccrual";
        public static string USERROLEPERMISSIONS = "USERROLEPERMISSIONS";

    }
    public static class ListTypeConstants
    {
        public static string PAYMENTTERM = "PAYMENTTERM";
        public static string COMPANYTYPE = "COMPANYTYPE";
        public static string INVOICINGPERIOD = "INVOICINGPERIOD";
        public static string VACATIONTYPE = "VACATIONTYPE";
        public static string LEAVEREQUESTSTATUS = "LEAVEREQUESTSTATUS";
        public static string TIMESHEETTYPE = "TIMESHEETTYPE";
        public static string TITLE = "TITLE";
        public static string VISATYPE = "VISATYPE";
        public static string RELATION = "RELATION";
        public static string GENDER = "GENDER";
        public static string EMPLOYMENTTYPE = "EMPLOYMENTTYPE";
        public static string WORKAUTHORIZATION = "WORKAUTHORIZATION";
        public static string MARITALSTATUS = "MARITALSTATUS";
        public static string WITHHOLDINGSTATUS = "WITHHOLDINGSTATUS";
        public static string ACCOUNTTYPE = "ACCOUNTTYPE";
        public static string CONTACTTYPE = "CONTACTTYPE";
        public static string TIMESHEETSTATUS = "TIMESHEETSTATUS";
        public static string PAYMENTTYPE = "PAYMENTTYPE";
        public static string ADDRESSTYPE = "ADDRESSTYPE";
        public static string ExpenseType = "EXPENSETYPE";
        public static string ExpenseStatus = "EXPENSESTATUS";
        public static string W4TYPE = "W4TYPE";
        public static string I9DOCUMENTTYPE = "I9DOCUMENTTYPE";
        public static string I9LISTADOCUMENTS = "I9LISTADOCUMENTS";
        public static string I9LISTBDOCUMENTS = "I9LISTBDOCUMENTS";
        public static string I9LISTCDOCUMENTS = "I9LISTCDOCUMENTS";
        public static string WFHSTATUS = "WFHSTATUS";
        public static string LWP = "LWP";
        public static string ASSETTYPE = "ASSETTYPE";
        public static string ASSETSTATUS = "ASSETSTATUS";
        public static string TICKETTYPE = "TICKETTYPE";
        public static string TICKETSTATUS = "TICKETSTATUS";
        public static string REPORTTYPE = "REPORTTYPE";
        public static string APPRAISALSTATUS = "APPRAISALSTATUS";
        public static string GREENCARD = "GC";
        public static string USCITIZEN = "USC";
        public static string H1B = "H1B";
        public static string E3 = "E3";
        public static string TN = "TN";
        public static string H4EAD = "H4EAD";
        public static string TICKETEMAILMAP = "TICKETEMAILMAP";
        public static string SKILLTYPE = "SKILLTYPE";

        public static class TimeSheetStatusConstants
        {            
            public static string PENDING = "PENDING";
            public static string SUBMITTED = "SUBMITTED";
            public static string APPROVED = "APPROVED";
            public static string REJECTED = "REJECTED";
            public static string CLOSED = "CLOSED";
            public static string VOID = "VOID";

        }

        public static class WizardStatusConstants
        {
            public static string PENDING = "PENDING";
            public static string INPROCESS = "INPROCESS";
            public static string PROCESSED = "PROCESSED";
        }
    }

    public static class ConfigPageSize
    {
        public static string PAGESIZE = "PageSize";

    }

    public static class TimeSheetType
    {
        public static string MONTHLY = "MONTHLY";
        public static string FRIDAY = "WEEKLY - FRIDAY";
        public static string SATURDAY = "WEEKLY - SATURDAY";
        public static string SUNDAY = "WEEKLY - SUNDAY";

    }
    public static class UserRole
    {
        public static string FINADMIN = "FINADMIN";
        public static string FINUSERL1 = "FINUSERL1";
        public static string FINUSERL2 = "FINUSERL2";
        public static string ADMIN = "ADMIN";
        public static string ITADMIN = "ITADMIN";
        public static string EMP = "EMP";
        public static string CONTRACTOR = "CONTRACTOR";
        public static string OPSADMIN = "OPSADMIN";
        public static string OPSUSERL1 = "OPSUSERL1";
        public static string OPSUSERL2 = "OPSUSERL2";
        public static string SUPPORT = "SUPPORT";

    }
    public static class EmployeeAddresses
    {
        public static string MAILADD = "MAILADD";
        public static string CURRADD = "CURRADD";
        public static string PERMADD = "PERMADD";
    }
    public static class AppraisalResponseType
    {
        public static string RATING = "RATING";
        public static string GOALPREVYEAR = "GOALPREVYEAR";
        public static string GOALCURRYEAR = "GOALCURRYEAR";
        public static string GOALRATING = "GOALRATING";
    }

    public static class AppraisalStatus
    {
        public static string ASSIGNED = "ASSIGNED";
        public static string COMPLETE = "COMPLETE";
        public static string PENDINGFINALREVIEW = "PENDINGFINALREVIEW";
        public static string PENDINGREVIEW = "PENDINGREVIEW";
    }


    public static class Countries
    {
        public static string INDIA = "INDIA";
        public static string UNITEDSTATES = "UNITED STATES";
    }

    public static class AssetStatusConstants
    {
        public static string ASSIGNED = "ASSIGNED";
        public static string UNASSIGNED = "UNASSIGNED";
        public static string DECOMMISSIONED = "DECOMMISSIONED";
        public static string MAINTENANCE = "MAINTENANCE";
        public static string ASSIGNEDTEMP = "ASSIGNEDTEMP";
    }

    public static class TicketStatusConstants
    {
        public static string ASSIGNED = "ASSIGNED";
        public static string NEW = "NEW";
        public static string RESOLVED = "RESOLVED";
    }
    public static class TicketTypeConstants
    {
        public static string ITSUPPORT = "ITSUPPORT";
        public static string IHRSUPPORT = "IHRSUPPORT";
        public static string OPERATIONS = "OPERATIONS";
        public static string FINANCE = "FINANCE";
    }

    public static class HelpDocumentation
    {
        public static string EMPLOYEES = "EMPLOYEES";
        public static string WFHREQUESTS = "WFHREQUESTS";
        public static string HOLIDAYS = "HOLIDAYS";
        public static string LEAVEREQUESTS = "LEAVEREQUESTS";
        public static string TIMESHEET = "TIMESHEET";
        public static string COMPANY = "COMPANY";
        public static string MANAGELEAVE = "MANAGELEAVE";
        public static string MANAGETIMESHEET = "MANAGETIMESHEET";
        public static string LOOKUPTABLES = "LOOKUPTABLES";
        public static string ROLEPERMISSION = "ROLEPERMISSION";
        public static string USERS = "USERS";
        public static string EXPENSES = "EXPENSES";
        public static string ASSET = "ASSET";
        public static string TICKET = "TICKET";
        public static string APPRAISAL = "APPRAISAL";
        public static string PROCESSDATAS = "PROCESSDATAS";
    }

    public static class ErrorMsg
    {
        public static string ERRORMSG = "An error occurred while processing your request";
    }

    public static class LeaveStatus
    {
        public static string PENDING = "PENDING";
        public static string APPROVED = "APPROVED";
        public static string DENIED = "DENIED";
        public static string CANCELLED = "CANCELLED";
    }

    public static class LeaveType
    {
        public static string CASUAL = "CASUAL";
        public static string LWP = "LWP";
    }
}








