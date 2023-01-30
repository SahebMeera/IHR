using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class Module : AbstractDataObject
    {
        public int ModuleID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string ModuleShort { get; set; }
        public string ModuleName { get; set; }
    }
}
