using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class Document : AbstractDataObject
    {
        public int DocumentID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int? EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int DocumentCategoryID { get; set; }
        public string DocumentCategory { get; set; }
        public int DocumentTypeID { get; set; }
        public string DocumentType { get; set; }
        public string IssuingAuthority { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Note { get; set; }
    }
}