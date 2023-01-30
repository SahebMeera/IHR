using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.Factory
{
    public class ProcessDataFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdProcessData";
        private readonly string UpdateSPName = "usp_InsUpdProcessData";
        private readonly string DeleteSPName = "USP_DeleteWizardData";
        private readonly string SelectSPName = "USP_GetProcessData";
        #endregion

        public ProcessDataFactory(string connString, IConfiguration config)
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
            base.parms.Add("@ProcessDataId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ProcessDataId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@ProcessDataId", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            ProcessData obj1 = obj as ProcessData;

            base.parms.Add("@ProcessDataID", obj1.ProcessDataID);
            base.parms.Add("@ProcessWizardID", obj1.ProcessWizardID);           
            base.parms.Add("@Data", obj1.Data);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@StatusID", obj1.StatusId);
            base.parms.Add("@ChangeNotificationEmailId", obj1.ChangeNotificationEmailId);
            base.parms.Add("@EmailApprovalValidity", obj1.EmailApprovalValidity);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            ProcessData processData;
            processData = reader.Read<ProcessData>().FirstOrDefault();
            if (processData != null)
            {
                processData.ProcessDataTickets = reader.Read<ProcessDataTicket>().ToList();
            }
                return (T)Convert.ChangeType(processData, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
    }
}
