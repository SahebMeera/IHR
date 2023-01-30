using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ILT.IHR.DTO
{
    public partial class Department : AbstractDataObject
    {
        public int DepartmentID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public int DeptLocationID { get; set; }
        public string DeptLocation { get; set; }
        public bool IsActive { get; set; }
    }
}