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
    public class WFHFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdWFH";
        private readonly string UpdateSPName = "usp_InsUpdWFH";
        private readonly string DeleteSPName = "USP_DeleteWFH";
        private readonly string SelectSPName = "USP_GetWFH";
        #endregion

        public WFHFactory(string connString, IConfiguration config) : base(connString, config )
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            WFH obj1 = obj as WFH;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@ApproverID", obj1.ApproverID);
            base.getStoredProc = SelectSPName;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@WFHId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@WFHId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            WFH obj1 = obj as WFH;
            base.parms.Add("@WFHId", obj1.WFHID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            WFH obj1 = obj as WFH;

            base.parms.Add("@WFHID", obj1.WFHID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@Title", obj1.Title);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@RequesterID", obj1.RequesterID);
            base.parms.Add("@ApproverID", obj1.ApproverID);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@Comment", obj1.Comment);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            WFH WFH = new WFH();
            WFH = reader.Read<WFH>().FirstOrDefault();
            return (T)Convert.ChangeType(WFH, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}