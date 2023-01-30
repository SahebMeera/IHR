using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class ListValue : AbstractDataObject
    {
        public int ListValueID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int ListTypeID { get; set; }
        public string Type { get; set; }
       
        public string TypeDesc { get; set; }
        [Required(ErrorMessage = "Lookup Value is required.")]
        public string Value { get; set; }
        [Required(ErrorMessage = "Lookup Description is required.")]
        public string ValueDesc { get; set; }
        public bool IsActive { get; set; }
    }
}