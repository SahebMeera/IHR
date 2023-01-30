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
    public class LeaveBalanceFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdLeaveBalance";
        private readonly string UpdateSPName = "usp_InsUpdLeaveBalance";
        private readonly string DeleteSPName = "USP_DeleteLeaveBalance";
        private readonly string SelectSPName = "USP_GetLeaveBalance";
        private readonly string LeavesCountSPName = "usp_GetLeavesCount";
        private readonly string LeaveDetailSPName = "usp_GetLeaveDetail";
        #endregion

        public LeaveBalanceFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            LeaveBalance obj1 = obj as LeaveBalance;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@LeaveBalanceId", (obj as AbstractDataObject).RecordID);
            base.parms.Add("@EmployeeID", (obj as LeaveBalance).EmployeeID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@LeaveBalanceId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@LeaveBalanceId", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            LeaveBalance obj1 = obj as LeaveBalance;

            base.parms.Add("@LeaveBalanceID", obj1.LeaveBalanceID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@LeaveYear", obj1.LeaveYear);
            base.parms.Add("@LeaveTypeID", obj1.LeaveTypeID);
            base.parms.Add("@VacationTotal", obj1.VacationTotal);
            base.parms.Add("@VacationUsed", obj1.VacationUsed);
            base.parms.Add("@EncashedLeave", obj1.EncashedLeave);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            LeaveBalance LeaveBalance = new LeaveBalance();
            LeaveBalance = reader.Read<LeaveBalance>().FirstOrDefault();
            return (T)Convert.ChangeType(LeaveBalance, typeof(T));
        }

        public Response<List<LeaveBalance>> GetLeavesCount<T>(Report obj)
        {
            Report obj1 = obj as Report;
            base.parms.Add("@startDate", obj1.StartDate);
            base.parms.Add("@endDate", obj1.EndDate);
            base.parms.Add("@Country", obj1.Country);
            base.getStoredProc = LeavesCountSPName;
            return base.GetList<LeaveBalance>();
        }

        public Response<List<LeaveBalance>> GetLeaveDetail<T>(Report obj)
        {
            Report obj1 = obj as Report;
            base.parms.Add("@startDate", obj1.StartDate);
            base.parms.Add("@endDate", obj1.EndDate);
            base.parms.Add("@Country", obj1.Country);
            base.getStoredProc = LeaveDetailSPName;
            return base.GetList<LeaveBalance>();
        }


        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}