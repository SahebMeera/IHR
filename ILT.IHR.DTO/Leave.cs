using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Leave : AbstractDataObject
    {
        public int LeaveID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludesHalfDay { get; set; }
        public Decimal Duration { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int LeaveTypeID { get; set; }
        public string LeaveType { get; set; }
        public Guid LinkID { get; set; }
        public int RequesterID { get; set; }
        public string Requester { get; set; }
        public int? ApproverID { get; set; }
        public string Approver { get; set; }
        public int StatusID { get; set; }
        public string StatusValue { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }
        public List<LeaveBalance> LeaveBalances { get; set; }
    }
}