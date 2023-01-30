using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using System.Data;

namespace ILT.IHR.UI.Service
{
    public interface IFormI9Service
    {
        Task<Response<IEnumerable<FormI9>>> GetFormI9(int EmployeeID);
       
        Task<Response<FormI9>> GetFormI9ByIdAsync(int Id);
        Task<Response<FormI9>> SaveFormI9(FormI9 obj);
        Task<Response<FormI9>> UpdateFormI9(int Id, FormI9 updateObject);
        Task<DataTable> GetI9ExpiryForm(DateTime expirydate);
    }
}
