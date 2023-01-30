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
    public class DepartmentFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdDepartment";
        private readonly string UpdateSPName = "usp_InsUpdDepartment";
        private readonly string DeleteSPName = "USP_DeleteDepartment";
        private readonly string SelectSPName = "USP_GetDepartment";
        #endregion

        public DepartmentFactory(string connString, IConfiguration config)
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
            base.parms.Add("@AssignmentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AssignmentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            Department obj1 = obj as Department;
            base.parms.Add("@DepartmentId", obj1.DepartmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Department obj1 = obj as Department;

            base.parms.Add("@DepartmentID", obj1.DepartmentID);
            base.parms.Add("@DeptCode", obj1.DeptCode);
            base.parms.Add("@DeptName", obj1.DeptName);
            base.parms.Add("@DeptLocationID", obj1.DeptLocationID);
            base.parms.Add("@IsActive", obj1.IsActive);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Department department = new Department();
            department = reader.Read<Department>().FirstOrDefault();
           //  department.AssignmentRates = reader.Read<AssignmentRate>().ToList();
            return (T)Convert.ChangeType(department, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}