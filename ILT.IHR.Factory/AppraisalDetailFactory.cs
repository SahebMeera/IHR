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
    public class AppraisalDetailFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAppraisalDetail";
        private readonly string UpdateSPName = "usp_InsUpdAppraisalDetail";
        private readonly string DeleteSPName = "usp_DeleteAppraisalDetail";
        private readonly string SelectSPName = "USP_GetAppraisalDetail";
        #endregion

        public AppraisalDetailFactory(string connString, IConfiguration config)
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
            base.parms.Add("@AppraisalDetailId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AppraisalDetailId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            AppraisalDetail obj1 = obj as AppraisalDetail;
            base.parms.Add("@AppraisalDetailID", obj1.AppraisalDetailID);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            AppraisalDetail obj1 = obj as AppraisalDetail;

            base.parms.Add("@AppraisalDetailID", obj1.AppraisalDetailID);
            base.parms.Add("@AppraisalID", obj1.AppraisalID);
            base.parms.Add("@AppraisalQualityID", obj1.AppraisalQualityID);
            base.parms.Add("@EmpResponse", obj1.EmpResponse);
            base.parms.Add("@EmpComment", obj1.EmpComment);
            base.parms.Add("@MgrResponse", obj1.MgrResponse);
            base.parms.Add("@MgrComment", obj1.MgrComment);
            //base.parms.Add("@CreatedBy", obj1.CreatedBy);
            //base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            AppraisalDetail assignmentRate = new AppraisalDetail();
            assignmentRate = reader.Read<AppraisalDetail>().FirstOrDefault();
            return (T)Convert.ChangeType(assignmentRate, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}