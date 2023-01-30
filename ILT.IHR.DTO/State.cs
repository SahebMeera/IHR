using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class State : AbstractDataObject
    {
        public int StateID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public string StateShort { get; set; }
        public string StateDesc { get; set; }
    }
}