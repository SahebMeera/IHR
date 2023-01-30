using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class EmployeeSkill: AbstractDataObject
    {
        public int EmployeeSkillID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int SkillTypeID { get; set; }
        public string SkillType { get; set; }
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string Skill { get; set; }
        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$")]
        public string Experience { get; set; }

      
    }
}
