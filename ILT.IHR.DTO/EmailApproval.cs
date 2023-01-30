using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ILT.IHR.DTO
{
    public partial class EmailApproval : AbstractDataObject
    {
        public int EmailApprovalID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int ModuleID { get; set; }
        public int ID { get; set; }
        //public int UserID { get; set; }
        public DateTime ValidTime { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(100)]
        public string Value { get; set; }
        public Guid LinkID { get; set; }
        [MaxLength(50)]
        public string ApproverEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public string EmailBCC { get; set; }
        public string EmailBody { get; set; }
        public DateTime SendDate { get; set; }
        public int SentCount { get; set; }
        public int ReminderDuration { get; set; }
    }
}
