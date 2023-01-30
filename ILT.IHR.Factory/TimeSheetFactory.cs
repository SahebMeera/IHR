using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ILT.IHR.Factory
{
    public class TimeSheetFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdTimeSheet";
        private readonly string UpdateSPName = "usp_InsUpdTimeSheet";
        private readonly string DeleteSPName = "USP_DeleteTimeSheet";
        private readonly string SelectSPName = "USP_GetTimeSheet";

        public TimeSheetFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }
        #endregion

        public override Response<List<T>> GetList<T>(T obj)
        {
            TimeSheet obj1 = obj as TimeSheet;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@SubmittedByID", obj1.SubmittedByID);
            base.parms.Add("@DocGuid", obj1.DocGuid);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.getStoredProc = SelectSPName;           
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@TimeSheetID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@TimeSheetID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            TimeSheet obj1 = obj as TimeSheet;
            base.parms.Add("@TimeSheetID", obj1.TimeSheetID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@WeekEnding", obj1.WeekEnding);
            base.parms.Add("@ClientID", obj1.ClientID);
            base.parms.Add("@AssignmentID", obj1.AssignmentID);
            base.parms.Add("@TotalHours", obj1.TotalHours);
            base.parms.Add("@FileName", obj1.FileName);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@SubmittedByID", obj1.SubmittedByID);
            base.parms.Add("@SubmittedDate", obj1.SubmittedDate);
            //base.parms.Add("@ApprovedByID", obj1.ApprovedByID);
            base.parms.Add("@ApprovedByEmail", obj1.ApprovedByEmail);
            base.parms.Add("@ApprovedDate", obj1.ApprovedDate);
            base.parms.Add("@ClosedByID", obj1.ClosedByID);
            base.parms.Add("@ClosedDate", obj1.ClosedDate);
            base.parms.Add("@Comment", obj1.Comment);
            base.parms.Add("@xmlTimeEntry", ConvertToXML(obj1.TimeEntries));
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        public override Response<T> Delete<T>(T obj)
        {
            AssignmentRate obj1 = obj as AssignmentRate;
            base.parms.Add("@TimeSheetID", obj1.AssignmentRateID);
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            TimeSheet timeSheet = new TimeSheet();
            timeSheet = reader.Read<TimeSheet>().FirstOrDefault();
            if (timeSheet != null)
            {
                timeSheet.TimeEntries = reader.Read<TimeEntry>().ToList();
            }
            return (T)Convert.ChangeType(timeSheet, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

        public string  ConvertToXML<T>(List<T> items)
        {
            string xml = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                xs.Serialize(sw, items);
                xml = sw.ToString();
            }
            return xml;
        }

    }
}
