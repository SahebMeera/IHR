using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Appraisal : AbstractDataObject
    {
        public int AppraisalID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int ReviewYear { get; set; }
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int? FinalReviewerID { get; set; }
        public string Manager { get; set; }
        public string FinalReviewer { get; set; }
        public int? ReviewerID { get; set; }
        public string Reviewer { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public DateTime? FinalReviewDate { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string StatusValue { get; set; }
        public string MgrFeedback { get; set; }
        public string Comment { get; set; }
   
        public List<AppraisalDetail> AppraisalDetails { get; set; }
        public List<AppraisalGoal> AppraisalGoals { get; set; }
    }
}
