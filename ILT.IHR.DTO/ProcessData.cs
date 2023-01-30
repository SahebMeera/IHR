using System;
using System.Collections.Generic;
using System.Text;
namespace ILT.IHR.DTO
{
    public partial class ProcessData : AbstractDataObject
    {
        public int ProcessDataID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int ProcessWizardID { get; set; }
        public string Process { get; set; }
        public string Data { get; set; }      
        public string DataColumns { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string ChangeNotificationEmailId { get; set; }
        public int EmailApprovalValidity { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public List<ProcessDataTicket> ProcessDataTickets { get; set; }

    }
}
