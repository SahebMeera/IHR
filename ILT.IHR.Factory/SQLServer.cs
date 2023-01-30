using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Collections;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;

namespace ILT.IHR.Factory
{
    /// <summary>
    /// SQLServer class is an utility class to handle all the connectivities to the SQLServer database.
    /// This is derived from Component class to provide the base implementation for IComponent which enables object sharing between Applications
    /// </summary>
    public sealed class SQLServer : Component
    {
        #region "Private Variables and Objects"
        SqlDatabase _sqlServerDb;
        private SqlCommand command;
        private System.Data.SqlClient.SqlDataReader dataReader;
        private XmlReader xmlReader;
        private DataSet dataSet;

        //We are going to handle parameters internally and this array is going to hold them
        private ArrayList paramList = new ArrayList();

        //Connection string variables are defined
        private string connectionString;

        //Naming, Cleanup and Exception variables are defined
        private string moduleName;
        private bool isDisposed;

        //Log errors by default
        private const string excpMessage = "Data Application Error. Detail Error Information can be found in the Application Log";
        private System.Data.SqlClient.SqlTransaction transaction;
        //Indicates the status of the current transaction
        private MyTransactionStatus transactionStatus = MyTransactionStatus.NotInTransaction;

        //if timeout is integer.MinValue, then use SQLCommand.Timeout default value
        //otherwise, use the new value
        private int timeout = int.MinValue;
        #endregion

        public enum MyTransactionStatus
        {
            BeginTransaction,
            InTransaction,
            CommitTransaction,
            RollbackTransaction,
            NotInTransaction
        };

        /// <summary>
        /// Default Contructor
        /// </summary>
        public SQLServer()
        {
            //InitializeComponent();
        }
        /// <summary>
        /// Contructor with connectionString as parameter. Also invokes the base class constructor
        /// </summary>
        /// <param name="connString"></param>
        public SQLServer(string connString) : base()
        {
            connectionString = connString;
            moduleName = this.GetType().ToString();
            //InitializeComponent();
        }
        public SQLServer(IContainer container)
        {
            container.Add(this);
            //InitializeComponent();
        }
        /// <summary>
        /// ConnectionString property
        /// </summary>
        public string ConnectionString
        {
            get
            {
                try
                {
                    return _sqlServerDb.ConnectionString;
                }
                catch
                {
                    return "";
                }
            }
            set { connectionString = value; }
        }

        /// <summary>
        /// MyTransactionStatus Property.
        /// </summary>
        public MyTransactionStatus TransactionStatus
        {
            get { return this.transactionStatus; }
        }
        public int TimeOut
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// This method is used to close the connection.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                _sqlServerDb = null;
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// This method is clear all the parameters in paramsList
        /// </summary>
        public void ClearParameters()
        {
            try
            {
                paramList.Clear();
            }
            catch (Exception exp)
            {
                throw new Exception(excpMessage + " Parameter List did not clear", exp);
            }
        }
        /// <summary>
        /// This method is used to process the transaction based on the TransactionStatus type value being passed to the method
        /// </summary>
        /// <param name="inTransactionStatus"></param>
        public void ProcessTransaction(MyTransactionStatus inTransactionStatus)
        {
            if (inTransactionStatus == MyTransactionStatus.NotInTransaction)
            {
                throw new Exception("Do not call this method unless you want to participate in transactions");
            }
            else
            {
                try
                {
                    switch (inTransactionStatus)
                    {
                        case MyTransactionStatus.BeginTransaction:
                            this.transactionStatus = MyTransactionStatus.BeginTransaction;
                            break;
                        case MyTransactionStatus.CommitTransaction:
                            if (this.transaction == null)
                            {
                                throw new Exception("Nothing to commit in transaction.");
                            }

                            this.transaction.Commit();
                            this.transactionStatus = MyTransactionStatus.NotInTransaction;
                            CloseConnection();
                            break;
                        case MyTransactionStatus.RollbackTransaction:
                            this.transaction.Rollback();
                            this.transactionStatus = MyTransactionStatus.NotInTransaction;
                            CloseConnection();
                            break;
                    }
                }
                catch (Exception exp)
                {
                    //Cleanup
                    CloseConnection();
                    this.transactionStatus = MyTransactionStatus.NotInTransaction;
                    //do not throw exceptions back if it is rollback
                    if (inTransactionStatus != MyTransactionStatus.RollbackTransaction)
                    {
                        throw exp;
                    }
                }
            }
        }
        /// <summary>
        /// This method is to verify the SQL statement to be valid. To avoid anonimous sql injections. 
        /// </summary>
        /// <param name="SQLStatement"></param>
        private void ValidateSQLStatement(ref string SQLStatement)
        {
            //SQL Statement must be at least 10 characters ( "Select * form x" )
            if (SQLStatement.Trim().Length < 10)
            {
                throw new Exception(excpMessage + " The SQL Statement must be provided and at least 10 characters long");
            }
        }
        /// <summary>
        /// This method is to verify the stored procedure name atleast 2 characters to avoid sql injections
        /// </summary>
        /// <param name="SQLStatement"></param>
        private void ValidateSPStatement(ref string SQLStatement)
        {
            //SQL Statement must be at least 2 characters for stored proc
            if (SQLStatement.Trim().Length < 2)
            {
                throw new Exception(excpMessage + " The Stored Procedure must be provided and at least 2 characters long");
            }
        }
        /// <summary>
        /// This method is used to set the time out for the command object. The value will be retrieved from the Config file
        /// </summary>
        private void SetCommandTimeout()
        {
            try
            {
                //check timeout property (the caller would have set it)
                if (timeout != int.MinValue)
                {
                    command.CommandTimeout = timeout;
                }
                else
                {
                    //Get the default timeout from the Machine.Config. if it says "Default" or if it is not present, then leave it alone. Otherwise set it to that
                    string MyCommandTimeOut = ConfigurationManager.AppSettings["Sheridan.InspireQuality.Utilities.DBAccess.CommandTimeout"];
                    if ((MyCommandTimeOut != null) && !MyCommandTimeOut.ToLower().Equals("default"))
                    {
                        command.CommandTimeout = int.Parse(MyCommandTimeOut);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        /// <summary>
        /// This method executes the SQL and returns the DataSet
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public DataSet ExecuteSQLAndReturnDataSet(string SQL)
        {

            //Validate the SQL String to be larger than 10 characters
            ValidateSQLStatement(ref SQL);

            //We include all called object in the try block to catch any excepitons that could occur ( even in the creation of the connection)
            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it.");
                }

                //Set a new Connecton
                //if (connection == null)
                //{
                //    connection = new SqlConnection(connectionString);
                //}
                if (_sqlServerDb == null)
                {
                    _sqlServerDb = new SqlDatabase(connectionString);
                }
                //Set a new Command that accepts an SQL statement and the connection. 
                //The command.commandtype does not have to be set since it defaults to text
                // command = new SqlCommand(SQL, connection);
                command = new SqlCommand(SQL);

                SetCommandTimeout();
                //Set a new DataSet
                dataSet = new DataSet();
                ////Set a new DataAdapter that will run the SQL statement
                //dataAdapter = new SqlDataAdapter(command);

                //dataAdapter.Fill(dataSet);
                dataSet = _sqlServerDb.ExecuteDataSet(command);

                return dataSet;
            }
            catch (Exception ExceptionObject)
            {
                //The exception is passed back to the calling code, with our custom message and specific exception information
                throw new Exception(excpMessage, ExceptionObject);
            }
            finally
            {
                //Immediately Close the Connection after use to free up resources
                if (this.transactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    CloseConnection();
                }

            }
        }
        /// <summary>
        /// This method executes the SQL statement and returns the DataReader
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteSQLAndReturnDataReader(string SQL)
        {

            //Validate the SQL String to be larger than 10 characters
            ValidateSQLStatement(ref SQL);

            //We include all called object in the try block to catch any excepitons that could occur ( even in the creation of the connection)
            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it.");
                }

                //Set a new Connecton
                //////if (connection == null)
                //////{
                //////    connection = new SqlConnection(connectionString);
                //////}

                if (_sqlServerDb == null)
                {
                    _sqlServerDb = new SqlDatabase(connectionString);
                }
                //Set a new Command that accepts an SQL statement and the connection. 
                //The command.commandtype does not have to be set since it defaults to text
                ////command = new SqlCommand(SQL, connection);
                command = new SqlCommand(SQL);

                SetCommandTimeout();
                //We need to open the connection for the DataReader explicitly
                //if (!connection.State.Equals(ConnectionState.Open))
                //{
                //    connection.Open();
                //}
                //Run the Execute Reader method of the Command Object

                ////dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                //Start. Added to implement Enterprise Library 5.0 Mayank Kukadia. 03/14/2011.
                // Enterprise Library 5.0 returns RefCounterDataReder instead of SqlDataReader and it must be closed. 
                command.Connection = _sqlServerDb.CreateConnection() as SqlConnection;
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                //dataReader = (SqlDataReader)_sqlServerDb.ExecuteReader(command);
                //End. Added to implement Enterprise Library 5.0 Mayank Kukadia. 03/14/2011.


                return dataReader;
            }
            catch (Exception ExceptionObject)
            {
                //If an exception occurs, close the connection now!
                if (this.transactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    CloseConnection();
                }
                //The exception is passed back to the calling code, with our custom message and specific exception information
                throw new Exception(excpMessage, ExceptionObject);

            }
        }
        /// <summary>
        /// This method executes the stored procedure and returns the Dataset
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public DataSet ExecuteSPAndReturnDataSet(string SPName)
        {
            //Validate Stored Procedure
            ValidateSPStatement(ref SPName);

            //Setting the objects to handle parameters
            Parameter privateUsedParameter = default(Parameter);
            //will return the specific parameter in the paramList
            SqlParameter privateParameter = default(SqlParameter);
            //will contain the converted SQLParameter
            //The usedEnumerator makes it easy to step through the list of parameters in the paramList
            IEnumerator usedEnumerator = paramList.GetEnumerator();

            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it");
                }

                //Set a new connection and DataSet
                //////if (connection == null)
                //////{
                //////    connection = new SqlConnection(connectionString);
                //////}
                if (_sqlServerDb == null)
                {
                    _sqlServerDb = new SqlDatabase(connectionString);
                }
                DataSet dataSet = new DataSet();

                //Define the command object and set commandtype to process Stored Procedure
                //command = new SqlCommand(SPName, connection);
                command = new SqlCommand(SPName);

                command.CommandType = CommandType.StoredProcedure;

                SetCommandTimeout();
                //Move through the paramList wiht the help of the enumerator
                while ((usedEnumerator.MoveNext()))
                {
                    privateUsedParameter = null;
                    //Get parameter in paramList
                    privateUsedParameter = (Parameter)usedEnumerator.Current;
                    //Convert paramter to SQLParameter
                    privateParameter = ConvertParameters(privateUsedParameter);
                    //Add converted parameter to the command object that imports data through the DataAdapter
                    command.Parameters.Add(privateParameter);
                }
                //Have the DataAdapter run the Stored Procedure
                ////dataAdapter = new SqlDataAdapter(command);
                //Depending on table name passed, create the DataSet with or without specifically naming the table
                dataSet = _sqlServerDb.ExecuteDataSet(command);
                ////dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ExceptionObject)
            {
                //The exception is passed to the calling code
                throw new Exception(excpMessage, ExceptionObject);
            }
            finally
            {
                //Always close the connection as soon as possible(only then will object be allowed to go out of scope)
                if (this.transactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    CloseConnection();
                }
            }
        }
        /// <summary>
        /// This method executes the storedprocedure and returns  DataReader
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteSPAndReturnDataReader(string SPName)
        {
            //Validate Stored Procedure
            ValidateSPStatement(ref SPName);

            //Setting the objects to handle parameters
            Parameter privateUsedParameter = default(Parameter);
            //will return the specific parameter in the paramList
            SqlParameter privateParameter = default(SqlParameter);
            //will contain the converted SQLParameter
            //The usedEnumerator makes it easy to step through the list of parameters in the paramList
            IEnumerator usedEnumerator = paramList.GetEnumerator();

            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it");
                }

                if (_sqlServerDb == null)
                    _sqlServerDb = new SqlDatabase(connectionString);

                command = new SqlCommand(SPName);

                SetCommandTimeout();
                //if we are in transaction, then set the command object's transaction object
                if (this.TransactionStatus == MyTransactionStatus.InTransaction)
                {
                    command.Transaction = this.transaction;
                }
                command.CommandType = CommandType.StoredProcedure;

                //Move through the paramList wiht the help of the enumerator
                while ((usedEnumerator.MoveNext()))
                {
                    privateUsedParameter = null;
                    //Get parameter in paramList
                    privateUsedParameter = (Parameter)usedEnumerator.Current;
                    //Convert paramter to SQLParameter
                    privateParameter = ConvertParameters(privateUsedParameter);
                    //Add converted parameter to the command object that imports data through the DataAdapter
                    command.Parameters.Add(privateParameter);
                }

                //Start. Added to implement Enterprise Library 5.0 Mayank Kukadia. 03/14/2011.
                // Enterprise Library 5.0 returns RefCounterDataReder instead of SqlDataReader and it must be closed. 
                command.Connection = _sqlServerDb.CreateConnection() as SqlConnection;
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                //dataReader = (SqlDataReader)_sqlServerDb.ExecuteReader(command);
                //End. Added to implement Enterprise Library 5.0 Mayank Kukadia. 03/14/2011.

                return dataReader;
            }
            catch (Exception ExceptionObject)
            {
                //The exception is passed back to the calling code, with our custom message and specific exception information
                if (this.transactionStatus != MyTransactionStatus.InTransaction)
                {
                    CloseConnection();
                }
                throw new Exception(excpMessage, ExceptionObject);
            }
        }

        /// <summary>
        /// This method executes Stored Proc and returns xml string
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public string ExecuteSPAndReturnXMLReader(string SPName)
        {
            //Validate Stored Procedure
            ValidateSPStatement(ref SPName);

            //Setting the objects to handle parameters
            Parameter privateUsedParameter = default(Parameter);
            //will return the specific parameter in the paramList
            SqlParameter privateParameter = default(SqlParameter);
            //will contain the converted SQLParameter
            //The usedEnumerator makes it easy to step through the list of parameters in the paramList
            IEnumerator usedEnumerator = paramList.GetEnumerator();
            string privateXMLString = string.Empty;
            //We need this string to build the XML Statement

            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it");
                }

                if (_sqlServerDb == null)
                    _sqlServerDb = new SqlDatabase(connectionString);
                //Define the command object and set commandtype to process Stored Procedure
                command = new SqlCommand(SPName);
                command.CommandType = CommandType.StoredProcedure;

                SetCommandTimeout();
                //Move through the paramList wiht the help of the enumerator
                while ((usedEnumerator.MoveNext()))
                {
                    privateUsedParameter = null;
                    //Get parameter in paramList
                    privateUsedParameter = (Parameter)usedEnumerator.Current;
                    //Convert paramter to SQLParameter
                    privateParameter = ConvertParameters(privateUsedParameter);
                    //Add converted parameter to the command object that imports data through the DataAdapter
                    command.Parameters.Add(privateParameter);
                }
                xmlReader = _sqlServerDb.ExecuteXmlReader(command);
                //Build the XML string
                while (!(xmlReader.Read() == false))
                {
                    privateXMLString += xmlReader.ReadOuterXml() + "<BR>";
                }


                return privateXMLString;
            }
            catch (Exception ExceptionObject)
            {
                throw new Exception(excpMessage, ExceptionObject);
            }
            finally
            {
                //Close the XMLReader and the Connection - we don't need it anymore
                xmlReader.Close();
                if (this.transactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    CloseConnection();
                }
            }
        }
        /// <summary>
        /// This method executes storedproc and returns the stored proc output parameter
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public ArrayList ExecuteSPAndReturnOutput(string SPName)
        {
            //Validate Stored Procedure
            ValidateSPStatement(ref SPName);

            //Setting the objects to handle parameters
            Parameter privateUsedParameter = default(Parameter);
            //will return the specific parameter in the paramList
            SqlParameter privateParameter = default(SqlParameter);
            //will contain the converted SQLParameter
            //The usedEnumerator makes it easy to step through the list of parameters in the paramList
            IEnumerator usedEnumerator = paramList.GetEnumerator();
            ArrayList outputParameters = new ArrayList();
            //We need this arraylist to hold output parameters
            // SqlParameter privateParameterOut = default(SqlParameter);
            //Helps to create the output parameter array

            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it");
                }

                if (_sqlServerDb == null)
                    _sqlServerDb = new SqlDatabase(connectionString);

                //Define the command object and set commandtype to process Stored Procedure
                command = new SqlCommand(SPName);

                command.CommandType = CommandType.StoredProcedure;

                SetCommandTimeout();
                //Move through the paramList with the help of the enumerator
                while ((usedEnumerator.MoveNext()))
                {
                    privateUsedParameter = null;
                    //Get parameter in paramList
                    privateUsedParameter = (Parameter)usedEnumerator.Current;
                    //Convert paramter to SQLParameter
                    privateParameter = ConvertParameters(privateUsedParameter);
                    //Add converted parameter to the command object that imports data through the DataAdapter
                    command.Parameters.Add(privateParameter);
                }

                ExecuteMyQuery();

                //Iterate through all output parameters and return values
                foreach (System.Data.SqlClient.SqlParameter parameterOut in command.Parameters)
                {
                    if (parameterOut.Direction == ParameterDirection.Output | parameterOut.Direction == ParameterDirection.ReturnValue)
                    {
                        //Add each output and return value to our output paramterlist
                        outputParameters.Add(parameterOut.Value);
                    }
                }

                //Return the array list of output parameter values

                return outputParameters;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //Close the Connection
                if (this.transactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    CloseConnection();
                }
            }
        }
        /// <summary>
        /// This private method will be called in all the other methods to execute the SQL query or sp
        /// </summary>
        private void ExecuteMyQuery()
        {
            try
            {
                //if we are in transaction, then set the command object's transaction object
                if (this.TransactionStatus == MyTransactionStatus.BeginTransaction)
                {
                    this.transaction = (SqlTransaction)_sqlServerDb.CreateConnection().BeginTransaction();
                    this.transactionStatus = MyTransactionStatus.InTransaction;
                }
                //if we are in transaction, then set the command object's transaction object
                if (this.TransactionStatus != MyTransactionStatus.NotInTransaction)
                {
                    command.Transaction = this.transaction;
                }
                //System.Diagnostics.EventLog.WriteEntry("SQLServer", "Executing command.ExecuteNonQuery()");
                //System.Diagnostics.EventLog.WriteEntry("SQLServer", "command" + command.CommandType.ToString());
                try
                {
                    int result = _sqlServerDb.ExecuteNonQuery(command);
                    //System.Diagnostics.EventLog.WriteEntry("SQLServer", "Completed Executing command.ExecuteNonQuery()::result=" + result.ToString());

                }
                catch (SqlException sqlException)
                {
                    //System.Diagnostics.EventLog.WriteEntry("SQLServer", "SqlException raised::ERR:" + sqlException.Message + ":" + sqlException.InnerException.Message);
                }

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SQLServer", "Error raised in ExecuteMyQuery()::ERR:" + ex.StackTrace + ":" + ex.Message + ":" + ex.InnerException.Message);
                throw;
            }

        }

        /// <summary>
        /// This method executes storedprocedure and do not return any results back.
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public bool ExecuteSPNoReturn(string SPName)
        {
            bool functionReturnValue = false;
            //System.Diagnostics.EventLog.WriteEntry("SQLServer", "Start  ExecuteSPNoReturn() : SPName=" + SPName);

            //Validate Stored Procedure
            ValidateSPStatement(ref SPName);

            //Setting the objects to handle parameters
            Parameter privateUsedParameter = default(Parameter);
            //will return the specific parameter in the paramList
            SqlParameter privateParameter = default(SqlParameter);
            //will contain the converted SQLParameter
            //The usedEnumerator makes it easy to step through the list of parameters in the paramList
            IEnumerator usedEnumerator = paramList.GetEnumerator();
            functionReturnValue = false;
            try
            {
                //Check to see if this object has already been disposed
                if (isDisposed == true)
                {
                    throw new ObjectDisposedException(moduleName, "This object has already been disposed. You cannot reuse it");
                }

                if (_sqlServerDb == null)
                {
                    _sqlServerDb = new SqlDatabase(connectionString);
                }

                //Define the command object and set commandtype to process Stored Procedure
                //  System.Diagnostics.EventLog.WriteEntry("SQLServer", "connection=" + _sqlServerDb.ConnectionString);

                command = new SqlCommand(SPName);
                command.CommandType = CommandType.StoredProcedure;

                SetCommandTimeout();
                //Move through the paramList wiht the help of the enumerator
                while ((usedEnumerator.MoveNext()))
                {
                    privateUsedParameter = null;
                    //Get parameter in paramList
                    privateUsedParameter = (Parameter)usedEnumerator.Current;
                    //Convert paramter to SQLParameter
                    privateParameter = ConvertParameters(privateUsedParameter);
                    //Add converted parameter to the command object that imports data through the DataAdapter
                    //   System.Diagnostics.EventLog.WriteEntry("SQLServer", "privateParameter.ParameterName=" + privateParameter.ParameterName);
                    //   System.Diagnostics.EventLog.WriteEntry("SQLServer", "privateParameter.Value=" + privateParameter.Value);

                    command.Parameters.Add(privateParameter);
                }
                //  System.Diagnostics.EventLog.WriteEntry("SQLServer", "Calling SESDatabase.ExecuteNonQuery():ConnectionString=" + this.ConnectionString);               
                int result = _sqlServerDb.ExecuteNonQuery(command);

                //ExecuteMyQuery();
                //  System.Diagnostics.EventLog.WriteEntry("SQLServer", "Completed ExecuteMyQuery():result=" + result.ToString());

                functionReturnValue = true;
            }
            catch (Exception ExceptionObject)
            {
                //  System.Diagnostics.EventLog.WriteEntry("SQLServer", "Error Raised in ExecuteMyQuery():ERR=" + ExceptionObject.Message + ":" + ExceptionObject.InnerException.Message + ":" + ExceptionObject.StackTrace);
                //The exception is passed to the calling code
                throw new Exception(ExceptionObject.Message, ExceptionObject);
            }
            finally
            {
                if (this.TransactionStatus == MyTransactionStatus.NotInTransaction)
                {
                    //Always close the connection as soon as possible(only then will object be allowed to go out of scope)
                    CloseConnection();
                }
            }
            //  System.Diagnostics.EventLog.WriteEntry("SQLServer", "End  ExecuteSPNoReturn() : SPName=" + SPName);

            return functionReturnValue;
        }
        /// <summary>
        /// Adds parameter to ParameterList
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <param name="Value"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="Direction"></param>
        public void AddParameter(string ParameterName, object Value, SqlDbType dbType, int size, ParameterDirection Direction)
        {
            Parameter buildParameter = null;
            buildParameter = new Parameter(ParameterName, Value, dbType, size, Direction);
            paramList.Add(buildParameter);
        }

        /// <summary>
        /// This Parameter class is used internally inside the SQLServer class to handle the parameters being passed
        /// </summary>
        public class Parameter
        {
            public string ParameterName;
            public object ParameterValue;
            public SqlDbType ParameterDataType;
            public int ParameterSize;
            public ParameterDirection ParameterDirectionUsed;
            /// <summary>
            /// Constructor of the Parameter class
            /// </summary>
            /// <param name="passedParameterName"></param>
            /// <param name="passedValue"></param>
            /// <param name="passedSQLType"></param>
            /// <param name="passedSize"></param>
            /// <param name="passedDirection"></param>
            public Parameter(string passedParameterName, object passedValue, SqlDbType passedSQLType, int passedSize, ParameterDirection passedDirection)
            {

                ParameterName = passedParameterName;
                ParameterValue = passedValue;
                ParameterDataType = passedSQLType;
                ParameterSize = passedSize;
                ParameterDirectionUsed = passedDirection;
            }
            /// <summary>
            /// Overloaded constructor of Parameter class
            /// </summary>
            /// <param name="passedParameterName"></param>
            /// <param name="passedValue"></param>
            /// <param name="passedSQLType"></param>
            /// <param name="passedSize"></param>
            public Parameter(string passedParameterName, object passedValue, SqlDbType passedSQLType, int passedSize)
            {

                ParameterName = passedParameterName;
                ParameterValue = passedValue;
                ParameterDataType = passedSQLType;
                ParameterSize = passedSize;
                ParameterDirectionUsed = ParameterDirection.Input;
            }

        }
        /// <summary>
        /// Converts internal Parameter class object to SqlParameter class object
        /// </summary>
        /// <param name="passedParameter"></param>
        /// <returns></returns>
        private SqlParameter ConvertParameters(Parameter passedParameter)
        {

            SqlParameter returnSQLParameter = new SqlParameter();

            returnSQLParameter.ParameterName = passedParameter.ParameterName;
            returnSQLParameter.Value = passedParameter.ParameterValue;
            returnSQLParameter.SqlDbType = passedParameter.ParameterDataType;
            returnSQLParameter.Size = passedParameter.ParameterSize;
            returnSQLParameter.Direction = passedParameter.ParameterDirectionUsed;


            return returnSQLParameter;
        }

    }
}
