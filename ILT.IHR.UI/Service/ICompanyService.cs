using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface ICompanyService
    {
        Task <Response<IEnumerable<Company>>> GetCompanies();
        Task<Response<Company>> GetCompanyByIdAsync(int Id);
        Task<Response<Company>> UpdateCompany(int Id, Company updateObject);
        Task<Response<Company>> SaveCompany(Company obj);
        Task<Response<Company>> DeleteCompany(int id);
    }
}
