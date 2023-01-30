export enum  AllowedClients {
    ILT= "InfoLogitech",
    ITE= "IT Echelon",
    DEV= "Development"
}
export enum  EmailApprovalUrl {
    ILT = "https://infohr.azurewebsites.net/",
    ITE = "https://infohr.azurewebsites.net/",
    DEV = "https://ihrtest.azurewebsites.net/"
}

export class Settings  {
    EmailNotifications = {
        ILT: {
            "LeaveRequest": "accounts@infologitech.com",
            "Timesheet": "accounts@infologitech.com",
            "WFHRequest": "accounts@infologitech.com",
            "Ticket": "ihrticket@infologitech.com",
            "FromEmail": "info@infologitech.com",
            "ChangeNotification": "IHRChangeNotification@infologitech.com",
            "Expense": "accounts@infologitech.com"
        },
        "ITE": {
            "LeaveRequest": "accounts@itechelon.com",
            "Timesheet": "accounts@itechelon.com",
            "WFHRequest": "accounts@itechelon.com",
            "Ticket": "ihrticket@itechelon.com",
            "FromEmail": "info@infologitech.com",
            "ChangeNotification": "IHRChangeNotification@infologitech.com",
            "Expense": "accounts@itechelon.com"
        },
        "DEV": {
            "LeaveRequest": "ihrtesting@infologitech.com",
            "Timesheet": "ihrtesting@infologitech.com",
            "WFHRequest": "ihrtesting@infologitech.com",
            "Ticket": "ihrtesting@infologitech.com",
            "FromEmail": "info@infologitech.com",
            "ChangeNotification": "ihrtesting@infologitech.com",
            "Expense": "ihrtesting@infologitech.com"
        }
    }

    ImageURL= {
        "ILT": {
            "Approve": "https://infohr.azurewebsites.net/assets/layout/images/pages/Approve.jpg",
            "Reject": "https://infohr.azurewebsites.net/assets/layout/images/pages/Reject.jpg"
        },
        "ITE": {
            "Approve": "https://infohr.azurewebsites.net/assets/layout/images/pages/Approve.jpg",
            "Reject": "https://infohr.azurewebsites.net/assets/layout/images/pages/Reject.jpg"
        },
        "DEV": {
            "Approve": "https://ihrtest.azurewebsites.net/assets/layout/images/pages/Approve.jpg",
            "Reject": "https://ihrtest.azurewebsites.net/assets/layout/images/pages/Reject.jpg"
        }
    }
}


export enum Constants {
    DASHBOARD = "DASHBOARD",
    EMPLOYEE = "EMPLOYEE",
    EMPLOYEEINFO = "EMPLOYEEINFO",
    COMPANY = "COMPANY",
    USER = "USER",
    LOOKUP = "LOOKUP",
    TIMESHEET = "TIMESHEET",
    TIMESHEETMGMT = "TIMESHEETMGMT",
    PERMISSION = "PERMISSION",
    HOLIDAY = "HOLIDAY",
    LEAVEREQUEST = "LEAVEREQUEST",
    MANAGELEAVE = "LEAVEMGMT",
    ASSIGNMENT = "ASSIGNMENT",
    MANAGETIMESHEET = "TIMESHEETMGMT",
    W4INFO = "W4INFO",
    I9INFO = "I9INFO",
    SALARY = "SALARY",
    SKILL = "SKILL",
    EXPENSES = "EXPENSES",
    WFHREQUEST = "WFHREQUEST",
    PROCESSDATA = "PROCESSDATA",
    ASSET = "ASSET",
    TICKET = "TICKET",
    REPORTS = "REPORTS",
    APPRAISAL = "APPRAISAL",
    WIZARDSTATUS = "WIZARDSTATUS",
    NPI = "NPI"
}
export enum SessionConstants {
    ROLEPERMISSION = "ROLEPERMISSION",
    USERROLES = "USERROLES",
    LEAVEACCRUAL = "LeaveAccrual",
    USERROLEPERMISSIONS = "USERROLEPERMISSIONS",
}

export enum ListTypeConstants {
    PAYMENTTERM = "PAYMENTTERM",
    COMPANYTYPE = "COMPANYTYPE",
    INVOICINGPERIOD = "INVOICINGPERIOD",
    VACATIONTYPE = "VACATIONTYPE",
    LEAVEREQUESTSTATUS = "LEAVEREQUESTSTATUS",
    TIMESHEETTYPE = "TIMESHEETTYPE",
    TITLE = "TITLE",
    VISATYPE = "VISATYPE",
    RELATION = "RELATION",
    GENDER = "GENDER",
    EMPLOYMENTTYPE = "EMPLOYMENTTYPE",
    WORKAUTHORIZATION = "WORKAUTHORIZATION",
    MARITALSTATUS = "MARITALSTATUS",
    WITHHOLDINGSTATUS = "WITHHOLDINGSTATUS",
    ACCOUNTTYPE = "ACCOUNTTYPE",
    CONTACTTYPE = "CONTACTTYPE",
    TIMESHEETSTATUS = "TIMESHEETSTATUS",
    PAYMENTTYPE = "PAYMENTTYPE",
    ADDRESSTYPE = "ADDRESSTYPE",
    ExpenseType = "EXPENSETYPE",
    ExpenseStatus = "EXPENSESTATUS",
    W4TYPE = "W4TYPE",
    I9DOCUMENTTYPE = "I9DOCUMENTTYPE",
    I9LISTADOCUMENTS = "I9LISTADOCUMENTS",
    I9LISTBDOCUMENTS = "I9LISTBDOCUMENTS",
    I9LISTCDOCUMENTS = "I9LISTCDOCUMENTS",
    WFHSTATUS = "WFHSTATUS",
    LWP = "LWP",
    ASSETTYPE = "ASSETTYPE",
    ASSETSTATUS = "ASSETSTATUS",
    TICKETTYPE = "TICKETTYPE",
    TICKETSTATUS = "TICKETSTATUS",
    REPORTTYPE = "REPORTTYPE",
    APPRAISALSTATUS = "APPRAISALSTATUS",
    GREENCARD = "GC",
    USCITIZEN = "USC",
    H1B = "H1B",
    E3 = "E3",
    TN = "TN",
    H4EAD = "H4EAD",
    TICKETEMAILMAP = "TICKETEMAILMAP",
    SKILLTYPE = "SKILLTYPE",
    TimeSheetStatusConstants = "TimeSheetStatusConstants"
}
export enum TimeSheetStatusConstants
{
    PENDING = "PENDING",
    SUBMITTED = "SUBMITTED",
    APPROVED = "APPROVED",
    REJECTED = "REJECTED",
    CLOSED = "CLOSED",
    VOID = "VOID"
}
export enum ConfigPageSize {
     PAGESIZE = "PageSize",
}

export enum TimeSheetType {
     MONTHLY = "MONTHLY",
     FRIDAY = "WEEKLY - FRIDAY",
     SATURDAY = "WEEKLY - SATURDAY",
     SUNDAY = "WEEKLY - SUNDAY",

}
export enum UserRole {
     FINADMIN = "FINADMIN",
     FINUSERL1 = "FINUSERL1",
     FINUSERL2 = "FINUSERL2",
     ADMIN = "ADMIN",
     ITADMIN = "ITADMIN",
     EMP = "EMP",
     CONTRACTOR = "CONTRACTOR",
     OPSADMIN = "OPSADMIN",
     OPSUSERL1 = "OPSUSERL1",
     OPSUSERL2 = "OPSUSERL2",
     SUPPORT = "SUPPORT",

}
export enum EmployeeAddresses {
     MAILADD = "MAILADD",
     CURRADD = "CURRADD",
     PERMADD = "PERMADD",
}
export enum AppraisalResponseType {
     RATING = "RATING",
     GOALPREVYEAR = "GOALPREVYEAR",
     GOALCURRYEAR = "GOALCURRYEAR",
     GOALRATING = "GOALRATING",
}

export enum AppraisalStatus {
     ASSIGNED = "ASSIGNED",
     COMPLETE = "COMPLETE",
     PENDINGFINALREVIEW = "PENDINGFINALREVIEW",
     PENDINGREVIEW = "PENDINGREVIEW",
}


export enum Countries {
     INDIA = "INDIA",
     UNITEDSTATES = "UNITED STATES",
}

export enum AssetStatusConstants {
     ASSIGNED = "ASSIGNED",
     UNASSIGNED = "UNASSIGNED",
     DECOMMISSIONED = "DECOMMISSIONED",
     MAINTENANCE = "MAINTENANCE",
     ASSIGNEDTEMP = "ASSIGNEDTEMP",
}

export enum TicketStatusConstants {
     ASSIGNED = "ASSIGNED",
     NEW = "NEW",
     RESOLVED = "RESOLVED",
}
export enum TicketTypeConstants {
     ITSUPPORT = "ITSUPPORT",
     IHRSUPPORT = "IHRSUPPORT",
     OPERATIONS = "OPERATIONS",
     FINANCE = "FINANCE",
}

export enum HelpDocumentation {
     DASHBOARD = "DASHBOARD",
     EMPLOYEES = "EMPLOYEES",
     WFHREQUESTS = "WFHREQUESTS",
     HOLIDAYS = "HOLIDAYS",
     LEAVEREQUESTS = "LEAVEREQUESTS",
     TIMESHEET = "TIMESHEET",
     COMPANY = "COMPANY",
     MANAGELEAVE = "MANAGELEAVE",
     MANAGETIMESHEET = "MANAGETIMESHEET",
     LOOKUPTABLES = "LOOKUPTABLES",
     ROLEPERMISSION = "ROLEPERMISSION",
     USERS = "USERS",
     EXPENSES = "EXPENSES",
     ASSET = "ASSET",
     TICKET = "TICKET",
     APPRAISAL = "APPRAISAL",
     PROCESSDATAS = "PROCESSDATAS",
    REPORTS = "REPORTS",
}

export enum ErrorMsg {
     ERRORMSG = "An error occurred while processing your request",
}

export enum LeaveStatus {
     PENDING = "PENDING",
     APPROVED = "APPROVED",
     DENIED = "DENIED",
     CANCELLED = "CANCELLED",
}

export enum LeaveType {
     CASUAL = "CASUAL",
     LWP = "LWP",
}
export enum WizardStatusConstants {
     PENDING = "PENDING",
     INPROCESS = "INPROCESS",
     PROCESSED = "PROCESSED",
}
export enum Guid {
    Empty = '00000000-0000-0000-0000-000000000000',
}
