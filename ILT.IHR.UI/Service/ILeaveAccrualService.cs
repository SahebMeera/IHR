using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ILeaveAccrualService
    {
        Task<Response<IEnumerable<LeaveAccrual>>> GetLeaveAccrual(string Country);
        Task<Response<LeaveAccrual>> GetLeaveAccrualByIdAsync(int Id);
        Task<Response<LeaveAccrual>> UpdateLeaveAccrual(int Id, LeaveAccrual updateObject);
        Task<Response<LeaveAccrual>> SaveLeaveAccrual(LeaveAccrual obj);
        Task<Response<LeaveAccrual>> DeleteLeaveAccrual(int id);
    }
}
