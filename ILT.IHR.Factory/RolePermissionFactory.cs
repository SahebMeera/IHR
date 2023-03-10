using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ITL.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.Factory
{
    public class RolePermissionFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdRolePermission";
        private readonly string UpdateSPName = "usp_InsUpdRolePermission";
        private readonly string DeleteSPName = "usp_DeleteRolePermission";
        private readonly string SelectSPName = "usp_GetRolePermission";
        #endregion

        public RolePermissionFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T Usr)
        {
            base.getStoredProc = SelectSPName;
            base.parms = null;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@RolePermissionID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@RolePermissionID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@RolePermissionID", (obj as AbstractDataObject).RecordID);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            RolePermission obj1 = obj as RolePermission;
            base.parms.Add("@RolePermissionID", obj1.RolePermissionID);
            base.parms.Add("@RoleId", obj1.RoleID);
            base.parms.Add("@ModuleID", obj1.ModuleID);
            base.parms.Add("@View", obj1.View);
            base.parms.Add("@Add", obj1.Add);
            base.parms.Add("@Update", obj1.Update);
            base.parms.Add("@Delete", obj1.Delete);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            User user = new User();
            user = reader.Read<User>().FirstOrDefault();
            user.RolePermissions = reader.Read<RolePermission>().ToList();
            return (T)Convert.ChangeType(user, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}
