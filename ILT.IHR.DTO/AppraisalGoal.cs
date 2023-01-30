using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class AppraisalGoal : AbstractDataObject
    {
        public int AppraisalGoalID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int AppraisalID { get; set; }
        //[Required]
        //[RegularExpression("^0*[1-9]\\d*$")]
        public int ReviewYear { get; set; }
        public string Goal { get; set; }
        public int? EmpResponse { get; set; }
        public string EmpComment { get; set; }
        public int? MgrResponse { get; set; }
        public string MgrComment { get; set; }
   
    }
}
