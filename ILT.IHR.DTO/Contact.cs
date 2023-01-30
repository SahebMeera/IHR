using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Contact : AbstractDataObject
    {
        public int ContactID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int ContactTypeID { get; set; }
        public string ContactType { get; set; }
        public int? EmployeeID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string Phone { get; set; }
        [Required]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string Email { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
