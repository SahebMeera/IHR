using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ILT.IHR.DTO
{
    public class TimeSheet: AbstractDataObject
    {
        public int TimeSheetID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }

        public int? EmployeeID { get; set; }        
        public string EmployeeName { get; set; }
        [Required]
        public DateTime? WeekEnding { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientManager { get; set; }
        public int AssignmentID { get; set; }
        public int TotalHours { get; set; }
        [MaxLength(100)]
        public string FileName { get; set; }
        public Guid DocGuid { get; set; }
        //public int TimesheetApproverID { get; set; }
        //public string TSApproverName { get; set; }
        public string TSApproverEmail { get; set; }
        public string ApprovedEmailTo { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int StatusID { get; set; }
        public string StatusValue { get; set; }
        public string Status { get; set; }
        public int? SubmittedByID { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        //public int? ApprovedByID { get; set; }
        //public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [MaxLength(50)]
        public string ApprovedByEmail { get; set; }
        public int? ClosedByID { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int EmailApprovalID { get; set; }
        public Guid LinkID { get; set; }
        public DateTime ValidTime { get; set; }
        public int TimeSheetTypeID { get; set; }
        public string TimeSheetType { get; set; }
        public string Comment { get; set; }
        public List<TimeEntry> TimeEntries { get; set; }
    }
}
