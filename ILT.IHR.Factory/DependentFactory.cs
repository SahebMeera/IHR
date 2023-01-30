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
    public class DependentFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdDependent";
        private readonly string UpdateSPName = "usp_InsUpdDependent";
        private readonly string DeleteSPName = "USP_DeleteDependent";
        private readonly string SelectSPName = "USP_GetDependent";
        #endregion

        public DependentFactory(string connString, IConfiguration config)
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
            base.parms.Add("@DependentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@DependentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@DependentId", obj1.UserID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Dependent obj1 = obj as Dependent;

            base.parms.Add("@DependentID", obj1.DependentID);
            base.parms.Add("@FirstName", obj1.FirstName);
            base.parms.Add("@MiddleName", obj1.MiddleName);
            base.parms.Add("@LastName", obj1.LastName);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@RelationID", obj1.RelationID);
            base.parms.Add("@BirthDate", obj1.BirthDate);
            base.parms.Add("@VisaTypeID", obj1.VisaTypeID);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));

            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Dependent dependent = new Dependent();
            dependent = reader.Read<Dependent>().FirstOrDefault();
            return (T)Convert.ChangeType(dependent, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}