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
    public class LeaveAccrualFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdLeaveAccrual";
        private readonly string UpdateSPName = "usp_InsUpdLeaveAccrual";
        private readonly string DeleteSPName = "usp_DeleteLeaveAccrual";
        private readonly string SelectSPName = "USP_GetLeaveAccrual";
        #endregion

        public LeaveAccrualFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            LeaveAccrual obj1 = obj as LeaveAccrual;
            base.parms.Add("@Country", obj1.Country);            
            base.getStoredProc = SelectSPName;          
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@LeaveAccrualId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@LeaveAccrualId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            LeaveAccrual obj1 = obj as LeaveAccrual;
            base.parms.Add("@LeaveAccrualID", obj1.LeaveAccrualID);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            LeaveAccrual obj1 = obj as LeaveAccrual;

            base.parms.Add("@LeaveAccrualID", obj1.LeaveAccrualID);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@AccruedDate", obj1.AccruedDate);
            base.parms.Add("@AccruedValue", obj1.AccruedValue);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            LeaveAccrual leaveAccrual = new LeaveAccrual();
            leaveAccrual = reader.Read<LeaveAccrual>().FirstOrDefault();
            return (T)Convert.ChangeType(leaveAccrual, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}