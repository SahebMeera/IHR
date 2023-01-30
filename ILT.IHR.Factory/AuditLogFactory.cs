using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.Factory
{
    public class AuditLogFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAuditLog";
        private readonly string UpdateSPName = "usp_InsUpdAuditLog";
        private readonly string DeleteSPName = "USP_DeleteAuditLog";
        private readonly string SelectSPName = "USP_GetAuditLog";
        private readonly string LeavesCountSPName = "usp_GetLeavesCount";
        private readonly string LeaveDetailSPName = "usp_GetLeaveDetail";
        #endregion

        public AuditLogFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            AuditLog obj1 = obj as AuditLog;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@AuditLogID", obj1.AuditLogID);
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@RecordID", (obj as AbstractDataObject).RecordID);
            base.parms.Add("@AuditLogID", (obj as AuditLog).AuditLogID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AuditLogID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@AuditLogID", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            AuditLog obj1 = obj as AuditLog;

            base.parms.Add("@RecordId", obj1.RecordId);
            base.parms.Add("@Action", obj1.Action);
            base.parms.Add("@TableName", obj1.TableName);
            base.parms.Add("@Values", obj1.Values);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            AuditLog AuditLog = new AuditLog();
            AuditLog = reader.Read<AuditLog>().FirstOrDefault();
            return (T)Convert.ChangeType(AuditLog, typeof(T));
        }

      
        public Response<List<AuditLog>> GetAuditLogInfo<T>(Report obj)
        {
            Report obj1 = obj as Report;
            base.parms.Add("@startDate", obj1.StartDate);
            base.parms.Add("@endDate", obj1.EndDate);
            base.getStoredProc = SelectSPName;
            return base.GetList<AuditLog>();
        }


        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}


