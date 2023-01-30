using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ITimeEntryService
    {
        Task<Response<List<TimeEntry>>> GetTimeEntries();
        Task<Response<TimeEntry>> GetTimeEntryByIdAsync(int Id);
        Task<Response<TimeEntry>> UpdateTimeEntry(int Id, TimeEntry updateObject);
        Task<Response<TimeEntry>> SaveTimeEntry(TimeEntry obj);
        Task<Response<TimeEntry>> DeleteTimeEntry(int id);
    }
}
