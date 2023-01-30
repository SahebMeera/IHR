using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class HolidayFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdHoliday";
        private readonly string UpdateSPName = "usp_InsUpdHoliday";
        private readonly string DeleteSPName = "USP_DeleteHoliday";
        private readonly string SelectSPName = "USP_GetHoliday";
        #endregion

        public HolidayFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            base.getStoredProc = SelectSPName;
            base.parms = null;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@HolidayId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@HolidayId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Holiday obj1 = obj as Holiday;
            base.parms.Add("@HolidayId", obj1.HolidayID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Holiday obj1 = obj as Holiday;

            base.parms.Add("@HolidayID", obj1.HolidayID);
            base.parms.Add("@Name", obj1.Name);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Holiday Holiday = new Holiday();
            Holiday = reader.Read<Holiday>().FirstOrDefault();
            return (T)Convert.ChangeType(Holiday, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}