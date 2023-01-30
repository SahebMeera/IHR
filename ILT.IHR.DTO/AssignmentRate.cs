using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class AssignmentRate : AbstractDataObject
    {
        public int AssignmentRateID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int AssignmentID { get; set; }
        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$")]
        public string BillingRate { get; set; }
        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$")]
        public string PaymentRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Boolean IsFLSAExempt { get; set; }
    }
}
