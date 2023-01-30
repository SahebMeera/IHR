using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;


namespace ILT.IHR.UI.Service
{
    public interface IWizardService
    {
        Task<Response<IEnumerable<ProcessWizard>>> GetWizards();
        Task<Response<ProcessWizard>> GetWizardByIdAsync(int Id);      
        Task<Response<ProcessWizard>> SaveWizard(ProcessWizard obj);
        Task<Response<ProcessWizard>> UpdateWizard(int Id, ProcessWizard updateObject);
    }
}
