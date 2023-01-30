using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentRate>> GetAssignmentRates();
        Task<Response<Assignment>> GetAssignmentById(int Id);
        Task<Response<Assignment>> UpdateAssignment(int Id,Assignment updateObject);
        Task<Response<Assignment>> SaveAssignment(Assignment obj);
        Task DeleteAssignment(int id);
        Task<Response<AssignmentRate>> GetAssignmentRateById(int Id);
        Task<Response<AssignmentRate>> UpdateAssignmentRate(int Id,AssignmentRate updateObject);
        Task<Response<AssignmentRate>> SaveAssignmentRate(AssignmentRate obj);
        Task DeleteAssignmentRate(int id);
    }
}
