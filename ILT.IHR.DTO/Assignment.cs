using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Assignment : AbstractDataObject
    {
        public int AssignmentID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? VendorID { get; set; }
        public string Vendor { get; set; }
        public string Role { get; set; }
        public string ClientManager { get; set; }
        public string Title { get; set; }
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
        [RegularExpression("^0*[1-9]\\d*$")]
        public int ClientID { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int? EndClientID { get; set; }
        public string SubClient { get; set; }
        public string Client { get; set; }
        public string Comments { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int PaymentTypeID { get; set; }
        public string PaymentType { get; set; }
        public int? TimeSheetTypeID { get; set; }
        public string TimeSheetType { get; set; }
        //public int? TimesheetApproverID { get; set; }
        //public string TimesheetApprover { get; set; }
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string TSApproverEmail { get; set; }
        [RegularExpression("^([\\w+-.%]+@[\\w.-]+\\.[A-Za-z]{2,4})(;[\\w+-.%]+@[\\w.-]+\\.[A-Za-z]{2,4})*$")]
        public string ApprovedEmailTo { get; set; }
        public List<AssignmentRate> AssignmentRates { get; set; }
    }
}
