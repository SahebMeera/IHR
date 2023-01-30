using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IDependentService
    {
        Task<Response<Dependent>> GetDependentByIdAsync(int Id);
       // Task<IEnumerable<Department>> GetDepartments();
       // Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<Response<Dependent>> SaveDependent(Dependent obj);
        Task<Response<Dependent>> UpdateDependent(int Id, Dependent updateObject);
    }
}
