using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class LeaveBalance : AbstractDataObject
    {
        public int LeaveBalanceID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int? EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int LeaveYear { get; set; }
        public int LeaveTypeID { get; set; }
        public string LeaveType { get; set; }
        public string Country { get; set; }
        public decimal VacationTotal { get; set; }
        public decimal VacationUsed { get; set; }
        public decimal UnpaidLeave { get; set; }
        public decimal VacationBalance { get; set; }
        public decimal EncashedLeave { get; set; }
        public decimal LWPAccounted { get; set; }
        public decimal LeaveInRange { get; set; }
        public decimal LWPPending { get; set; }
        public decimal LeaveInNextMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? TermDate { get; set; }

    }
}