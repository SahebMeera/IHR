using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Salary : AbstractDataObject
    {
        public int SalaryID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
      
        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$")]
        public string BasicPay { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string HRA { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string LTA { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string Bonus { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string EducationAllowance { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string VariablePay { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string SpecialAllowance { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string ProvidentFund { get; set; }   
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string TelephoneAllowance { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string MedicalAllowance { get; set; }   
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string MedicalInsurance { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string MealAllowance { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string Conveyance { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$")]
        public string Gratuity { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$")]
        public string CostToCompany { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }
}
