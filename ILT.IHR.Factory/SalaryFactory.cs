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
    public class SalaryFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdSalary";
        private readonly string UpdateSPName = "usp_InsUpdSalary";
        private readonly string DeleteSPName = "usp_DeleteSalary";
        private readonly string SelectSPName = "USP_GetSalary";
        #endregion

        public SalaryFactory(string connString, IConfiguration config)
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
            base.parms.Add("@SalaryId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@SalaryId", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Salary obj1 = obj as Salary;
            base.parms.Add("@SalaryID", obj1.SalaryID);

            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Salary obj1 = obj as Salary;

            base.parms.Add("@SalaryID", obj1.SalaryID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@BasicPay", obj1.BasicPay);
            base.parms.Add("@HRA", obj1.HRA);
            base.parms.Add("@LTA", obj1.LTA);
            base.parms.Add("@Bonus", obj1.Bonus);
            base.parms.Add("@EducationAllowance", obj1.EducationAllowance);
            base.parms.Add("@VariablePay", obj1.VariablePay);
            base.parms.Add("@TelephoneAllowance", obj1.TelephoneAllowance);
            base.parms.Add("@MedicalAllowance", obj1.MedicalAllowance);
            base.parms.Add("@MedicalInsurance", obj1.MedicalInsurance);
            base.parms.Add("@MealAllowance", obj1.MealAllowance);
            base.parms.Add("@Conveyance", obj1.Conveyance);
            base.parms.Add("@Gratuity", obj1.Gratuity);
            base.parms.Add("@SpecialAllowance", obj1.SpecialAllowance);
            base.parms.Add("@ProvidentFund", obj1.ProvidentFund);
            base.parms.Add("@CostToCompany", obj1.CostToCompany);
            base.parms.Add("@StartDate", obj1.StartDate);
            base.parms.Add("@EndDate", obj1.EndDate);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Salary Salary = new Salary();
            Salary = reader.Read<Salary>().FirstOrDefault();
            return (T)Convert.ChangeType(Salary, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }

    }
}