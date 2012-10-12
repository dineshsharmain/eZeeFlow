namespace eZeeFlow.StorageHelper
{
    using eZeeFlow.StorageHelper.Properties;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class TableHelper
    {
        #region Private Membres
        /// <summary>
        /// 
        /// </summary>
        private CloudStorageAccount _storageAccount;
        /// <summary>
        /// 
        /// </summary>
        private CloudTableClient _tableStorage;
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TableHelper"/> class.
        /// </summary>
        /// <param name="configurationSettingName">Name of the configuration setting.</param>
        /// <remarks></remarks>
        public TableHelper(string configurationSettingName)
        {
            var connectString = CloudConfigurationManager.GetSetting(configurationSettingName);
            _storageAccount = CloudStorageAccount.Parse(connectString);

            _tableStorage = _storageAccount.CreateCloudTableClient();
            _tableStorage.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.FromSeconds(1));
        }
        

        // List Tables.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Lists the tables.
        /// </summary>
        /// <param name="tableList">The table list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ListTables(out List<string> tableList)
        {
            tableList = new List<string>();

            try
            {
                IEnumerable<string> tables =  _tableStorage.ListTables();
                if (tables != null)
                {
                    tableList.AddRange(tables);
                }
                return true;
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Create Table.
        // Return true on success, throw exception on error.

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CreateTable(string tableName)
        {
            try
            {
                _tableStorage.CreateTableIfNotExist(tableName);
                return true;
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message,SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Delete Table.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Deletes the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteTable(string tableName)
        {
            try
            {
                _tableStorage.DeleteTableIfExist(tableName);
                return true;
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Insert entity.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool InsertEntity(string tableName, object value)
        {
            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                tableServiceContext.IgnoreResourceNotFoundException = true;                
                tableServiceContext.AddObject(tableName, value);
                tableServiceContext.SaveChangesWithRetries();
                return true;
            }
            catch (DataServiceRequestException DSEx)
            {
                if (DSEx.InnerException.Message.Contains("EntityAlreadyExists"))
                {
                    return false;
                }
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                if (SCEx.StatusCode == HttpStatusCode.Conflict)
                {
                    return true;
                }
                throw new ApplicationException(SCEx.Message, SCEx);
            }
        }

        /// <summary>
        /// Inserts the entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="objectList">The object list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool InsertEntities<T>(string tableName,Collection<T> objectList)
        {
            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                foreach (var obj in objectList)
                {                    
                    tableServiceContext.AddObject(tableName, obj);
                }
                tableServiceContext.IgnoreResourceNotFoundException = true;
                tableServiceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);

                return true;
            }
            catch (DataServiceRequestException DSEx)
            {
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                if (SCEx.StatusCode == HttpStatusCode.Conflict)
                {
                    return true;
                }
                throw new ApplicationException(SCEx.Message, SCEx);
            }           
        }

        // Retrieve an entity.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetEntity<T>(string tableName, string partitionKey, string rowKey, out T entity) where T : TableServiceEntity 
        {
            entity = null;

            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName) 
                                                        where e.PartitionKey == partitionKey && e.RowKey == rowKey 
                                                        select e);

                entity = entities.FirstOrDefault();

                return "true";
            }
            catch (DataServiceQueryException DSQEx)
            {
                if (DSQEx.Response.StatusCode == 404)
                {
                    return "404";
                }
                throw new ApplicationException(Resources.ExceptionDataNotFound, DSQEx);
            }
            catch (DataServiceRequestException DSEx)
            {
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                if ((int)SCEx.StatusCode == 404)
                {
                    return "404";
                }
                throw new ApplicationException(SCEx.Message, SCEx);
            }
        }

        
        // Query entities. Use LINQ clauses to filter data.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Queries the entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataServiceQuery<T> QueryEntities<T>(string tableName) where T : TableServiceEntity 
        {
            TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
            return tableServiceContext.CreateQuery<T>(tableName);
        }
            

        // Replace Update entity. Completely replace previous entity with new entity.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Replaces the update entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ReplaceUpdateEntity<T>(string tableName, string partitionKey, string rowKey, T obj) where T : TableServiceEntity
        {
            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                          where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                          select e);

                T entity = entities.FirstOrDefault();

                Type t = obj.GetType();
                PropertyInfo[] pi = t.GetProperties();

                foreach (PropertyInfo p in pi)
                {
                    p.SetValue(entity, p.GetValue(obj, null), null);
                }

                tableServiceContext.UpdateObject(entity);
                tableServiceContext.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

                return "true";
            }
            catch (DataServiceRequestException DSEx)
            {
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Merge update an entity (preserve previous properties not overwritten).
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Merges the update entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool MergeUpdateEntity<T>(string tableName, string partitionKey, string rowKey, T obj) where T : TableServiceEntity
        {
            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                          where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                          select e);

                T entity = entities.FirstOrDefault();

                Type t = obj.GetType();
                PropertyInfo[] pi = t.GetProperties();

                foreach (PropertyInfo p in pi)
                {
                    if (p.GetValue(obj, null) != null)
                    {
                        p.SetValue(entity, p.GetValue(obj, null), null);
                    }
                    else
                    {
                        p.SetValue(entity, p.GetValue(entity, null), null);
                    }
                }

                tableServiceContext.UpdateObject(entity);
                tableServiceContext.SaveChangesWithRetries();

                return true;
            }
            catch (DataServiceRequestException DSEx)
            {
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Delete entity.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteEntity<T>(string tableName, string partitionKey, string rowKey) where T : TableServiceEntity
        {
            try
            {
                TableServiceContext tableServiceContext = _tableStorage.GetDataServiceContext();
                IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                          where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                          select e);

                T entity = entities.FirstOrDefault();

                if (entities != null)
                {
                    tableServiceContext.DeleteObject(entity);
                    tableServiceContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (DataServiceRequestException DSEx)
            {
                throw new ApplicationException(DSEx.Message, DSEx);
            }
            catch (StorageClientException SCEx)
            {
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}


