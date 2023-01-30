using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IStateService
    {
        Task<IEnumerable<State>> GetStates();
        Task<State> GetStateByIdAsync(int Id);
        // Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<State> SaveState(State obj);
        Task<State> UpdateState(int Id, State updateObject);
    }
}
