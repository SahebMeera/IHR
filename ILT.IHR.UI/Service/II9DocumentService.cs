using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
   public interface II9DocumentService
    {
        Task<Response<IEnumerable<I9Document>>> GetI9Documents();
        Task<Response<I9Document>> GetI9DocumentByIdAsync(int Id);        
    }
}
