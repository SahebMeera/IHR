using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Shared
{
    public class CommonUtils
    {
        public string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
    }
}
