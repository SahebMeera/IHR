using System;
using System.Collections.Generic;
using System.Globalization;

namespace ILT.IHR.DTO
{
    public partial class EmailJob : AbstractDataObject
    {
        public string EmailJobID { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Body { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime SendDate { get; set; }
        public List<string> EmailToList { get; set; }
        public List<string> EmailCCList { get; set; }
    }
}
