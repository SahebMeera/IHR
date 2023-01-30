using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class EmployeeIN : AbstractDataObject
    {
        public int EmployeeINID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}