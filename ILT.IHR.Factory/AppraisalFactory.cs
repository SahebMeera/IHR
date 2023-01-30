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
    public class AppraisalFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAppraisal";
        private readonly string UpdateSPName = "usp_InsUpdAppraisal";
        private readonly string DeleteSPName = "usp_DeleteAppraisal";
        private readonly string SelectSPName = "USP_GetAppraisal";
        #endregion

        public AppraisalFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }


        public override Response<List<T>> GetList<T>(T obj)
        {
            Appraisal obj1 = obj as Appraisal;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.getStoredProc = SelectSPName;
           // base.parms = null;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@AppraisalId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AppraisalId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            Appraisal obj1 = obj as Appraisal;
            base.parms.Add("@AppraisalID", obj1.AppraisalID);
            base.getStoredProc = DeleteSPName;
            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Appraisal obj1 = obj as Appraisal;

            base.parms.Add("@AppraisalID", obj1.AppraisalID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@ReviewYear", obj1.ReviewYear);
            base.parms.Add("@ReviewerID", obj1.ReviewerID);
            base.parms.Add("@FinalReviewerID", obj1.FinalReviewerID);
            base.parms.Add("@AssignedDate", obj1.AssignedDate);
            base.parms.Add("@SubmitDate", obj1.SubmitDate);
            base.parms.Add("@ReviewDate", obj1.ReviewDate);
            base.parms.Add("@FinalReviewDate", obj1.FinalReviewDate);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@MgrFeedback", obj1.MgrFeedback);
            base.parms.Add("@Comment", obj1.Comment);
            base.parms.Add("@xmlAppraisalDetail", ConvertToXML(obj1.AppraisalDetails));
            base.parms.Add("@xmlAppraisalGoal", ConvertToXML(obj1.AppraisalGoals));
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Appraisal appraisal = new Appraisal();
            appraisal = reader.Read<Appraisal>().FirstOrDefault();
            appraisal.AppraisalDetails = reader.Read<AppraisalDetail>().ToList();
            appraisal.AppraisalGoals= reader.Read<AppraisalGoal>().ToList();
            return (T)Convert.ChangeType(appraisal, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
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