using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.Factory
{
    public class EndClientFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEndClient";
        private readonly string UpdateSPName = "usp_InsUpdEndClient";
        private readonly string DeleteSPName = "USP_DeleteEndClient";
        private readonly string SelectSPName = "USP_GetEndClient";
        #endregion

        public EndClientFactory(string connString, IConfiguration config)
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
            base.parms.Add("@EndClientID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EndClientID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            EndClient obj1 = obj as EndClient;
            base.parms.Add("@EndClientId", obj1.RecordID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            EndClient obj1 = obj as EndClient;
            base.parms.Add("@EndClientID", obj1.EndClientID);
            base.parms.Add("@Name", obj1.Name);
            base.parms.Add("@TaxID", obj1.TaxID);
            base.parms.Add("@CompanyID", obj1.CompanyID);
            base.parms.Add("@Address1", obj1.Address1);
            base.parms.Add("@Address2", obj1.Address2);
            base.parms.Add("@City", obj1.City);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@ZipCode", obj1.ZipCode); 
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));

            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            EndClient endclient = new EndClient();
            //endclient = reader.Read<EndClient>().FirstOrDefault();
            //endclient. = reader.Read<DTO>().ToList();
            return (T)Convert.ChangeType(endclient, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
    }
}
