using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class EmployeeW4 : AbstractDataObject
    {
        public int EmployeeW4ID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [MaxLength(9)]
        [MinLength(9)]
        public string SSN { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int W4TypeID { get; set; }
        public string W4Type { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int WithHoldingStatusID { get; set; }
        public string WithHoldingStatus { get; set; }
        [RegularExpression("^[0-9]\\d*$")]
        public int? Allowances { get; set; }
        public bool IsMultipleJobsOrSpouseWorks { get; set; }
        [Range(0, 10)]
        public int? QualifyingChildren { get; set; }
        [Range(0, 10)]
        public int? OtherDependents { get; set; }
        [RegularExpression("(\\d+)(\\.)?(\\d+)?")]
        public string OtherIncome { get; set; }
        [RegularExpression("(\\d+)(\\.)?(\\d+)?")]
        public string Deductions { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}