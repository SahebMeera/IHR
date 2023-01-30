using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IContactService
    {
        Task<Response<Contact>> GetContactByIdAsync(int Id, int employeeId);
        Task<Response<Contact>> SaveContact(Contact obj);
        Task<Response<Contact>> UpdateContact(int Id, Contact updateObject);
        Task DeleteContact(int Id);
    }
}
