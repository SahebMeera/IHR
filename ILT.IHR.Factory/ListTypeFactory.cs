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
    public class ListTypeFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdListType";
        private readonly string UpdateSPName = "usp_InsUpdListType";
        private readonly string DeleteSPName = "USP_DeleteListType";
        private readonly string SelectSPName = "USP_GetListType";
        #endregion

        public ListTypeFactory(string connString, IConfiguration config)
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
            base.parms.Add("@ListTypeID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ListTypeID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            ListType obj1 = obj as ListType;
            base.parms.Add("@ListTypeID", obj1.ListTypeID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            
            base.getStoredProc = DeleteSPName;
            
            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            ListType obj1 = obj as ListType;

            base.parms.Add("@ListTypeID", obj1.ListTypeID);
            base.parms.Add("@Type", obj1.Type);
            base.parms.Add("@TypeDesc", obj1.TypeDesc);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            ListType listType = new ListType();
            listType = reader.Read<ListType>().FirstOrDefault();
            listType.ListValues = reader.Read<ListValue>().ToList();
            return (T)Convert.ChangeType(listType, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}