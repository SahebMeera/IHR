using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ISalaryService
    {
        Task<Response<Salary>> GetSalaryById(int Id);
        Task<Response<Salary>> UpdateSalary(int Id,Salary updateObject);
        Task<Response<Salary>> SaveSalary(Salary obj);
        Task DeleteSalary(int id);
    }
}
