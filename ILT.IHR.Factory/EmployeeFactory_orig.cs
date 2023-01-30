using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using ILT.IHR.DTO;

namespace ILT.IHR.Factory
{
    public class EmployeeFactory_orig : AbstractFactory_orig
    {
        #region Member
        private readonly string InsertSPName = "usp_InsUpdEmployee";
        private readonly string UpdateSPName = "usp_InsUpdEmployee";
        private readonly string DeleteSPName = "USP_DeleteEmployee";
        private readonly string SelectSPName = "USP_SelectEmployee";

        #endregion

        #region Constructor
        /// <summary>
        /// Default Contructor
        /// </summary>
        public EmployeeFactory_orig()
            : base()
        {
        }

        /// <summary>
        /// Constructor with connection string as an argument. Calls the base class constructor.
        /// </summary>
        /// <param name="connString"></param>
        public EmployeeFactory_orig(string connString)
            : base(connString)
        {
        }
        #endregion

        #region Overriden Method
        /// <summary>
        /// Get Concreate Object
        /// </summary>
        /// <returns></returns>
        public override AbstractDataObject GetConcreteObject()
        {
            Employee obj = new Employee();
            try
            {
                foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                {
                    object dbValue = this.GetDBValue(propertyInfo.Name);
                    if (dbValue != null && !string.IsNullOrEmpty(dbValue.ToString()))
                    {
                        propertyInfo.SetValue(obj, Convert.ChangeType(dbValue, propertyInfo.PropertyType));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        public override AbstractDataObject Insert(List<AbstractDataObject> obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert Record
        /// </summary>
        /// <param name="obj"></param>
        public override AbstractDataObject Insert(AbstractDataObject obj)
        {
            try
            {
                base.SQLServerConnObj.ClearParameters(); // Clear Parameters
                AddParameter(obj as Employee);
                base.saveStoredProc = InsertSPName;
                obj = base.Save(obj);
                obj.Message = ConstSuccessMessage;
                obj.MessageType = MessageType.Success;
                return obj;
            }
            catch (Exception ex)
            {
                throw ex; //Store the Error
            }
            //obj.Message = ConstDangerMessage;
            //obj.MessageType = MessageType.Danger;
            //return obj;
        }

        /// <summary>
        /// Update Record
        /// </summary>
        /// <param name="obj"></param>
        public override AbstractDataObject Update(AbstractDataObject obj)
        {
            try
            {
                if (IsConcurrencyOccured(obj))
                {
                    base.SQLServerConnObj.ClearParameters(); // Clear Parameters
                    AddParameter(obj as Employee);
                    base.saveStoredProc = UpdateSPName;
                    obj = base.Save(obj);
                    obj.Message = ConstSuccessMessage;
                    obj.MessageType = MessageType.Success;
                    return obj;
                }
                else
                {
                    obj.Message = ConstConcurrencyMessage;
                    obj.MessageType = MessageType.Warning;
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //obj.Message = ConstDangerMessage;
            //obj.MessageType = MessageType.Danger;
            //return obj;
        }
        public override AbstractDataObject Update(List<AbstractDataObject> obj)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Delete Record
        /// </summary>
        /// <param name="obj"></param>
        public override AbstractDataObject Delete(AbstractDataObject obj)
        {
            try
            {
                base.SQLServerConnObj.ClearParameters(); // Clear Parameters
                AddCommonParameter(obj);
                base.saveStoredProc = DeleteSPName;
                obj = base.Save(obj);
                obj.Message = ConstSuccessMessage;
                obj.MessageType = MessageType.Success;
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //obj.Message = ConstDangerMessage;
            //obj.MessageType = MessageType.Danger;
            //return obj;
        }
        public override AbstractDataObject Delete(List<AbstractDataObject> obj)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Select data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override List<Employee> SelectRelatedObjects<Employee>(long recordID)
        {
            List<AbstractDataObject> lstAbstractObj = new List<AbstractDataObject>();
            List<Employee> lstReturnObj = new List<Employee>();
            try
            {
                base.SQLServerConnObj.ClearParameters();
                base.SQLServerConnObj.AddParameter("@RecordID", recordID, SqlDbType.VarChar, 100, ParameterDirection.Input);

                base.getStoredProc = SelectSPName;
                lstAbstractObj = base.GetRelatedObjects();
                for (int i = 0; i < lstAbstractObj.Count; i++)
                {
                    lstReturnObj.Add((Employee)lstAbstractObj[i]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReturnObj;
        }

        public override List<Employee> SelectRelatedObjects<Employee>()
        {
            List<AbstractDataObject> lstAbstractObj = new List<AbstractDataObject>();
            List<Employee> lstReturnObj = new List<Employee>();
            try
            {
                base.SQLServerConnObj.ClearParameters();
                base.getStoredProc = SelectSPName;
                lstAbstractObj = base.GetRelatedObjects();
                for (int i = 0; i < lstAbstractObj.Count; i++)
                {
                    lstReturnObj.Add((Employee)lstAbstractObj[i]);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error raised in ComplianceFactory.GetRelatedObjects()::ERR:" + ex.Message + ":" + ex.StackTrace, ex);
                throw ex;
            }
            return lstReturnObj;
        }

        public override List<Employee> SelectRelatedObjects<Employee>(AbstractDataObject obj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void AddParameter(Employee obj)
        {
            try
            {
                AddCommonParameter(obj);
                //base.SQLServerConnObj.AddParameter("@RecordID", obj.RecordID, SqlDbType.VarChar, 100, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@FirstName", obj.FirstName, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@EmployeeCode", obj.EmployeeCode, SqlDbType.VarChar, 10, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@Address1", obj.Address1, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@Address2", obj.Address2, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@City", obj.City, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@State", obj.State, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@Country", obj.Country, SqlDbType.VarChar, 50, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@ZipCode", obj.ZipCode, SqlDbType.VarChar, 10, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@HireDate", obj.HireDate, SqlDbType.Date,8, ParameterDirection.Input);
                base.SQLServerConnObj.AddParameter("@WorkAuthorization", obj.WorkAuthorization, SqlDbType.VarChar, 50, ParameterDirection.Input);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
         
    }
}
