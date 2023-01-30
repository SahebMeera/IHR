using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using ILT.IHR.DTO;
using ILT.IHR.Factory;

namespace ITL.IHR.Factory
{
    public abstract class AbstractFactory
    {
        private bool isInTransaction = false;
        private ArrayList _auditTrailFields = new ArrayList();
        protected System.Data.SqlClient.SqlDataReader dr;
        protected System.Data.DataSet ds;
        protected QueryType queryType = QueryType.StoredProcedure;
        private SQLServer sqlServerConnObj;
        protected string getStoredProc;
        protected string sqlQuery;
        private string connectionString = string.Empty;
        protected TransactionManager transactionManager;
        protected AbstractDataObject _originalObject;
        protected bool IsInTransaction
        {get { return isInTransaction; }
         set { isInTransaction = value; }}

        public abstract List<T> GetList<T>(T obj);
        public abstract T GetByID<T>(T obj);
        public abstract T GetRelatedObjectsByID<T>(T obj);
        public abstract bool Save<T>(T obj);
        public abstract bool Delete<T>(T obj);
        protected abstract T LoadRelatedObjects<T>(DataSet DS);

        public AbstractFactory(string DBConnection)
        {
            this.connectionString = DBConnection;
            InitParameters();
            sqlServerConnObj = SQLServerConnObj;
        }

        private void InitParameters()
        {
            //determine if this object is operating in a transaction scope
            if (System.Transactions.Transaction.Current != null)
            {
                if (transactionManager == null)
                {
                    //initialize the shared instance if it has not been initialized
                    //and we are part of a transaction
                    transactionManager = TransactionManager.Instance;
                    this.sqlServerConnObj = TransactionManager.Instance.SQLServerToUse(System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier);
                }
                else
                {
                    //we are in a transaction and shared connection exists so use it
                    this.sqlServerConnObj = TransactionManager.Instance.SQLServerToUse(System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier);
                }

                //Register for the transaction completed event for the current transaction
                System.Transactions.Transaction.Current.TransactionCompleted += transactionManager.TransactionCompleted;
            }
        }

        protected List<T> GetList<T>()
        {
            List<T> MyResults = new List<T>();
            try
            {
                BuildDR();
                if ((dr != null))
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    MyResults = CollectionHelper.ConvertTo<T>(dt);
                   // CloseDR();
                }
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

        protected T GetByID<T>()
        {
            T MyResults = default(T);
            try
            {
                BuildDR();
                if ((dr != null))
                {
                    if (dr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(dr);
                        MyResults = CollectionHelper.CreateItem<T>(dt.Rows[0]);
                    }
                    // CloseDR();
                }
                return MyResults;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".GetInstance():" + exp.Message;
                throw new Exception(msg, exp);

                //ValidationErrorDTO error = new ValidationErrorDTO { Message = msg, Target = "" };
                //return new ValidationErrorsDTO(error);
            }
            finally
            {
                CloseDR();
            }
        }

        protected T GetRelatedObjectsByID<T>()
        {
            T MyResults = default(T);

            try
            {
                ds = this.SQLServerConnObj.ExecuteSPAndReturnDataSet(getStoredProc);
                if ((ds.Tables.Count > 0))
                {
                    MyResults = LoadRelatedObjects<T>(ds);
                }
                return MyResults;
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".GetDetailsByID():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
        }

        protected bool SaveInstanceReturnOutput<T>(T ProviderKey)
        {
            bool flag = false;
            //object result = null;
            Int32 ID = 0;
            try
            {
                if (ProviderKey != null)
                {


                    ArrayList arLi = this.SQLServerConnObj.ExecuteSPAndReturnOutput(this.getStoredProc);
                    if (Convert.ToInt32(arLi[arLi.Count - 1]) > 0 && (ProviderKey as AbstractDataObject).RecordID == 0)
                    {
                        flag = true;
                        ID = Convert.ToInt32(arLi[arLi.Count - 1]);
                    }
                }
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".SaveInstance():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
            return flag;
        }

       

        protected bool SaveInstance<T>(T ProviderKey)
        {
            bool flag = false;
            try
            {
                if (ProviderKey != null)
                {
                    flag = this.SQLServerConnObj.ExecuteSPNoReturn(this.getStoredProc);
                }
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".SaveInstance():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
            return flag;
        }

        protected bool DeleteInstance<T>(T ProviderKey)
        {
            bool flag = false;
            try
            {
                if (ProviderKey != null)
                    flag = this.SQLServerConnObj.ExecuteSPNoReturn(this.getStoredProc);
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".SaveInstance():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
            return flag;
        }

        protected int DeleteInstanceReturnOutput<T>(T ProviderKey)
        {
            Int32 ID = 0;
            try
            {
                if (ProviderKey != null)
                {
                    ArrayList arLi = this.SQLServerConnObj.ExecuteSPAndReturnOutput(this.getStoredProc);

                    if (Convert.ToInt32(arLi[arLi.Count - 1]) > 0 && (ProviderKey as AbstractDataObject).RecordID > 0)
                    {
                        ID = Convert.ToInt32(arLi[arLi.Count - 1]);
                    }
                }
            }
            catch (Exception exp)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".DeleteInstanceReturnOutput():" + exp.Message;
                throw new Exception(msg, exp);
            }
            finally
            {
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
                {
                    this.SQLServerConnObj.CloseConnection();
                    this.sqlServerConnObj = null;
                }
            }
            return ID;
        }


        protected void BuildDR()
        {
            try
            {
                //string tmpQueryOrSPName = string.Empty;
                if (this.queryType == QueryType.StoredProcedure)
                {
                    this.dr = this.sqlServerConnObj.ExecuteSPAndReturnDataReader(this.getStoredProc);
                    //tmpQueryOrSPName = this.getStoredProc;
                }
                else if (this.queryType == QueryType.Query)
                {
                    this.dr = this.sqlServerConnObj.ExecuteSQLAndReturnDataReader(this.sqlQuery);
                    //tmpQueryOrSPName = this.sqlQuery;
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

        protected void BuildDS()
        {
            try
            {
                string tmpQueryOrSPName = string.Empty;
                if (this.queryType == QueryType.StoredProcedure)
                {
                    this.ds = this.sqlServerConnObj.ExecuteSPAndReturnDataSet(this.getStoredProc);
                    tmpQueryOrSPName = this.getStoredProc;
                }
                else if (this.queryType == QueryType.Query)
                {
                    this.ds = this.sqlServerConnObj.ExecuteSPAndReturnDataSet(this.sqlQuery);
                    tmpQueryOrSPName = this.sqlQuery;
                }

            }
            catch (Exception ex)
            {
                string msg = null;
                msg = this.GetType().ToString() + ".GetRelatedObjects()";

                this.ds = null;
                if (System.Transactions.Transaction.Current == null && !this.IsInTransaction)
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

        protected SQLServer SQLServerConnObj
        {
            get
            {
                try
                {
                    if (sqlServerConnObj == null)
                    {
                        sqlServerConnObj = new SQLServer();
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

        public enum QueryType
        {
            StoredProcedure = 0,
            Query = 1
        }

    }

}
