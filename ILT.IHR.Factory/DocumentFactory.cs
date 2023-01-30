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
    public class DocumentFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdDocument";
        private readonly string UpdateSPName = "usp_InsUpdDocument";
        private readonly string DeleteSPName = "USP_DeleteDocument";
        private readonly string SelectSPName = "USP_GetDocument";
        #endregion

        public DocumentFactory(string connString, IConfiguration config)
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
            base.parms.Add("@DocumentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@DocumentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@DocumentId", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Document obj1 = obj as Document;

            base.parms.Add("@DocumentID", obj1.DocumentID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@CompanyID", obj1.CompanyID);
            base.parms.Add("@DocumentCategoryID", obj1.DocumentCategoryID);
            base.parms.Add("@DocumentTypeID", obj1.DocumentTypeID);
            base.parms.Add("@IssuingAuthority", obj1.IssuingAuthority); 
            base.parms.Add("@DocumentNumber", obj1.DocumentNumber);
            base.parms.Add("@IssueDate", obj1.IssueDate);
            base.parms.Add("@ExpiryDate", obj1.ExpiryDate);
            base.parms.Add("@Note", obj1.Note);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Document Document = new Document();
            Document = reader.Read<Document>().FirstOrDefault();
            return (T)Convert.ChangeType(Document, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}