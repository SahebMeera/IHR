using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.DTO
{
    public class EmployeeChangeSet: Employee
    {
        public int EmployeeChangeSetID { get; set; }

        public int UserID { get; set; }
    }
}
