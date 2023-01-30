using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.DTO
{
    public partial class ProcessWizard : AbstractDataObject
    {
        public int ProcessWizardID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string Process { get; set; }        
        public string Elements { get; set; }
        public string Name { get; set; }
        public List<Element> Fields { get; set; }
    }
}
