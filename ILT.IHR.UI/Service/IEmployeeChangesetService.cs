using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public interface IEmployeeChangesetService
    {
        Task<Response<IEnumerable<EmployeeChangeSet>>> GetEmployeeChangeset(int employeeid);
    }
}
