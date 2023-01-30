using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class AppraisalDetail : AbstractDataObject
    {
        public int AppraisalDetailID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int AppraisalID { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int AppraisalQualityID { get; set; }
        public string Quality { get; set; }
        public int ResponseTypeID { get; set; }
        public string ResponseType { get; set; }
        public string ResponseTypeDescription { get; set; }
        public int? EmpResponse { get; set; }
        public string EmpComment { get; set; }
        public int? MgrResponse { get; set; }
        public string MgrComment { get; set; }
   
    }
}
