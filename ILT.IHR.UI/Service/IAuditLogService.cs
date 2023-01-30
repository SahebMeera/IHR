using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IAuditLogService
    {
        Task<Response<IEnumerable<AuditLog>>> GetAuditLog(int? AuditLogID);
        Task<Response<AuditLog>> GetAuditLogById(int ID);
        Task<Response<AuditLog>> updateAuditLog(int ID, AuditLog updateAuditLog);
        Task<DataTable> GetReportAuditLogInfo(Report report);
       
    }
}
