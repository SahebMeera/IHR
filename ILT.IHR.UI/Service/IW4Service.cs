using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IEmployeeW4Service
    {
        Task<Response<IEnumerable<EmployeeW4>>> GetEmployeesW4(int EmployeeID);
       
        Task<Response<EmployeeW4>> GetEmployeeW4ByIdAsync(int Id);
        Task<Response<EmployeeW4>> SaveEmployeeW4(EmployeeW4 obj);
        Task<Response<EmployeeW4>> UpdateEmployeeW4(int Id, EmployeeW4 updateObject);
        //Task<T> SaveAsync(string requestUri, T obj);
        //Task<T> UpdateAsync(string requestUri, int Id, T obj);
        //Task<bool> DeleteAsync(string requestUri, int Id);
    }
}
