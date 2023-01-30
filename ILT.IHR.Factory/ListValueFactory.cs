using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace ITL.IHR.Factory
{
    public class ListValueFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdListValue";
        private readonly string UpdateSPName = "usp_InsUpdListValue";
        private readonly string DeleteSPName = "usp_DeleteListValue";
        private readonly string SelectSPName = "USP_GetListValue";
        #endregion

        public ListValueFactory(string connString, IConfiguration config)
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
            base.parms.Add("@ListValueID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ListValueID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            ListValue obj1 = obj as ListValue;
            base.parms.Add("@ListValueId", obj1.ListTypeID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }


        public override Response<T> Save<T>(T obj)
        {
            ListValue obj1 = obj as ListValue;

            base.parms.Add("@ListValueID", obj1.ListValueID);
            base.parms.Add("@ListTypeID", obj1.ListTypeID);
            base.parms.Add("@Value", obj1.Value);
            base.parms.Add("@ValueDesc", obj1.ValueDesc);
            base.parms.Add("@IsActive", obj1.IsActive);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            //base.parms.Add("@TimeStamp", obj1.TimeStamp);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));

            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            ListValue value = new ListValue();
            List<ListValue> lstValue = reader.Read<ListValue>().ToList();
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}