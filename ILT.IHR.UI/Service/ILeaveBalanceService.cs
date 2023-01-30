using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ILeaveBalanceService
    {
        Task<Response<IEnumerable<LeaveBalance>>> GetLeaveBalance(int? EmployeID);
        Task<Response<LeaveBalance>> GetLeaveBalanceById(int ID);
        Task<Response<LeaveBalance>> updateLeaveBalance(int ID, LeaveBalance updateLeaveBalance);
        Task<DataTable> GetReportLeaveInfo(Report report, string LeaveSummaryStatus);
        Task<DataTable> GetReportLeaveDetailInfo(Report report, string LeaveDetailStatus);
    }
}
