using System;
using System.Collections.Generic;
using System.Data;
using ILT.IHR.DTO;
using ILT.IHR.Factory;

namespace ITL.IHR.Factory
{
    public class ModuleFactory : AbstractFactory
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdModule";
        private readonly string UpdateSPName = "usp_InsUpdModule";
        private readonly string DeleteSPName = "USP_DeleteModule";
        private readonly string SelectSPName = "USP_GetModule";
        #endregion

        public ModuleFactory(string connString)
            : base(connString)
        {
        }

        public override List<T> GetList<T>(T obj)
        {
            List<T> GetListData = new List<T>();
            base.SQLServerConnObj.ClearParameters();
            base.getStoredProc = SelectSPName;
            GetListData = base.GetList<T>();
            return GetListData;
        }

        public override T GetByID<T>(T obj)
        {
            T retObj = default(T);
            base.getStoredProc = SelectSPName;
            retObj = base.GetByID<T>();
            return retObj;
        }

        public override T GetRelatedObjectsByID<T>(T obj)
        {
            T retObj = default(T);
            base.SQLServerConnObj.ClearParameters();
            base.SQLServerConnObj.AddParameter("@ModuleId", (obj as AbstractDataObject).RecordID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.getStoredProc = SelectSPName;

            retObj = base.GetRelatedObjectsByID<T>();

            return retObj;
        }

        public override bool Delete<T>(T obj)
        {
            bool flag = false;
            //To fetch existing value and compare it with new enter value
            base.SQLServerConnObj.ClearParameters();
            base.SQLServerConnObj.AddParameter("@ModuleId", (obj as AbstractDataObject).RecordID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@ModifiedBy", (obj as AbstractDataObject).ModifiedBy, SqlDbType.VarChar, 50, ParameterDirection.Input);
            base.getStoredProc = DeleteSPName;

            flag = base.DeleteInstance(obj);

            return flag;
        }

        public override bool Save<T>(T obj1)
        {
            bool flag = false;
            int ReturnCode = 0;
            Module obj = obj1 as Module;

            base.SQLServerConnObj.ClearParameters();
            
            base.getStoredProc = InsertSPName;
            flag = base.SaveInstanceReturnOutput<T>(obj1);

            return flag;
        }

        protected override T LoadRelatedObjects<T>(System.Data.DataSet DS)
        {
            Module value = new Module();
            value = CollectionHelper.ConvertTo<Module>(DS.Tables[0])[0];
            //if (DS.Tables.Count > 1)
               //
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}