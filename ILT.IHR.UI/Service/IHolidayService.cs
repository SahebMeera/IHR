using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IHolidayService
    {
        Task<Response<IEnumerable<Holiday>>> GetHolidays();
        Task<Response<Holiday>> GetHolidayByIdAsync(int Id);
        Task<Response<Holiday>> UpdateHoliday(int Id, Holiday updateObject);
        Task<Response<Holiday>> SaveHoliday(Holiday obj);
        Task<Response<Holiday>> DeleteHoliday(int id);
    }
}
