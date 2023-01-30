using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace ILT.IHR.Factory
{
    public class UserNotificationFactory : AbstractFactory1
    {
        #region ProcedureNames
        private readonly string SelectSPName = "USP_GetUserNotification";
        private readonly string UpdateSPName = "usp_InsUpdUserNotification";
       
        #endregion
        public UserNotificationFactory(string connString, IConfiguration config)
           : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            base.getStoredProc = SelectSPName;
            UserNotification obj1 = obj as UserNotification;
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
            UserNotification obj1 = obj as UserNotification;
            base.parms.Add("@NotificationID", obj1.NotificationID);
            base.parms.Add("@IsAck", obj1.IsAck);
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
