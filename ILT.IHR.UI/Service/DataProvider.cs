using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;

namespace ILT.IHR.UI.Service
{
    public class DataProvider
    {
        public object storage;
        public string country;
        public string status;
        public List<IMultiSelectDropDownList> employeeType;
        public int DefaultPageSize;
        public object table;
        public int TabIndex;
            
    }
}
