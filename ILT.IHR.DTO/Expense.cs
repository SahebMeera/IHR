using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Expense : AbstractDataObject
    {
        public int ExpenseID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public int ExpenseTypeID { get; set; }
        public string ExpenseType { get; set; }
        public string FileName { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        [Required]
        public DateTime SubmissionDate { get; set; }
        public string SubmissionComment { get; set; }
        public int StatusID { get; set; }

        public Guid LinkID { get; set; }
        public string Status { get; set; }
        public int? AmountPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentComment { get; set; }
    }
}



