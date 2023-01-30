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
    public class AssetFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAsset";
        private readonly string UpdateSPName = "usp_InsUpdAsset";
        private readonly string DeleteSPName = "USP_DeleteAsset";
        private readonly string SelectSPName = "USP_GetAsset";
        #endregion

        public AssetFactory(string connString, IConfiguration config)
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
            base.parms.Add("@AssetID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AssetID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Asset obj1 = obj as Asset;
            base.parms.Add("@AssetID", obj1.AssetID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            
            Asset obj1 = obj as Asset;
          

            base.parms.Add("@AssetID", obj1.AssetID);
            base.parms.Add("@AssetTypeID", obj1.AssetTypeID);
            base.parms.Add("@Tag", obj1.Tag);
            base.parms.Add("@Make", obj1.Make);
            base.parms.Add("@Model", obj1.Model);
            base.parms.Add("@WiFiMAC ", obj1.WiFiMAC);
            base.parms.Add("@LANMAC", obj1.LANMAC);
            base.parms.Add("@OS", obj1.OS);
            base.parms.Add("@Configuration", obj1.Configuration);
            base.parms.Add("@PurchaseDate", obj1.PurchaseDate);
            base.parms.Add("@WarantyExpDate", obj1.WarantyExpDate);
            base.parms.Add("@AssignedToID", obj1.AssignedToID);
            base.parms.Add("@AssignedTo", obj1.AssignedTo);
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
            Asset Asset = new Asset();
            Asset = reader.Read<Asset>().FirstOrDefault();
            return (T)Convert.ChangeType(Asset, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}