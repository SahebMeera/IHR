using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ILT.IHR.DTO
{
    public partial class Company : AbstractDataObject
    {
        public int CompanyID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        public string Name { get; set; }
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
        public string ContactName { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string ContactPhone { get; set; }
        [Required]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string ContactEmail { get; set; }
        public string AlternateContactName { get; set; }
        public string AlternateContactPhone { get; set; }
        public string AlternateContactEmail { get; set; }
        [Required]
        public string InvoiceContactName { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string InvoiceContactPhone { get; set; }
        [Required]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string InvoiceContactEmail { get; set; }
        public string AlternateInvoiceContactName { get; set; }
        public string AlternateInvoiceContactPhone { get; set; }
        public string AlternateInvoiceContactEmail { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int InvoicingPeriodID { get; set; }
        public string InvoicingPeriod { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int PaymentTermID { get; set; }
        public string PaymentTerm { get; set; }
        [Required]
        public string TaxID { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int CompanyTypeID { get; set; }
        public string CompanyType { get; set; }
        public bool IsEndClient { get; set; }
    }
}