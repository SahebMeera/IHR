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
    public class EmailApprovalFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEmailApproval";
        private readonly string UpdateSPName = "usp_InsUpdEmailApproval";
        private readonly string DeleteSPName = "USP_DeleteEmailApproval";
        private readonly string SelectSPName = "USP_GetEmailApproval";
        private readonly string ActionSPName = "usp_EmailApprovalAction";
        private readonly string EmailActionSPName = "usp_EmailSendAction";
        #endregion

        public EmailApprovalFactory(string connString, IConfiguration config)
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
            EmailApproval obj1 = obj as EmailApproval;
            base.parms.Add("@LinkID", obj1.LinkID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EmailApprovalId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@EmailApprovalId", obj1.AssignmentID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            EmailApproval obj1 = obj as EmailApproval;
            base.parms.Add("@EmailApprovalID", obj1.EmailApprovalID);
            base.parms.Add("@ModuleID", obj1.ModuleID);
            base.parms.Add("@ID", obj1.ID);
            //base.parms.Add("@UserID", obj1.UserID);
            base.parms.Add("@ValidTime", obj1.ValidTime);
            base.parms.Add("@IsActive", obj1.IsActive);
            base.parms.Add("@Value", obj1.Value);
            base.parms.Add("@LinkID", obj1.LinkID);
            base.parms.Add("@ApproverEmail", obj1.ApproverEmail);
            base.parms.Add("@EmailSubject", obj1.EmailSubject);
            base.parms.Add("@EmailBody", obj1.EmailBody);
            base.parms.Add("@EmailFrom", obj1.EmailFrom);
            base.parms.Add("@EmailTo", obj1.EmailTo);
            base.parms.Add("@EmailCC", obj1.EmailCC);
            base.parms.Add("@EmailBCC", obj1.EmailBCC);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            EmailApproval EmailApproval = new EmailApproval();
            EmailApproval = reader.Read<EmailApproval>().FirstOrDefault();
            return (T)Convert.ChangeType(EmailApproval, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }


        public  Response<T> EmailApprovalAction<T>(T obj)
        {
            EmailApproval obj1 = obj as EmailApproval;
            base.parms.Add("@LinkID", obj1.LinkID);
            base.parms.Add("@Value", obj1.Value);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = ActionSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }


        public Response<T> OnEmailSent<T>(T obj)
        {
            EmailApproval obj1 = obj as EmailApproval;
            base.parms.Add("@EmailApprovalID", obj1.EmailApprovalID);
            base.parms.Add("@IsActive", obj1.IsActive);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = EmailActionSPName; ;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

    }
}