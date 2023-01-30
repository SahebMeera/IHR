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
    public class EmployeeFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEmployee";
        private readonly string UpdateSPName = "usp_InsUpdEmployee";
        private readonly string DeleteSPName = "USP_DeleteEmployee";
        private readonly string SelectSPName = "USP_GetEmployee";
        private readonly string SelectEmployeeSPName = "USP_GetEmployeeInfo";
        #endregion

        public EmployeeFactory(string connString, IConfiguration config)
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
            base.parms.Add("@EmployeeID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EmployeeID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            Employee obj1 = new Employee();
            obj1 = obj as Employee;
            base.parms.Add("@EmployeeId", obj1.EmployeeID);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.selectStoredProc = SelectSPName;
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            Employee obj1 = obj as Employee;
            //this.Delete<Employee>(obj1);

            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@EmployeeCode", obj1.EmployeeCode);
            base.parms.Add("@FirstName", obj1.FirstName);
            base.parms.Add("@MiddleName", obj1.MiddleName);
            base.parms.Add("@LastName", obj1.LastName);
            base.parms.Add("@Country", obj1.Country);
            base.parms.Add("@@xmlEmployeeAddress", ConvertToXML(obj1.EmployeeAddresses));
            base.parms.Add("@TitleID", obj1.TitleID);
            base.parms.Add("@GenderID", obj1.GenderID);
            base.parms.Add("@DepartmentID", obj1.DepartmentID);
            base.parms.Add("@Phone", obj1.Phone);
            base.parms.Add("@HomePhone", obj1.HomePhone);
            base.parms.Add("@WorkPhone", obj1.WorkPhone);
            base.parms.Add("@Email", obj1.Email);
            base.parms.Add("@WorkEmail", obj1.WorkEmail);
            base.parms.Add("@BirthDate", obj1.BirthDate);
            base.parms.Add("@HireDate", obj1.HireDate);
            base.parms.Add("@TermDate", obj1.TermDate);
            base.parms.Add("@WorkAuthorizationID", obj1.WorkAuthorizationID);         
            base.parms.Add("@SSN", obj1.SSN);         
            base.parms.Add("@PAN", obj1.PAN);         
            base.parms.Add("@AadharNumber", obj1.AadharNumber);         
            base.parms.Add("@Salary", obj1.Salary);           
            base.parms.Add("@VariablePay", obj1.VariablePay);           
            base.parms.Add("@MaritalStatusID", obj1.MaritalStatusID);           
            base.parms.Add("@ManagerID", obj1.ManagerID);
            base.parms.Add("@EmploymentTypeID", obj1.EmploymentTypeID);
            base.parms.Add("@IsDeleted", obj1.IsDeleted);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

       

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Employee employee   = new Employee();
            employee = reader.Read<Employee>().FirstOrDefault();
            if(employee != null)
            {
                employee.Dependents = reader.Read<Dependent>().ToList();
                employee.DirectDeposits = reader.Read<DirectDeposit>().ToList();
                employee.Assignments = reader.Read<Assignment>().ToList();
                //reader.Read<AssignmentRate>().ToList();
                employee.Contacts = reader.Read<Contact>().ToList();
                employee.EmployeeAddresses = reader.Read<EmployeeAddress>().ToList();
                employee.Salaries = reader.Read<Salary>().ToList();
            }
            return (T)Convert.ChangeType(employee, typeof(T));
        }
        public Response<List<T>> GetEmployeeInfo<T>(T obj)
        {
            base.getStoredProc = SelectEmployeeSPName;
            return base.GetList<T>();
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