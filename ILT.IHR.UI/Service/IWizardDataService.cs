using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IWizardDataService
    {
        Task<Response<IEnumerable<ProcessData>>> GetWizardDatas();
        Task<Response<ProcessData>> GetWizardDataByIdAsync(int Id);
        Task<Response<ProcessData>> SaveWizardData(ProcessData obj);
        Task<Response<ProcessData>> UpdateWizardData(int Id, ProcessData updateObject);
    }
}
