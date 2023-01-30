using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ILT.IHR.DTO
{
    public class EmployeeAddress : AbstractDataObject
    {
        public int EmployeeAddressID { get; set; }
        public int EmployeeID { get; set; }
        public int AddressTypeID { get; set; }
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
        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
            
    }
}
