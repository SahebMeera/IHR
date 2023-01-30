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
    public class AssignmentFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdAssignment";
        private readonly string UpdateSPName = "usp_InsUpdAssignment";
        private readonly string DeleteSPName = "usp_DeleteAssignment";
        private readonly string SelectSPName = "USP_GetAssignment";
        #endregion

        public AssignmentFactory(string connString, IConfiguration config)
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
            base.parms.Add("@AssignmentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@AssignmentId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;
            base.parms.Add("@AssignmentID", obj1.AssignmentID);
            base.getStoredProc = DeleteSPName;
            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Assignment obj1 = obj as Assignment;

            base.parms.Add("@AssignmentID", obj1.AssignmentID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@VendorID", obj1.VendorID);
            base.parms.Add("@ClientManager", obj1.ClientManager);
            base.parms.Add("@Title", obj1.Title);
            //base.parms.Add("@Role", obj1.Role);
            base.parms.Add("@Address1", obj1.Address1);
            base.parms.Add("@Address2", obj1.Address2);
            base.parms.Add("@City", obj1.City);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@ZipCode", obj1.ZipCode);
            base.parms.Add("@ClientID", obj1.ClientID);
            base.parms.Add("@EndClientID", obj1.EndClientID);
            base.parms.Add("@SubClient", obj1.SubClient);
            base.parms.Add("@Comments", obj1.Comments);
            base.parms.Add("@PaymentTypeID", obj1.PaymentTypeID);
            base.parms.Add("@TimeSheetTypeID", obj1.TimeSheetTypeID);
            //base.parms.Add("@TimesheetApproverID", obj1.TimesheetApproverID);
            base.parms.Add("@TSApproverEmail", obj1.TSApproverEmail);
            base.parms.Add("@ApprovedEmailTo", obj1.ApprovedEmailTo); 
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Assignment assignment = new Assignment();
            assignment = reader.Read<Assignment>().FirstOrDefault();
            assignment.AssignmentRates = reader.Read<AssignmentRate>().ToList();
            return (T)Convert.ChangeType(assignment, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}