namespace eZeeFlow.StorageHelper
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    
    public class QueueHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private CloudStorageAccount _storageAccount;
        /// <summary>
        /// 
        /// </summary>
        private CloudQueueClient _queueClient;

        // Constructor - get settings from a hosted service configuration

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueHelper"/> class.
        /// </summary>
        /// <param name="configurationSettingName">Name of the configuration setting.</param>
        /// <remarks></remarks>
        public QueueHelper(string configurationSettingName)
        {
            var connectString = CloudConfigurationManager.GetSetting(configurationSettingName);
            _storageAccount = CloudStorageAccount.Parse(connectString);
            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queueClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.FromSeconds(1));
        }

        // List queues.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Lists the queues.
        /// </summary>
        /// <param name="queueList">The queue list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ListQueues(out List<CloudQueue> queueList)
        {
            queueList = new List<CloudQueue>();

            try
            {
                IEnumerable<CloudQueue> queues = _queueClient.ListQueues();
                if (queues != null)
                {
                    queueList.AddRange(queues);
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


        // Create a queue. 
        // Return true on success, false if already exists, throw exception on error.

        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CreateQueue(string queueName)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.CreateIfNotExist();
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


        // Delete a queue. 
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Deletes the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteQueue(string queueName)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.Delete();
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


        // Get queue metadata.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Gets the queue metadata.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetQueueMetadata(string queueName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.FetchAttributes();

                metadata = queue.Metadata;

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


        // Set queue metadata.
        // Return true on success, false if not found, throw exception on error.

        /// <summary>
        /// Sets the queue metadata.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="metadataList">The metadata list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SetQueueMetadata(string queueName, NameValueCollection metadataList)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.Metadata.Clear();
                queue.Metadata.Add(metadataList);
                queue.SetMetadata();
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


        // Peek the next message from a queue. 
        // Return true on success (message available), false if no message or no queue, throw exception on error.

        /// <summary>
        /// Peeks the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool PeekMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                message = queue.PeekMessage();
                return message != null;
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


        // Retrieve the next message from a queue. 
        // Return true on success (message available), false if no message or no queue, throw exception on error.

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                message = queue.GetMessage();
                return message != null;
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
        /// <summary>
        /// Get the Message and sets visibility timeout.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="visibilityTimeOut"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool GetMessage(string queueName,TimeSpan visibilityTimeOut,out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                message = queue.GetMessage(visibilityTimeOut);
                return message != null;
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

        // Create or update a blob. 
        // Return true on success, false if already exists, throw exception on error.

        /// <summary>
        /// Puts the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool PutMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.AddMessage(message);
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


        // Clear all messages from a queue. 
        // Return true on success, false if already exists, throw exception on error.

        /// <summary>
        /// Clears the messages.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ClearMessages(string queueName)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                queue.Clear();
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


        // Delete a previously read message. 
        // Return true on success, false if already exists, throw exception on error.

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DeleteMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                if (message != null)
                {
                    queue.DeleteMessage(message);
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

    }
}


