using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IDirectDepositService
    {
        Task<Response<DirectDeposit>> GetDirectDepositByIdAsync(int Id);
        // Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<Response<DirectDeposit>> SaveDirectDeposit(DirectDeposit obj);
        Task<Response<DirectDeposit>> UpdateDirectDeposit(int Id, DirectDeposit updateObject);
    }
}
