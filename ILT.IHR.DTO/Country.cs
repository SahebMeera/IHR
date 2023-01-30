using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class Country : AbstractDataObject
    {
        public int CountryID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string CountryDesc { get; set; }
        public List<State> States{ get; set; }
    }
}
