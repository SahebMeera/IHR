using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IUserService
    {
        Task<Response<IEnumerable<User>>> GetUsers();
        Task<Response<List<Employee>>> GetEmployees();
        Task<Response<User>> GetUserByIdAsync(int Id);
        Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<Response<User>> UpdateUser(int Id, User updateObject);
        Task<Response<User>> SaveUser(User obj);
        Task<Response<User>> DeleteUser(int id);
    }
}
