using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IEndClientService
    {
        Task<Response<IEnumerable<EndClient>>> GetEndClients();
        Task<Response<EndClient>> GetEndClientByIdAsync(int Id);
        Task<Response<EndClient>> UpdateEndClient(int Id, EndClient updateObject);
        Task<Response<EndClient>> SaveEndClient(EndClient obj);
        Task<Response<EndClient>> DeleteEndClient(int id);
    }
}
