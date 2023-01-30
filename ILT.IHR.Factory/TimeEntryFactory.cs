using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ILT.IHR.Factory
{
    public class TimeEntryFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdTimeEntry";
        private readonly string UpdateSPName = "usp_InsUpdTimeEntry";
        private readonly string DeleteSPName = "USP_DeleteTimeEntry";
        private readonly string SelectSPName = "USP_GetTimeEntry";
        #endregion

        public TimeEntryFactory(string connString, IConfiguration config)
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
            base.parms.Add("@TimeEntryID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@TimeEntryID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            TimeEntry obj1 = obj as TimeEntry;
            base.parms.Add("@TimeSheetID", obj1.TimeSheetID);
            base.parms.Add("@Project", obj1.Project);
            base.parms.Add("@Activity", obj1.Activity);
            base.parms.Add("@WorkDate", obj1.WorkDate);
            base.parms.Add("@Hours", obj1.Hours);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        public override Response<T> Delete<T>(T obj)
        {
            AssignmentRate obj1 = obj as AssignmentRate;
            base.parms.Add("@TimeEntryID", obj1.AssignmentRateID);
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry = reader.Read<TimeEntry>().FirstOrDefault();
            return (T)Convert.ChangeType(timeEntry, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
    }
}
