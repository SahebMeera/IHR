using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class CompanyFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdCompany";
        private readonly string UpdateSPName = "usp_InsUpdCompany";
        private readonly string DeleteSPName = "USP_DeleteCompany";
        private readonly string SelectSPName = "USP_GetCompany";
        #endregion

        public CompanyFactory(string connString, IConfiguration config)
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
            base.parms.Add("@CompanyID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@CompanyID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Company obj1 = obj as Company;
            base.parms.Add("@CompanyId", obj1.RecordID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Company obj1 = obj as Company;
            base.parms.Add("@CompanyID", obj1.CompanyID);
            base.parms.Add("@Name", obj1.Name);
            base.parms.Add("@Address1", obj1.Address1);
            base.parms.Add("@Address2", obj1.Address2);
            base.parms.Add("@City", obj1.City);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@ZipCode", obj1.ZipCode);
            base.parms.Add("@ContactName", obj1.ContactName);
            base.parms.Add("@ContactPhone", obj1.ContactPhone);
            base.parms.Add("@ContactEmail", obj1.ContactEmail);
            base.parms.Add("@AlternateContactName", obj1.AlternateContactName);
            base.parms.Add("@AlternateContactPhone", obj1.AlternateContactPhone);
            base.parms.Add("@AlternateContactEmail", obj1.AlternateContactEmail);
            base.parms.Add("@InvoiceContactName", obj1.InvoiceContactName);
            base.parms.Add("@InvoiceContactPhone", obj1.InvoiceContactPhone);
            base.parms.Add("@InvoiceContactEmail", obj1.InvoiceContactEmail);
            base.parms.Add("@AlternateInvoiceContactName", obj1.AlternateInvoiceContactName);
            base.parms.Add("@AlternateInvoiceContactPhone", obj1.AlternateInvoiceContactPhone);
            base.parms.Add("@AlternateInvoiceContactEmail", obj1.AlternateInvoiceContactEmail);
            base.parms.Add("@InvoicingPeriodID", obj1.InvoicingPeriodID);
            base.parms.Add("@PaymentTermID", obj1.PaymentTermID);
            base.parms.Add("@TaxID", obj1.TaxID);
            base.parms.Add("@CompanyTypeID", obj1.CompanyTypeID);
            base.parms.Add("@IsEndClient", obj1.IsEndClient);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));

            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Company company = new Company();
            //company = reader.Read<Company>().FirstOrDefault();
            //company. = reader.Read<DTO>().ToList();
            return (T)Convert.ChangeType(company, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}