using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class ListType : AbstractDataObject
    {
        public int ListTypeID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public List<ListValue> ListValues { get; set; }

    }
}