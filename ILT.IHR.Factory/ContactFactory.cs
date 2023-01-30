using Dapper;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace ILT.IHR.Factory
{
    public class ContactFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdContact";
        private readonly string UpdateSPName = "usp_InsUpdContact";
        private readonly string DeleteSPName = "USP_DeleteContact";
        private readonly string SelectSPName = "USP_GetContact";
        #endregion

        public ContactFactory(string connString, IConfiguration config)
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
            Contact obj1 = obj as Contact;
            base.parms.Add("@ContactID", obj1.RecordID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ContactID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
           Contact obj1 = obj as Contact;
            base.parms.Add("@ContactID", obj1.ContactID);
            base.parms.Add("@ContactTypeID", obj1.ContactTypeID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@FirstName", obj1.FirstName);
            base.parms.Add("@LastName", obj1.LastName);
            base.parms.Add("@Phone", obj1.Phone); 
            base.parms.Add("@Email", obj1.Email);
            base.parms.Add("@Address1", obj1.Address1);
            base.parms.Add("@Address2", obj1.Address2);
            base.parms.Add("@City", obj1.City);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@ZipCode", obj1.ZipCode);
            base.parms.Add("@IsDeleted", obj1.IsDeleted);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@ContactId", obj1.UserID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Contact contact = new Contact();
            // contact = reader.Read<Contact>().FirstOrDefault();
           
            return (T)Convert.ChangeType(contact, typeof(T));
        }
    }
}
