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
    public class LeaveFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdLeave";
        private readonly string UpdateSPName = "usp_InsUpdLeave";
        private readonly string DeleteSPName = "USP_DeleteLeave";
        private readonly string SelectSPName = "USP_GetLeave";
        private readonly string selectLeaveDaysSPName = "usp_GetLeaveDays";
        #endregion

        public LeaveFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            Leave obj1 = obj as Leave;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@ApproverID", obj1.ApproverID);
            base.getStoredProc = SelectSPName;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@LeaveId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@LeaveId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Leave obj1 = obj as Leave;
            base.parms.Add("@LeaveId", obj1.LeaveID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Leave obj1 = obj as Leave;

            base.parms.Add("@LeaveID", obj1.LeaveID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@Title", obj1.Title);
            base.parms.Add("@Detail", obj1.Detail);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@IncludesHalfDay", obj1.IncludesHalfDay);
            base.parms.Add("@LeaveTypeID", obj1.LeaveTypeID);
            base.parms.Add("@RequesterID", obj1.RequesterID);
            base.parms.Add("@ApproverID", obj1.ApproverID);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@Comment", obj1.Comment);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Leave leave = new Leave();
            leave = reader.Read<Leave>().FirstOrDefault();
            leave.LeaveBalances = reader.Read<LeaveBalance>().ToList();
            return (T)Convert.ChangeType(leave, typeof(T));
        }

        //public override Response<T> GetByID<T>(T obj)
        //{
        //    base.parms.Add("@CountryId", (obj as AbstractDataObject).RecordID);
        //    base.getStoredProc = SelectSPName;
        //    return base.GetByID<T>();
        //}

        public Response<T> GetLeaveDays<T>(T obj)
        {
            Leave obj1 = obj as Leave;
            base.parms.Add("@employeeID", obj1.EmployeeID);
            base.parms.Add("@startDate", obj1.StartDate);
            base.parms.Add("@endDate", obj1.EndDate);
            base.parms.Add("@IncludesHalfDay", obj1.IncludesHalfDay);
            base.getStoredProc = selectLeaveDaysSPName;
            return base.GetByID<T>();
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}