using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
   public interface ICountryService
    {
        Task<Response<IEnumerable<Country>>> GetCountries();
        Task<Response<Country>> GetCountryByIdAsync(int Id);
        // Task<Response<Employee>> GetEmployeeByIdAsync(int Id);
        Task<Response<Country>> SaveCountry(Country obj);
        Task<Response<Country>> UpdateCountry(int Id, Country updateObject);
    }
}
