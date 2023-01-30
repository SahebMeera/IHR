using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class DirectDeposit : AbstractDataObject
    {
        public int DirectDepositID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int AccountTypeID { get; set; }
        public string AccountType { get; set; }
        [Required]
        public string RoutingNumber { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        [Required]
        [RegularExpression("^0*[0-9]\\d*$")]
        public int Amount { get; set; }
    }
}