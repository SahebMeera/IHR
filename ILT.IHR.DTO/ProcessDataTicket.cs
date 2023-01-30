using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.DTO
{
   public class ProcessDataTicket : AbstractDataObject
    {
        public int ProcessDataTicketID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int ProcessDataID { get; set; }
        public int TicketID { get; set; }
        public string Title { get; set; }
        public string TicketDescription { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public int AssignedToId { get; set; }
        public string AssignedToUser { get; set; }
    }
}
