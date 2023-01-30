using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public interface ILookupService
    {
        Task<Response<IEnumerable<ListType>>> GetLookups();
        Task<Response<IEnumerable<ListValue>>> GetListValues();
        Task<Response<ListType>> GetLookupByIdAsync(int Id);
        Task<Response<ListValue>> GetLookupValueByIdAsync(int Id);
        Task<Response<ListValue>> UpdateLookupValue(int Id, ListValue updateObject);
        Task<Response<ListValue>> SaveListValue(ListValue obj);
        Task DeleteListValue(int id);
        //Task<T> SaveAsync(string requestUri, T obj);
        //Task<T> UpdateAsync(string requestUri, int Id, T obj);
        //Task<bool> DeleteAsync(string requestUri, int Id);
    }
}
