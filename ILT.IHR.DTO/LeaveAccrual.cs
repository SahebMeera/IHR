using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class LeaveAccrual : AbstractDataObject
    {
        public int LeaveAccrualID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string Country { get; set; }        
        public DateTime AccruedDate { get; set; }
        [Required]
        //[RegularExpression("^0*[1-9]\\d*$")]
        [RegularExpression("^(0*[1-9][0-9]*(\\.[0-9]+)?|0+\\.[0-9]*[1-9][0-9]*)$")]
        public Decimal AccruedValue { get; set; }

    }
}
