using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class UserFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdUser";
        private readonly string UpdateSPName = "usp_InsUpdUser";
        private readonly string DeleteSPName = "USP_DeleteUser";
        private readonly string SelectSPName = "USP_GetUser";

        #endregion

        public UserFactory(string connString, IConfiguration config)
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
            base.parms.Add("@UserID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@UserID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }


        public override Response<T> Save<T>(T obj)
        {
            User obj1 = obj as User;

            base.parms.Add("@UserID", obj1.RecordID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@FirstName", obj1.FirstName);
            base.parms.Add("@LastName", obj1.LastName);
            base.parms.Add("@xmlUserRole", ConvertToXML(obj1.UserRoles));
            base.parms.Add("@Email", obj1.Email);
            base.parms.Add("@Password", obj1.Password);
            base.parms.Add("@IsOAuth", obj1.IsOAuth);
            base.parms.Add("@IsActive", obj1.IsActive);
            base.parms.Add("@CompanyID", obj1.CompanyID);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);           
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID ,direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            User user = new User();           
            user = reader.Read<User>().FirstOrDefault();
            user.UserRoles = reader.Read<UserRole>().ToList();
            user.RolePermissions = reader.Read<RolePermission>().ToList();               
                

            return (T)Convert.ChangeType(user, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

        public Response<T> ValidateUser<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@Email", obj1.Email);
            //base.parms.Add("@Password", obj1.Password);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public string ConvertToXML<T>(List<T> items)
        {
            string xml = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                xs.Serialize(sw, items);
                xml = sw.ToString();
            }
            return xml;
        }
    }
}