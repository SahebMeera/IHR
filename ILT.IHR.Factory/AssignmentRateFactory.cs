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
    public class AssignmentRateFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAssignmentRate";
        private readonly string UpdateSPName = "usp_InsUpdAssignmentRate";
        private readonly string DeleteSPName = "usp_DeleteAssignmentRate";
        private readonly string SelectSPName = "USP_GetAssignmentRate";
        #endregion

        public AssignmentRateFactory(string connString, IConfiguration config)
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
            base.parms.Add("@AssignmentRateId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AssignmentRateId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            AssignmentRate obj1 = obj as AssignmentRate;
            base.parms.Add("@AssignmentRateID", obj1.AssignmentRateID);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            AssignmentRate obj1 = obj as AssignmentRate;

            base.parms.Add("@AssignmentRateID", obj1.AssignmentRateID);
            base.parms.Add("@AssignmentID", obj1.AssignmentID);
            base.parms.Add("@BillingRate", obj1.BillingRate);
            base.parms.Add("@PaymentRate", obj1.PaymentRate);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            AssignmentRate assignmentRate = new AssignmentRate();
            assignmentRate = reader.Read<AssignmentRate>().FirstOrDefault();
            return (T)Convert.ChangeType(assignmentRate, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}