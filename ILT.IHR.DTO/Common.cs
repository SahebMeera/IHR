using System;
using System.Collections.Generic;
using System.Globalization;

namespace ILT.IHR.DTO
{
    public partial class Common : AbstractDataObject
    {
        public string EmailTo { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailCC { get; set; }
        public bool isMultipleEmail { get; set; }
        public List<string> EmailToList { get; set; }
        public List<string> EmailCCList { get; set; }
    }


    public class EmailFields
    {
        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailCC { get; set; }
        public bool isMultipleEmail { get; set; }
        public List<string> EmailToList { get; set; }
        public List<string> EmailCCList { get; set; }

    }

    public class AuditLog
    {
        public int AuditLogID { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }

        public int RecordId { get; set; }
        public string Values { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
