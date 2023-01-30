using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Dependent : AbstractDataObject
    {
        public int DependentID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int EmployeeID { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int RelationID { get; set; }
        public string Relation { get; set; }
        public DateTime BirthDate { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int VisaTypeID { get; set; }
        public string VisaType{ get; set; }
    }
}
