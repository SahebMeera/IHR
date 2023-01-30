using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ITicketService
    {
        Task<Response<IEnumerable<Ticket>>> GetTickets();
        Task<Response<IEnumerable<Ticket>>> GetTicketsList(int RequestedByID, int? AssignedToID);
        Task<Response<Ticket>> GetTicketByIdAsync(int Id);
        Task<Response<Ticket>> UpdateTicket(int Id, Ticket updateObject);
        Task<Response<Ticket>> SaveTicket(Ticket obj);
        Task<Response<Ticket>> DeleteTicket(int id);
    }
}
