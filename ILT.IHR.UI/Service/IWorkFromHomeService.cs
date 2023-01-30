using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IWorkFromHomeService
    {
        Task<Response<IEnumerable<WFH>>> GetWFH(string Parameter = "", int ID = 0);
        Task<Response<WFH>> GetWFHByIdAsync(int Id,int EmployeeID,int ApproverID);
        Task<Response<WFH>> SaveWFH(WFH obj);
        Task<Response<WFH>> UpdateWFH(int Id, WFH updateObject);
    }
}
