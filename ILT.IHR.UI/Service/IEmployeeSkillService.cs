using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IEmployeeSkillService
    {
        Task<Response<IEnumerable<EmployeeSkill>>> GetEmployeeSkill(int EmployeeID);
        Task<Response<EmployeeSkill>> GetEmployeeSkillByIdAsync(int Id);
        Task<Response<EmployeeSkill>> SaveEmployeeSkill(EmployeeSkill obj);
        Task<Response<EmployeeSkill>> UpdateEmployeeSkill(int Id, EmployeeSkill updateObject);
        Task DeleteEmployeeSkill(int id);
    }
}
