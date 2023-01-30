using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Linq;
using ILT.IHR.DTO;

namespace ILT.IHR.Factory
{
    public abstract class AbstractFactory_orig
    {
        #region Members
        private List<AbstractDataObject> _listOfObject = new List<AbstractDataObject>();
        private bool isInTransaction = false;
        protected SqlDataReader dr;
        protected QueryType queryType = QueryType.StoredProcedure;

        public delegate AbstractDataObject BuildConcreteObject();
        protected BuildConcreteObject getConcreteObject;
        private SQLServer sqlServerConnObj;
        protected string sQLQuery;
        protected string getStoredProc;
        protected string saveStoredProc;
        protected string deleteStoredProc;
        protected string connectionString = string.Empty;
        protected string sqlQuery;
        protected TransactionManager transactionManager;
        protected AbstractDataObject _originalObject;

        public AbstractDataObject OriginalObject
        {
            get { return _originalObject; }
            set { _originalObject = value; }
        }

        protected enum expectedDBType
        {
            IsNumeric,
            IsByte,
            IsString,
            IsDate,
            IsBinary,
            IsByteArray,
            IsXml,
            IsChar,
            IsLong
        }
        #endregion

        #region Const Value
        protected string ConstSuccessMessage = "Record Saved Successfully!!";
        protected string ConstErrorMessage = "An error occured during the operation. Please contact Adminstrator.";
        protected string ConstConcurrencyMessage = "Record has been modified by other user. Please try again.";
        #endregion

        #region Constructors

        public AbstractFactory_orig()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //determine if this object is operating in a transaction scope
            if (System.Transactions.Transaction.Current != null)
            {
                if (transactionManager == null)
                {
                    //initialize the shared instance if it has not been initialized
                    //and we are part of a transaction
                    transactionManager = TransactionManager.Instance;
                    //this.sqlServerConnObj = DTO.TransactionManager.Instance.SQLServerToUse(System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier);
                }
                else
                {
                    //we are in a transaction and shared connection exists so use it
                    //this.sqlServerConnObj = DTO.TransactionManager.Instance.SQLServerToUse(System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier);
                }

                //Register for the transaction completed event for the current transaction
                System.Transactions.Transaction.Current.TransactionCompleted += transactionManager.TransactionCompleted;
            }
        }

        public AbstractFactory_orig(string DBConnection)
        {
            this.connectionString = DBConnection;
            
        }

        #endregion Constructors

        #region Abstract Methods
        public abstract AbstractDataObject GetConcreteObject();
        public abstract AbstractDataObject Insert(AbstractDataObject obj);
        public abstract AbstractDataObject Insert(List<AbstractDataObject> obj);
        public abstract AbstractDataObject Update(AbstractDataObject obj);
        public abstract AbstractDataObject Update(List<AbstractDataObject> obj);
        public abstract AbstractDataObject Delete(AbstractDataObject obj);
        public abstract AbstractDataObject Delete(List<AbstractDataObject> obj);
        public abstract List<T> SelectRelatedObjects<T>(long recordID=0) where T : AbstractDataObject;
        public abstract List<T> SelectRelatedObjects<T>() where T : AbstractDataObject;
        public abstract List<T> SelectRelatedObjects<T>(AbstractDataObject obj) where T : AbstractDataObject;
        #endregion

        public List<AbstractDataObject> ListOfObject
        {
            get { return _listOfObject; }
            set { _listOfObject = value; }
        }

        protected SQLServer SQLServerConnObj
        {
            get
            {
                try
                {
                    if (sqlServerConnObj == null)
                    {
                        
                        sqlServerConnObj = new SQLServer(connectionString);
                        if (!this.connectionString.Equals(string.Empty)) sqlServerConnObj.ConnectionString = this.connectionString;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid Database Connection", ex);
                }
                return sqlServerConnObj;
            }
            set { sqlServerConnObj = value; }
        }
        
        protected bool IsInTransaction
        {
            get { return isInTransaction; }
            set { isInTransaction = value; }
        }

        protected virtual AbstractDataObject GetInstance()
        {
            AbstractDataObject o = null;
            try
            {

                this.dr = this.SQLServerConnObj.ExecuteSPAndReturnDataReader(this.getStoredProc);
                if (this.dr.Read())
                {
                    o = this.GetConcreteObject();
                }
                return o;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    this.dr.Close();
                }
                catch
                {
                }
                this.dr = null;

                if (System.Transactions.Transaction.Current == null && !this.isInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        protected virtual AbstractDataObject Save(AbstractDataObject inAbstractCoreObject)
        {
            Int32 ID = 0;
            try
            {
                if (inAbstractCoreObject != null)
                {
                    this.SQLServerConnObj.ExecuteSPNoReturn(saveStoredProc);
                }
                return inAbstractCoreObject;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".Save(ByVal As Object):" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {

                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        protected virtual AbstractDataObject Save(List<AbstractDataObject> inAbstractCoreObject)
        {
            try
            {
                if (inAbstractCoreObject != null)
                {
                    this.SQLServerConnObj.ExecuteSPNoReturn(saveStoredProc);
                }
                
                return inAbstractCoreObject[0];
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ". Save(List<AbstractDataObject> inAbstractCoreObject):" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {

                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        /// <summary>
        /// Pass AbstractDataObject in list.
        /// </summary>
        /// <param name="inAbstractCoreObject"></param>
        /// <returns>It will return first object of passed list</returns>
        protected virtual ArrayList SaveAndReturnOutput(List<AbstractDataObject> inAbstractCoreObject)
        {
            try
            {
                if (inAbstractCoreObject != null)
                {
                    return this.SQLServerConnObj.ExecuteSPAndReturnOutput(saveStoredProc);
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ". SaveAndReturnOutput(List<AbstractDataObject> inAbstractCoreObject):" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        protected virtual AbstractDataObject SaveNewData(AbstractDataObject inAbstractCoreObject)
        {
            
            DataSet outDataSet = new DataSet();
            Int32 ID = 0;
            try
            {
                if (inAbstractCoreObject != null)
                {
                    outDataSet = this.SQLServerConnObj.ExecuteSPAndReturnDataSet(saveStoredProc);
                    if (outDataSet != null)
                    {
                        ID = Convert.ToInt32(outDataSet.Tables[0].Rows[0]["RecordID"]);
                    }
                    if (((inAbstractCoreObject as AbstractDataObject).RecordID != 0 || ID > 0))
                    {
                        (inAbstractCoreObject as AbstractDataObject).RecordID = ID;
                    }
                    
                }
                return inAbstractCoreObject;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".Save(ByVal As Object):" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {

                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
            
        }

        protected virtual ArrayList Delete()
        {
            try
            {
                return SQLServerConnObj.ExecuteSPAndReturnOutput(deleteStoredProc);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        //protected virtual void Delete(AbstractDataObject inObject)
        //{
        //    bool flag = false;
        //    AuditTrailFactory objAudit = new AuditTrailFactory();
        //    try
        //    {
        //        flag = SQLServerConnObj.ExecuteSPNoReturn(deleteStoredProc);

        //        if (flag)
        //        {
        //            // Fetching all the fields which needs to be audited and add these to DTO
        //            inObject = objAudit.GetListByTableName(inObject, CONST_OPERATION_DELETE);

        //            if ((inObject as AbstractDataObject).ID != 0)
        //                objAudit.LogAuditTrail(inObject, objAudit.GetDataChangesToAudit(inObject, _originalObject, CONST_OPERATION_DELETE));

        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
        //        {
        //            this.SQLServerConnObj.CloseConnection();
        //            this.SQLServerConnObj = null;
        //        }
        //    }
        //}

        protected virtual void Delete(System.Decimal id)
        {
            try
            {
                SQLServerConnObj.ExecuteSPNoReturn(deleteStoredProc);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        private string getDatabaseConnection(string inKey)
        {
            string DBConnection = string.Empty;
            try  
            {
                DBConnection = System.Configuration.ConfigurationManager.AppSettings[inKey];
            }
            catch (Exception ex)
            {
                throw new Exception("Error raised in AbstractFactory.getDatabaseConnection()::ERR:" + ex.Message + ":" + ex.StackTrace, ex);
            }
            return DBConnection;
        }

        protected List<T> GetRelatedObjects<T>() where T : AbstractDataObject
        {
            List<T> MyResults = new List<T>();
            try
            {
                BuildDR();
                if ((dr != null))
                {
                    T obj = default(T);
                    while (dr.Read())
                    {
                        obj = (T)this.GetConcreteObject();
                        MyResults.Add(obj);
                    }
                    CloseDR();
                }
                MyResults.TrimExcess();
                return MyResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseDR();
            }
        }

        protected virtual List<AbstractDataObject> GetRelatedObjects()
        {
            List<AbstractDataObject> MyResults = new List<AbstractDataObject>();
            try
            {
                BuildDR();
                if ((dr != null))
                {
                    AbstractDataObject obj = default(AbstractDataObject);
                    while (dr.Read())
                    {
                        obj = this.GetConcreteObject();
                        MyResults.Add(obj);
                    }
                    CloseDR();
                }
                MyResults.TrimExcess();
                return MyResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseDR();
            }
        }

        protected DataSet GetRelatedDataSet()
        {
            try
            {
                DataSet outDataSet = new DataSet();
                outDataSet = this.SQLServerConnObj.ExecuteSPAndReturnDataSet(getStoredProc);
                return outDataSet;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".GetRelatedDataSet():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {

                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        protected void BuildDR()
        {
            try
            {
                string tmpQueryOrSPName = string.Empty;
                if (this.queryType == QueryType.StoredProcedure)
                {
                    this.dr = this.sqlServerConnObj.ExecuteSPAndReturnDataReader(this.getStoredProc);
                    tmpQueryOrSPName = this.getStoredProc;
                }
                else if (this.queryType == QueryType.Query)
                {
                    this.dr = this.sqlServerConnObj.ExecuteSQLAndReturnDataReader(this.sqlQuery);
                    tmpQueryOrSPName = this.sqlQuery;
                }

            }
            catch (Exception ex)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".GetRelatedObjects()";
                try
                {
                    this.dr.Close();
                    this.queryType = QueryType.StoredProcedure;
                }
                catch
                {
                }
                this.dr = null;
                if (System.Transactions.Transaction.Current == null && !this.isInTransaction)
                {
                    this.sqlServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
                throw new Exception(msg, ex);
            }
        }

        protected void CloseDR()
        {
            try
            {
                this.dr.Close();
                this.queryType = QueryType.StoredProcedure;
                this.dr = null;

                if (System.Transactions.Transaction.Current == null && !this.isInTransaction)
                {
                    this.sqlServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            //Nothing to do
        }

        protected object GetDBValue(string FieldName)
        {
            if (DataRecordExtensions.HasColumn(dr, FieldName))
            {
                int i = dr.GetOrdinal(FieldName);
                return dr.GetValue(i);
            }
            else
                return null;
        }
        protected object GetDBValue(string FieldName, expectedDBType exType)
        {
            int i = 0;
            try
            {
                if (DataRecordExtensions.HasColumn(dr, FieldName))
                {
                    i = dr.GetOrdinal(FieldName);
                    if (!dr.IsDBNull(i))
                    {
                        try
                        {
                            if (exType == expectedDBType.IsNumeric | exType == expectedDBType.IsDate | exType == expectedDBType.IsByte | exType == expectedDBType.IsChar)
                            {
                                return dr.GetValue(i);
                            }
                            else if (exType == expectedDBType.IsBinary)
                            {
                                return dr.GetSqlBinary(i).Value;
                            }
                            else
                            {
                                return dr.GetValue(i).ToString().Trim();
                            }
                        }
                        catch (Exception)
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (exType == expectedDBType.IsLong)
                            {
                                return ConvertTo<Int64>(Convert.ToString(0));
                            }
                            else if (exType == expectedDBType.IsNumeric)
                            {
                                return ConvertTo<Int32>(Convert.ToString(0));
                            }
                            else if (exType == expectedDBType.IsByte)
                            {
                                return false;
                            }
                            else if (exType == expectedDBType.IsDate)
                            {
                                return ConvertTo<DateTime>(Convert.ToString(null));
                            }
                            else if (exType == expectedDBType.IsBinary)
                            {
                                return null;
                            }
                            else if (exType == expectedDBType.IsChar)
                            {
                                return ConvertTo<char>(Convert.ToString(""));
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        catch (Exception)
                        {

                        }
                        return string.Empty;
                    }
                }
                else
                {
                    if (exType == expectedDBType.IsNumeric)
                    {
                        return ConvertTo<Int32>(Convert.ToString(0));
                    }
                    else if (exType == expectedDBType.IsByte)
                    {
                        return ConvertTo<byte>(Convert.ToString(""));
                    }
                    else if (exType == expectedDBType.IsDate)
                    {
                        return ConvertTo<DateTime>(Convert.ToString(""));
                    }
                    else if (exType == expectedDBType.IsChar)
                    {
                        return ConvertTo<char>(Convert.ToString(""));
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                if (exType == expectedDBType.IsNumeric | exType == expectedDBType.IsByte)
                {
                    return 0;
                }
                else if (exType == expectedDBType.IsDate)
                {
                    return DateTime.MinValue;
                }
                else if (exType == expectedDBType.IsBinary)
                {
                    return null;
                }
                else if (exType == expectedDBType.IsChar)
                {
                    return null;
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public static Nullable<T> ConvertTo<T>(string strVal) where T : struct
        {
            try  //Task #17906: Check for if we are capturing all the exceptions at the app layer. Mayank Kukadia. 03/12/2012.
            {
                if (typeof(T) == typeof(Guid))
                {
                    return (string.IsNullOrEmpty(strVal) ? (Nullable<T>)null : (T)Convert.ChangeType(new Guid(strVal), typeof(T)));
                }
                if (typeof(T) == typeof(Boolean))
                {
                    return (string.IsNullOrEmpty(strVal) ? (Nullable<T>)null : (T)Convert.ChangeType(Boolean.Parse(strVal.ToLower() == "false" ? "false" : "true"), typeof(T)));
                }
                else
                {
                    return (string.IsNullOrEmpty(strVal) ? (Nullable<T>)null : (T)Convert.ChangeType(strVal, typeof(T)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error raised in AbstractFactory.ConvertTo<T>()::ERR:" + ex.Message + ":" + ex.StackTrace, ex);
            }
        }

        public string GetConfigurationSettingValue(string key)
        {
            string strValue = String.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings != null && System.Configuration.ConfigurationManager.AppSettings[key] != null)
            {
                strValue = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }
            return strValue;
        }

        /// <summary>
        /// Save operation if don't require to save complete object.
        /// </summary>
        protected void Save()
        {
            try
            {
                SQLServerConnObj.ExecuteSPNoReturn(saveStoredProc);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.SQLServerConnObj = null;
                }
            }
        }

        protected virtual void AddCommonParameter(AbstractDataObject obj)
        {
            if (obj.RecordID != 0)
            {
                SQLServerConnObj.AddParameter("@RecordID", obj.RecordID, SqlDbType.BigInt, 8, ParameterDirection.Input);
                SQLServerConnObj.AddParameter("@ModifiedBy", obj.ModifiedBy, SqlDbType.VarChar, 8, ParameterDirection.Input);
            }
            else
            {
                SQLServerConnObj.AddParameter("@CreatedBy", obj.CreatedBy, SqlDbType.VarChar, 8, ParameterDirection.Input);
            }
        }

        protected bool IsConcurrencyOccured(AbstractDataObject obj)
        {
            var lst = (
                         from ls in SelectRelatedObjects<AbstractDataObject>(obj.RecordID).ToList()
                         where ls.TimeStamp.SequenceEqual(obj.TimeStamp)
                         select ls
                         ).Count();
            if (lst == 0)
            {
                return false;
            }
            return true;
        }
    }

    public enum QueryType
    {
        StoredProcedure = 0,
        Query = 1
    }

    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
