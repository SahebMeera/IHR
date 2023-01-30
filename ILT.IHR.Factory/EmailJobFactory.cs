using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class EmailJobFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEmailJob";
        private readonly string UpdateSPName = "usp_InsUpdEmailJob";
        private readonly string DeleteSPName = "USP_DeleteEmailJob";
        private readonly string SelectSPName = "usp_GetPendingEmailJob";
        #endregion

        public EmailJobFactory(string connString, IConfiguration config)
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
            base.parms.Add("@EmailJobID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EmailJobID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            EmailJob obj1 = obj as EmailJob;
            base.parms.Add("@EmailJobId", obj1.RecordID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            EmailJob obj1 = obj as EmailJob;
            base.parms.Add("@EmailJobID", obj1.EmailJobID);
            base.parms.Add("@Subject", obj1.Subject);
            base.parms.Add("@From", obj1.From);
            base.parms.Add("@To", obj1.To);
            base.parms.Add("@CC", obj1.CC);
            base.parms.Add("@BCC", obj1.BCC);
            base.parms.Add("@Body", obj1.Body);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);

            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));

            base.getStoredProc = InsertSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            EmailJob EmailJob = new EmailJob();
            //EmailJob = reader.Read<EmailJob>().FirstOrDefault();
            //EmailJob. = reader.Read<DTO>().ToList();
            return (T)Convert.ChangeType(EmailJob, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}