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
    public class I9DocumentFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdI9Document";
        private readonly string UpdateSPName = "usp_InsUpdI9Document";
        private readonly string DeleteSPName = "usp_DeleteI9Document";
        private readonly string SelectSPName = "usp_GetI9Document";
        #endregion

        public I9DocumentFactory(string connString, IConfiguration config)
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
            base.parms.Add("@I9DocumentID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@I9DocumentID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            I9Document obj1 = obj as I9Document;
            base.parms.Add("@I9DocumentID", obj1.I9DocumentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            I9Document obj1 = obj as I9Document;

            base.parms.Add("@I9DocumentID", obj1.I9DocumentID);
            base.parms.Add("@I9DocName", obj1.I9DocName);
            base.parms.Add("@I9DocTypeID", obj1.I9DocTypeID);
            base.parms.Add("@WorkAuthID", obj1.WorkAuthID);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            I9Document i9document = new I9Document();
            i9document = reader.Read<I9Document>().FirstOrDefault();            
            return (T)Convert.ChangeType(i9document, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}