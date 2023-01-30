using System;
using System.Data;

namespace ILT.IHR.DTO
{
    public abstract class AbstractDataObject
    {
        public int RecordID { get; set; }
        public bool HasChange { get; set; }
        public int MyProperty { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Byte[] TimeStamp { get; set; }

        public string EmployeeName { get; set; }
    }
}
