using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class FormI9 : AbstractDataObject
    {
        public int FormI9ID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        //[Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
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
        public DateTime? BirthDate { get; set; }
        [Required]
        public string SSN { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        [Required]
        public string Phone { get; set; }
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        [Required]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int WorkAuthorizationID { get; set; }
        public string WorkAuthorization { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        public string ANumber { get; set; }
        [MaxLength(10)]
        public string USCISNumber { get; set; }
        [MaxLength(11)]           
        public string I94Number { get; set; }
        public DateTime? I94ExpiryDate { get; set; }
        [MaxLength(20)] 
        public string PassportNumber { get; set; }        
        public string PassportCountry { get; set; }
        [Required]
        public DateTime? HireDate { get; set; }
        public int? ListADocumentTitleID { get; set; }
        public string ListADocumentTitle { get; set; }
        public string ListAIssuingAuthority { get; set; }
        public string ListADocumentNumber { get; set; }
        public DateTime? ListAStartDate { get; set; }
        public DateTime? ListAExpirationDate { get; set; }
        public int? ListBDocumentTitleID { get; set; }
        public string ListBDocumentTitle { get; set; }
        public string ListBIssuingAuthority { get; set; }
        public string ListBDocumentNumber { get; set; }
        public DateTime? ListBStartDate { get; set; }
        public DateTime? ListBExpirationDate { get; set; }
        public int? ListCDocumentTitleID { get; set; }
        public string ListCDocumentTitle { get; set; }
        public string ListCIssuingAuthority { get; set; }
        public string ListCDocumentNumber { get; set; }
        public DateTime? ListCStartDate { get; set; }
        public DateTime? ListCExpirationDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? InputDate { get; set; }

    }
}