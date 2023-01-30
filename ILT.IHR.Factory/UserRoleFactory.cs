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
    public class UserRoleFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdUserRole";
        private readonly string UpdateSPName = "usp_InsUpdUserRole";
        private readonly string DeleteSPName = "usp_DeleteUserRole";
        private readonly string SelectSPName = "usp_GetUserRole";
        #endregion

        public UserRoleFactory(string connString, IConfiguration config)
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
            UserRole obj1 = obj as UserRole;
            base.parms.Add("@UserRoleID", (obj1 as AbstractDataObject).RecordID);
            base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@RoleID", obj1.RoleID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            UserRole obj1 = obj as UserRole;
            base.parms.Add("@UserRoleID", (obj1 as AbstractDataObject).RecordID);
            base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@RoleID", obj1.RoleID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@UserRoleID", obj1.UserID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.getStoredProc = DeleteSPName;
            return base.DeleteInstance<T>();
        }


        public override Response<T> Save<T>(T obj)
        {
            UserRole obj1 = obj as UserRole;

            base.parms.Add("@UserRoleID", obj1.RecordID);
            base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@RoleID", obj1.RoleID);
            base.parms.Add("@RoleShort", obj1.RoleShort);
            base.parms.Add("@RoleName", obj1.RoleName);
            base.parms.Add("@IsDefault", obj1.IsDefault);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);           
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID ,direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            UserRole userRole = new UserRole();
            return (T)Convert.ChangeType(userRole, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
        
    }
}