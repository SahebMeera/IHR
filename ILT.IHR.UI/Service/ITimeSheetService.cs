using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ITimeSheetService
    {
        Task<Response<IEnumerable<TimeSheet>>> GetTimeSheets(int EmployeeID, int SubmittedByID, int? StatusID = null);
        Task<Response<TimeSheet>> GetTimeSheetByIdAsync(int Id);
        Task<Response<TimeSheet>> UpdateTimeSheet(int Id, TimeSheet updateObject);
        Task<Response<TimeSheet>> SaveTimeSheet(TimeSheet obj);
        Task<Response<TimeSheet>> DeleteTimeSheet(int id);
        Task<Response<FileDownloadResponse>> DownloadFile(string Client, Guid Doc);
    }
}
