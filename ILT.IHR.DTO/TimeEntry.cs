using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class TimeEntry: AbstractDataObject
    {
        public int TimeEntryID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }

        public int TimeSheetID { get; set; }
        [MaxLength(50)]
        public string Project { get; set; }
        [MaxLength(50)]
        public string Activity { get; set; }        
        public DateTime WorkDate { get; set; }        
        public int Hours { get; set; }
    }
}
