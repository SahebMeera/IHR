using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace ILT.IHR.Factory
{
    public class NotificationFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string SelectSPName = "USP_GetNotification";
        private readonly string UpdateSPName = "usp_InsUpdNotification";
        #endregion
        public NotificationFactory(string connString, IConfiguration config)
           : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            base.getStoredProc = SelectSPName;
            Notification obj1 = obj as Notification;
            base.parms.Add("@TableName", obj1.TableName);
            base.parms.Add("@UserID", obj1.UserID);
            return base.GetList<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override Response<T> Save<T>(T obj)
        {
            Notification obj1 = obj as Notification;
            base.parms.Add("@TableName", obj1.TableName);
            base.parms.Add("@RecordID", obj1.RecordID);
            base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = UpdateSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
