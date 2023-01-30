using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IAppraisalService
    {
        Task<Response<IEnumerable<Appraisal>>> GetAppraisalList(int EmployeeID);
        Task<Response<Appraisal>> GetAppraisalById(int Id);
        Task<Response<Appraisal>> UpdateAppraisal(int Id,Appraisal updateObject);
        Task<Response<Appraisal>> SaveAppraisal(Appraisal obj);
        Task DeleteAppraisal(int id);
        Task<Response<AppraisalDetail>> GetAppraisalDetailById(int Id);
        Task<Response<AppraisalDetail>> UpdateAppraisalDetail(int Id,AppraisalDetail updateObject);
        Task<Response<AppraisalDetail>> SaveAppraisalDetail(AppraisalDetail obj);
        Task DeleteAppraisalDetail(int id);
    }
}
