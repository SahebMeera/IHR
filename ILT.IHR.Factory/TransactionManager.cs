using System;

namespace ILT.IHR.Factory
{
    public sealed class TransactionManager
    {

        public static readonly TransactionManager Instance = new TransactionManager();
        private System.Collections.Generic.Dictionary<string, SQLServer> sqlServerObjPool = new System.Collections.Generic.Dictionary<string, SQLServer>();

        /// <summary>
        /// Singleton Pattern - cannot expose a public constructor - Use Instance member
        /// </summary>
        /// <remarks></remarks>
        private TransactionManager()
        {
        }

        /// <summary>
        /// Method that returns the SQL Server instance to be used for a particular transaction.
        /// </summary>
        /// <param name="transactionScopeID">The local unique identified of the transaction scope.</param>
        /// <returns>SQLServer object to use for accessing the database</returns>
        /// <remarks>
        /// This method will create a new SQL Server instance and add it to the pool if no instance
        /// exists for the submitted tranasction scope ID. An existing SQL Server instance will
        /// be returned if one had already been instantiated for a given transaction scope ID.
        /// </remarks>
        public SQLServer SQLServerToUse(string transactionScopeID)
        {
            try
            {
                if (!string.IsNullOrEmpty(transactionScopeID))
                {
                    lock (this)
                    {
                        if (sqlServerObjPool.ContainsKey(transactionScopeID))
                        {
                            //sql server instance exists for this transaction so return it
                            return sqlServerObjPool[transactionScopeID];
                        }
                        else
                        {
                            //create a new sqlServer instace and add to collection
                            SQLServer tmpSQLServer = new SQLServer();
                            sqlServerObjPool.Add(transactionScopeID, tmpSQLServer);
                            return sqlServerObjPool[transactionScopeID];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to determine SQLServer instance for transaction scope ID: " + transactionScopeID + " " + ex.Message);
            }
            //shouldnt ever get here
            return null;
        }

        /// <summary>
        /// Method to handle the completed event from a transaction scope object
        /// </summary>
        /// <param name="sender">Event sender object</param>
        /// <param name="e">Transactions.TransactionEventArgs object</param>
        /// <remarks>Removed the SQLServer instance created for a given transaction scope ID.</remarks>
        public void TransactionCompleted(object sender, System.Transactions.TransactionEventArgs e)
        {
            try
            {
                SQLServer removedSQLServer = null;
                if (!string.IsNullOrEmpty(e.Transaction.TransactionInformation.LocalIdentifier))
                {
                    lock (this)
                    {
                        if (sqlServerObjPool.ContainsKey(e.Transaction.TransactionInformation.LocalIdentifier))
                        {
                            //close the connection in use and remove the SQL Server instance from pool
                            removedSQLServer = sqlServerObjPool[e.Transaction.TransactionInformation.LocalIdentifier];
                            if (removedSQLServer != null)
                            {
                                removedSQLServer.CloseConnection();
                            }
                            sqlServerObjPool.Remove(e.Transaction.TransactionInformation.LocalIdentifier);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //dont throw object disposed exceptions this means that a factory the registered an
                //event handler for the ts.complete() method has gone out of scope.
                if (ex.GetType().ToString()!="System.ObjectDisposedException")
                {
                    throw new Exception("Unable to remove SQLServer instance for transactionID: " + e.Transaction.TransactionInformation.LocalIdentifier + " : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Method to determine the number of SQLServer instances in the pool.
        /// </summary>
        /// <returns>The count value of SQLServer instances</returns>
        /// <remarks></remarks>
        public int InstanceCount()
        {
            try
            {
                lock (this)
                {
                    return this.sqlServerObjPool.Count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get SQLServer instance count from pool: " + ex.Message);
            }
        }
    }
}
