using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public interface IFormI9ChangesetService
    {
        Task<Response<IEnumerable<FormI9ChangeSet>>> GetFormI9Changeset(int formi9id);
    }
}
