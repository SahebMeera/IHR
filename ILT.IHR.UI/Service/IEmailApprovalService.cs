using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
   public interface IEmailApprovalService
    {
        Task<Response<IEnumerable<EmailApproval>>> GetEmailApprovals();
        Task<Response<EmailApproval>> GetEmailApprovalByIdAsync(Guid Id);        
        Task<Response<EmailApproval>> SaveEmailApproval(EmailApproval obj);
        Task<Response<EmailApproval>> UpdateEmailApproval(int Id, EmailApproval updateObject);
        Task<string> EamilApprovalAction(string ClientID, Guid LinkID, string Value, string Module="Timesheet");
    }
}
