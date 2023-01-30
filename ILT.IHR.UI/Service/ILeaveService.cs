using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ILeaveService
    {
        Task<Response<IEnumerable<Leave>>> GetLeave(string Parameter = "", int ID = 0);
        Task<Response<Leave>> GetLeaveByIdAsync(int Id,int EmployeeID,int ApproverID);
        Task<Response<Leave>> SaveLeave(Leave obj);
        Task<Response<Leave>> UpdateLeave(int Id, Leave updateObject);
        Task<Response<Leave>> GetLeaveDays(string clientId, int employeeId, DateTime startDate, DateTime endDate, bool includesHalfDay);
    }
}
