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
    public class DirectDepositFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdDirectDeposit";
        private readonly string UpdateSPName = "usp_InsUpdDirectDeposit";
        private readonly string DeleteSPName = "USP_DeleteDirectDeposit";
        private readonly string SelectSPName = "USP_GetDirectDeposit";
        #endregion

        public DirectDepositFactory(string connString, IConfiguration config)
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
            base.parms.Add("@DirectDepositId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@DirectDepositId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            User obj1 = obj as User;
            base.parms.Add("@DirectDepositId", obj1.UserID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            DirectDeposit obj1 = obj as DirectDeposit;

            base.parms.Add("@DirectDepositID", obj1.DirectDepositID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@BankName", obj1.BankName);
            base.parms.Add("@AccountTypeID", obj1.AccountTypeID);
            base.parms.Add("@RoutingNumber", obj1.RoutingNumber);
            base.parms.Add("@AccountNumber", obj1.AccountNumber);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@State", obj1.State);
            base.parms.Add("@Amount", obj1.Amount);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            DirectDeposit directDeposit = new DirectDeposit();
            directDeposit = reader.Read<DirectDeposit>().FirstOrDefault();
            return (T)Convert.ChangeType(directDeposit, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}