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
    public class EmployeeW4Factory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEmployeeW4";
        private readonly string UpdateSPName = "usp_InsUpdEmployeeW4";
        private readonly string DeleteSPName = "USP_DeleteEmployeeW4";
        private readonly string SelectSPName = "USP_GetEmployeeW4";
        #endregion

        public EmployeeW4Factory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            EmployeeW4 obj1 = obj as EmployeeW4;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@EmployeeW4Id", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EmployeeW4Id", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            EmployeeW4 obj1 = obj as EmployeeW4;
            base.parms.Add("@EmployeeW4Id", obj1.EmployeeW4ID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            EmployeeW4 obj1 = obj as EmployeeW4;
   
            base.parms.Add("@EmployeeW4ID", obj1.EmployeeW4ID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@SSN", obj1.SSN);
            base.parms.Add("@W4TypeID", obj1.W4TypeID);
            base.parms.Add("@WithHoldingStatusID", obj1.WithHoldingStatusID);
            base.parms.Add("@Allowances", obj1.Allowances);
            base.parms.Add("@IsMultipleJobsOrSpouseWorks", obj1.IsMultipleJobsOrSpouseWorks);
            base.parms.Add("@QualifyingChildren", obj1.QualifyingChildren);
            base.parms.Add("@OtherDependents", obj1.OtherDependents);
            base.parms.Add("@OtherIncome", obj1.OtherIncome); 
            base.parms.Add("@Deductions", obj1.Deductions);
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
            EmployeeW4 EmployeeW4 = new EmployeeW4();
            EmployeeW4 = reader.Read<EmployeeW4>().FirstOrDefault();
            return (T)Convert.ChangeType(EmployeeW4, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}