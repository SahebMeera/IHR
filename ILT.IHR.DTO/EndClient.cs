using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class EndClient : AbstractDataObject
    {
        public int EndClientID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string Name { get; set; }
        public int CompanyID { get; set; }
        public string TaxID { get; set; }
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
    }
}
