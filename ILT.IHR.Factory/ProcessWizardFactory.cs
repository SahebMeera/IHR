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
    public class ProcessWizardFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdWizard";
        private readonly string UpdateSPName = "usp_InsUpdWizard";
        private readonly string DeleteSPName = "USP_DeleteWizard";
        private readonly string SelectSPName = "USP_GetProcessWizard";
        #endregion

        public ProcessWizardFactory(string connString, IConfiguration config)
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
            base.parms.Add("@ProcessWizardId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ProcessWizardId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@ProcessWizardId", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            ProcessWizard obj1 = obj as ProcessWizard;

            base.parms.Add("@ProcessWizardID", obj1.ProcessWizardID);
            base.parms.Add("@Process", obj1.Process);
            base.parms.Add("@Elements", obj1.Elements);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            ProcessWizard wizard = new ProcessWizard();
            wizard = reader.Read<ProcessWizard>().FirstOrDefault();           
            return (T)Convert.ChangeType(wizard, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
    }
}
