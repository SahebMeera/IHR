using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IExpenseService
    {
        Task <Response<IEnumerable<Expense>>> GetExpenses();
        Task<Response<Expense>> GetExpenseByIdAsync(int Id);
        Task<Response<Expense>> UpdateExpense(int Id, Expense updateObject);
        Task<Response<Expense>> SaveExpense(Expense obj);
        Task<Response<Expense>> DeleteExpense(int id);
        Task<Response<FileDownloadResponse>> DownloadFile(string Client, string FileName);
    }
}
