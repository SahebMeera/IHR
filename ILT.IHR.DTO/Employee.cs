using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Employee : AbstractDataObject
    {
        public int EmployeeID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string EmployeeCode { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        // public string EmployeeName { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int? TitleID { get; set; }
        public string Title { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int GenderID { get; set; }
        public string Gender { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        [Required]
        public string Phone { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        public string HomePhone { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        public string WorkPhone { get; set; }
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        [Required]
        public string Email { get; set; }
        public string LoginEmail { get; set; }
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string WorkEmail { get; set; }
        [Required]
        public DateTime? BirthDate { get; set; }
        [Required]
        public DateTime? HireDate { get; set; }
        public DateTime? TermDate { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int WorkAuthorizationID { get; set; }
        public string WorkAuthorization { get; set; }
        public string SSN { get; set; }
        //[RegularExpression("[A-Za-z]{5}\\d{4}[A-Za-z]{1}")]
        public string PAN { get; set; }
        //[RegularExpression("^[2-9]{1}[0-9]{3}[0-9]{4}[0-9]{4}$")]
        public string AadharNumber { get; set; }
        public int VariablePay { get; set; }
        public int Salary { get; set; }
        [Required]
        [Range(1,1000)]
        public int MaritalStatusID { get; set; }
        public string MaritalStatus { get; set; }
        public int? ManagerID { get; set; }
        public string Manager { get; set; }
        public string ManagerEmail { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int EmploymentTypeID { get; set; }
        public string EmploymentType { get; set; }
        public bool IsDeleted { get; set; }
        public List<Dependent> Dependents { get; set; }
        public List<DirectDeposit> DirectDeposits { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<EmployeeAddress> EmployeeAddresses { get; set; }
        public List<Salary> Salaries { get; set; }
        public string Client { get; set; }
        public string EndClient { get; set; }
        public DateTime? StatDate { get; set; }
        public string Skill { get; set; }
    }
}