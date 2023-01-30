using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.Factory
{
    public class FormI9ChangeSetFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "";
        private readonly string UpdateSPName = "";
        private readonly string DeleteSPName = "";
        private readonly string SelectSPName = "usp_GetFormI9ChangeSets";
        #endregion

        public FormI9ChangeSetFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }
        public override Response<T> Delete<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            FormI9ChangeSet obj1 = obj as FormI9ChangeSet;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@FormI9ID", obj1.FormI9ID);
            return base.GetList<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<T> Save<T>(T obj)
        {
            throw new NotImplementedException();
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
