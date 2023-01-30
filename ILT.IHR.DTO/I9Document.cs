using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class I9Document : AbstractDataObject
    {
        public int I9DocumentID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string I9DocName { get; set; }
        public int I9DocTypeID { get; set; }
        public int WorkAuthID { get; set; }
    }
}
