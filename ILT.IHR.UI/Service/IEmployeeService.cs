using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IEmployeeService
    {
        Task<Response<IEnumerable<Employee>>> GetEmployees();
        Task<Response<IEnumerable<Department>>> GetDepartments();
        Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<Response<Employee>> SaveEmployee(Employee obj);
        Task<Response<Employee>> UpdateEmployee(int Id, Employee updateObject);
        Task<Response<IEnumerable<Employee>>> GetEmployeeInfo();
        //Task<T> SaveAsync(string requestUri, T obj);
        //Task<T> UpdateAsync(string requestUri, int Id, T obj);
        //Task<bool> DeleteAsync(string requestUri, int Id);
    }
}
