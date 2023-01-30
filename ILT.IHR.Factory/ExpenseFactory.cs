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
    public class ExpenseFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdExpense";
        private readonly string UpdateSPName = "usp_InsUpdExpense";
        private readonly string DeleteSPName = "USP_DeleteExpense";
        private readonly string SelectSPName = "USP_GetExpense";
        #endregion

        public ExpenseFactory(string connString, IConfiguration config)
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
            base.parms.Add("@ExpenseId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@ExpenseId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Expense obj1 = obj as Expense;
            base.parms.Add("@ExpenseId", obj1.ExpenseID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            
            Expense obj1 = obj as Expense;
          

            base.parms.Add("@ExpenseID", obj1.ExpenseID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@ExpenseTypeID", obj1.ExpenseTypeID);
            base.parms.Add("@FileName", obj1.FileName);
            base.parms.Add("@Amount", obj1.Amount);
            base.parms.Add("@SubmissionDate", obj1.SubmissionDate);
            base.parms.Add("@SubmissionComment", obj1.SubmissionComment);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@AmountPaid", obj1.AmountPaid);
            base.parms.Add("@PaymentDate", obj1.PaymentDate);
            base.parms.Add("@PaymentComment", obj1.PaymentComment);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Expense Expense = new Expense();
            Expense = reader.Read<Expense>().FirstOrDefault();
            return (T)Convert.ChangeType(Expense, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}