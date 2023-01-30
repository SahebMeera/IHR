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
    public class FormI9Factory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdFormI9";
        private readonly string UpdateSPName = "usp_InsUpdFormI9";
        private readonly string DeleteSPName = "USP_DeleteFormI9";
        private readonly string SelectSPName = "USP_GetFormI9";
        private readonly string SelectI9ExpirySPName = "USP_GetI9Expiry";
        #endregion

        public FormI9Factory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            FormI9 obj1 = obj as FormI9;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@FormI9Id", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@FormI9Id", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            FormI9 obj1 = obj as FormI9;
            base.parms.Add("@FormI9Id", obj1.FormI9ID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            FormI9 obj1 = obj as FormI9;

            base.parms.Add("@FormI9ID", obj1.FormI9ID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@FirstName", obj1.FirstName);
            base.parms.Add("@MiddleName", obj1.MiddleName);
            base.parms.Add("@LastName", obj1.LastName);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@Address1", obj1.Address1);
            base.parms.Add("@Address2", obj1.Address2);
            base.parms.Add("@City", obj1.City);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@ZipCode", obj1.ZipCode);
            base.parms.Add("@BirthDate", obj1.BirthDate);
            base.parms.Add("@SSN", obj1.SSN);
            base.parms.Add("@Phone", obj1.Phone);
            base.parms.Add("@Email", obj1.Email);
            base.parms.Add("@WorkAuthorizationID", obj1.WorkAuthorizationID);
            base.parms.Add("@ANumber", obj1.ANumber);
            base.parms.Add("@USCISNumber", obj1.USCISNumber);
            base.parms.Add("@I94Number", obj1.I94Number);
            base.parms.Add("@I94ExpiryDate", obj1.I94ExpiryDate);
            base.parms.Add("@PassportNumber", obj1.PassportNumber);
            base.parms.Add("@PassportCountry", obj1.PassportCountry);
            base.parms.Add("@HireDate", obj1.HireDate);
            base.parms.Add("@ListADocumentTitleID", obj1.ListADocumentTitleID);
            base.parms.Add("@ListAIssuingAuthority", obj1.ListAIssuingAuthority);
            base.parms.Add("@ListADocumentNumber", obj1.ListADocumentNumber);
            base.parms.Add("@ListAStartDate", obj1.ListAStartDate);
            base.parms.Add("@ListAExpirationDate", obj1.ListAExpirationDate);
            base.parms.Add("@ListBDocumentTitleID", obj1.ListBDocumentTitleID);
            base.parms.Add("@ListBIssuingAuthority", obj1.ListBIssuingAuthority);
            base.parms.Add("@ListBDocumentNumber", obj1.ListBDocumentNumber);
            base.parms.Add("@ListBStartDate", obj1.ListBStartDate);
            base.parms.Add("@ListBExpirationDate", obj1.ListBExpirationDate);
            base.parms.Add("@ListCDocumentTitleID", obj1.ListCDocumentTitleID);
            base.parms.Add("@ListCIssuingAuthority", obj1.ListCIssuingAuthority);
            base.parms.Add("@ListCDocumentNumber", obj1.ListCDocumentNumber);
            base.parms.Add("@ListCStartDate", obj1.ListCStartDate);
            base.parms.Add("@ListCExpirationDate", obj1.ListCExpirationDate);
            base.parms.Add("@IsDeleted", obj1.IsDeleted);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);            
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);            
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            FormI9 formI9 = new FormI9();
            formI9 = reader.Read<FormI9>().FirstOrDefault();
            return (T)Convert.ChangeType(formI9, typeof(T));
        }
        public Response<List<FormI9>> GetI9ExpiryForm<T>(T obj)
        {
            FormI9 obj1 = obj as FormI9;
            base.parms.Add("@I94ExpiryDate", obj1.I94ExpiryDate);
            base.getStoredProc = SelectI9ExpirySPName;
            return base.GetList<FormI9>();
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}