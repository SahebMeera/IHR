using System;
using System.Collections.Generic;
using System.Data;
using ILT.IHR.DTO;
using ILT.IHR.Factory;

namespace ITL.IHR.Factory
{
    public class StateFactory : AbstractFactory
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdState";
        private readonly string UpdateSPName = "usp_InsUpdState";
        private readonly string DeleteSPName = "USP_DeleteState";
        private readonly string SelectSPName = "USP_GetState";
        #endregion

        public StateFactory(string connString)
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
            base.SQLServerConnObj.AddParameter("@StateId", (obj as AbstractDataObject).RecordID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.getStoredProc = SelectSPName;

            retObj = base.GetRelatedObjectsByID<T>();

            return retObj;
        }

        public override bool Delete<T>(T obj)
        {
            bool flag = false;
            //To fetch existing value and compare it with new enter value
            base.SQLServerConnObj.ClearParameters();
            base.SQLServerConnObj.AddParameter("@StateId", (obj as AbstractDataObject).RecordID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@ModifiedBy", (obj as AbstractDataObject).ModifiedBy, SqlDbType.VarChar, 50, ParameterDirection.Input);
            base.getStoredProc = DeleteSPName;

            flag = base.DeleteInstance(obj);

            return flag;
        }

        public override bool Save<T>(T obj1)
        {
            bool flag = false;
            int ReturnCode = 0;
            State obj = obj1 as State;

            base.SQLServerConnObj.ClearParameters();
            base.SQLServerConnObj.AddParameter("@StateID", obj.StateID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@CountryID", obj.CountryID, SqlDbType.Int, sizeof(int), ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@StateShort", obj.StateShort, SqlDbType.NVarChar, 3, ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@StateDesc", obj.StateDesc, SqlDbType.NVarChar, 50, ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@CreatedBy", obj.CreatedBy, SqlDbType.NVarChar, 50, ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@ModifiedBy", obj.ModifiedBy, SqlDbType.NVarChar, 50, ParameterDirection.Input);
            base.SQLServerConnObj.AddParameter("@ReturnCode", @ReturnCode, SqlDbType.Int, sizeof(int), ParameterDirection.Output);

            base.getStoredProc = InsertSPName;
            flag = base.SaveInstanceReturnOutput<T>(obj1);

            return flag;
        }

        protected override T LoadRelatedObjects<T>(System.Data.DataSet DS)
        {
            State value = new State();
            List<State> obj = CollectionHelper.ConvertTo<State>(DS.Tables[0]);
            //value.States = obj;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}