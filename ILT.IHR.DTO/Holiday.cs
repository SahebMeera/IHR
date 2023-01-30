using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Holiday : AbstractDataObject
    {
        public int HolidayID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public string Country { get; set; }
    }
}
