using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Ticket : AbstractDataObject
    {
        public int TicketID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int TicketTypeID { get; set; }
        public string TicketType { get; set; }
        public string TicketShort { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int RequestedByID { get; set; }
        public string RequestedBy { get; set; }
        //[Required]
        //[RegularExpression("^0*[1-9]\\d*$")]
        public int? ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int? ID { get; set; }
        [Required]
        public string Description { get; set; }
        //[Required]
        //[RegularExpression("^0*[1-9]\\d*$")]
        public int? AssignedToID { get; set; }
        public string AssignedTo { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int StatusID { get; set; }
        public string Status { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Comment { get; set; }
        [Required]
        public string Title { get; set; }
        public Guid LinkID { get; set; }
    }
}
