using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace eZeeFlow.StorageHelper
{
    public class BlobHelper : IBlobHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobStorage;

        ////TDODO: get conString as param.
        //public BlobHelper(string configurationSettingName)
        //{
        //    _storageAccount = CloudStorageAccount.FromConfigurationSetting(configurationSettingName);
        //    _blobStorage = _storageAccount.CreateCloudBlobClient();
        //    _blobStorage.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.FromSeconds(1));
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobHelper"/> class.
        /// </summary>
        /// <param name="storageAccountName">Storage connection string.</param>
        public BlobHelper(string storageConnectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobStorage = _storageAccount.CreateCloudBlobClient();
            _blobStorage.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Enumerate the containers in a storage account. Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerList">The container list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ListContainers(out List<CloudBlobContainer> containerList)
        {
            containerList = new List<CloudBlobContainer>();

            try
            {
                IEnumerable<CloudBlobContainer> containers = _blobStorage.ListContainers();
                if (containers != null)
                {
                    containerList.AddRange(containers);
                }
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

        /// <summary>
        /// return the blob container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            return _blobStorage.GetContainerReference(containerName);
        }

        /// <summary>
        /// Creates the container.Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CreateContainer(string containerName)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                BlobRequestOptions options = new BlobRequestOptions()
                {
                    RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(15))
                };
                container.CreateIfNotExist(options);

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

        /// <summary>
        /// Gets the container access control.
        /// Return true on success, false if not found, throw exception on error.
        /// Access level set to container|blob|private.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="accessLevel">The access level.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetContainerACL(string containerName, out string accessLevel)
        {
            accessLevel = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();
                switch (permissions.PublicAccess)
                {
                    case BlobContainerPublicAccessType.Container:
                        accessLevel = "container";
                        break;
                    case BlobContainerPublicAccessType.Blob:
                        accessLevel = "blob";
                        break;
                    case BlobContainerPublicAccessType.Off:
                        accessLevel = "private";
                        break;
                }
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

        /// <summary>
        /// Sets the container control.
        /// Return true on success, false if not found, throw exception on error.
        /// Set access level to container|blob|private.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="accessLevel">The access level.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SetContainerAcl(string containerName, string accessLevel)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                BlobContainerPermissions permissions = new BlobContainerPermissions();
                switch(accessLevel)
                {
                    case "container":
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        break;
                    case "blob":
                        permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                        break;
                    case "private":
                    default:
                        permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                        break;
                }
                BlobRequestOptions options = new BlobRequestOptions()
                {
                    RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(15))
                };
                container.SetPermissions(permissions, options);
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

        /// <summary>
        /// Gets the container access policy.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policies">The policies.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetContainerAccessPolicy(string containerName, out SortedList<string, SharedAccessPolicy> policies)
        {
            policies = new SortedList<string, SharedAccessPolicy>();

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();

                if (permissions.SharedAccessPolicies != null)
                {
                    foreach (KeyValuePair<string, SharedAccessPolicy> policy in permissions.SharedAccessPolicies)
                    {
                        policies.Add(policy.Key, policy.Value);
                    }
                }

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

        /// Sets the container access policy.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policies">The policies.</param>
        /// <returns></returns>
        public bool SetContainerAccessPolicy(string containerName, SortedList<string, SharedAccessPolicy> policies)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();

                permissions.SharedAccessPolicies.Clear();

                if (policies != null)
                {
                    foreach (KeyValuePair<string, SharedAccessPolicy> policy in policies)
                    {
                        permissions.SharedAccessPolicies.Add(policy.Key, policy.Value);
                    }
                }

                container.SetPermissions(permissions);

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

        /// <summary>
        /// Generates the shared access signature.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        public bool GenerateSharedAccessSignature(string containerName, SharedAccessPolicy policy, out string signature)
        {
            signature = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                signature = container.GetSharedAccessSignature(policy);
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
        
        /// <summary>
        /// Generates the shared access signature.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        public bool GenerateSharedAccessSignature(string containerName, string policyName, out string signature)
        {
            signature = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                signature = container.GetSharedAccessSignature(new SharedAccessPolicy(), policyName);
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

        /// <summary>
        /// Gets the container properties.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        public bool GetContainerProperties(string containerName, out BlobContainerProperties properties)
        {
            properties = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                container.FetchAttributes();
                properties = container.Properties;
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

        /// <summary>
        /// Gets the container metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public bool GetContainerMetadata(string containerName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                container.FetchAttributes();
                metadata = container.Metadata;
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

        /// <summary>
        /// Sets the container metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public bool SetContainerMetadata(string containerName, NameValueCollection metadata)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                container.Metadata.Clear();
                container.Metadata.Add(metadata);
                container.SetMetadata();
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
        
        /// <summary>
        /// Deletes the container.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        public bool DeleteContainer(string containerName)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                container.Delete();
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

        /// <summary>
        /// Lists the blobs in a container.
        /// Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobList">The BLOB list.</param>
        /// <returns></returns>
        public bool ListBlobs(string containerName, out List<CloudBlob> blobList)
        {
            blobList = new List<CloudBlob>();

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                IEnumerable<IListBlobItem> blobs = container.ListBlobs();
                if (blobs != null)
                {
                    foreach (CloudBlob blob in blobs)
                    {
                        blobList.Add(blob);
                    }
                }
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

        /// <summary>
        /// (create or update) a blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public string PutBlob(string containerName, string blobName, string content)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.UploadText(content);
                return "true";
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

        /// <summary>
        /// Put (create or update) a page blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="pageBlobSize">Size of the page BLOB.</param>
        /// <returns></returns>
        public bool PutBlob(string containerName, string blobName, int pageBlobSize)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                blob.Create(pageBlobSize);
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
        
        /// <summary>
        /// Put (create or update) a blob conditionally based on expected ETag value.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <param name="expectedETag">The expected E tag.</param>
        /// <returns></returns>
        public bool PutBlobIfUnchanged(string containerName, string blobName, string content)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                NameValueCollection metadata = new NameValueCollection();               
                blob.UploadText(content);
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

        /// <summary>
        /// Put (create or update) a blob with an MD5 hash.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public bool PutBlobWithMD5(string containerName, string blobName, string content)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                string md5 = Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(content)));
                blob.Properties.ContentMD5 = md5;
                blob.UploadText(content);
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

        /// <summary>
        /// Get a page of a page blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="pageOffset">The page offset.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public bool GetPage(string containerName, string blobName, int pageOffset, int pageSize, out string content)
        {
            content = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                BlobStream stream = blob.OpenRead();
                byte[] data = new byte[pageSize];
                stream.Seek(pageOffset, SeekOrigin.Begin);
                stream.Read(data, 0, pageSize);
                content = new UTF8Encoding().GetString(data);
                stream.Close();

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

        /// <summary>
        /// Put a page of a page blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <param name="pageOffset">The page offset.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public bool PutPage(string containerName, string blobName, string content, int pageOffset, int pageSize)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                MemoryStream stream = new MemoryStream(new UTF8Encoding().GetBytes(content));
                blob.WritePages(stream, pageOffset);
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

        /// <summary>
        /// Retrieve the list of regions in use for a page blob. 
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="regions">The regions.</param>
        /// <returns></returns>
        public bool GetPageRegions(string containerName, string blobName, out PageRange[] regions)
        {
            regions = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);

                IEnumerable<PageRange> regionList = blob.GetPageRanges();
                regions = new PageRange[regionList.Count()];
                int i = 0;
                foreach (PageRange region in regionList)
                {
                    regions[i++] = region;
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

        /// <summary>
        /// Retrieve the list of uploaded blocks for a blob. 
        /// Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="blockIds">The block ids.</param>
        /// <returns></returns>
        public bool GetBlockList(string containerName, string blobName, out string[] blockIds)
        {
            blockIds = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                IEnumerable<ListBlockItem> blobs = blob.DownloadBlockList();
                blockIds = new string[blobs.Count()];
                int i = 0;
                foreach (ListBlockItem block in blobs)
                {
                    blockIds[i++] = block.Name;
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

        /// <summary>
        ///  Get (retrieve) a blob and return its content.
        ///  Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public string GetBlob(string containerName, string blobName, out string content)
        {
            content = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                content = blob.DownloadText();
                return "true";
            }
            catch (StorageClientException SCEx)
            {
                if (SCEx.StatusCode == HttpStatusCode.NotFound)
                {
                    return "404";
                }
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the BLOB.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <returns></returns>
        public bool DeleteBlob(string containerName, string blobName)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.DeleteIfExists();
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

        /// <summary>
        /// Gets the BLOB metadata.
        ///  Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public string GetBlobMetadata(string containerName, string blobName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.FetchAttributes();

                metadata = blob.Metadata;

                return "true";
            }
            catch (StorageClientException SCEx)
            {
                if (SCEx.StatusCode == HttpStatusCode.NotFound)
                {
                    return "404";
                }
                throw new ApplicationException(SCEx.Message, SCEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the BLOB metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public string SetBlobMetadata(string containerName, string blobName, NameValueCollection metadata)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.Metadata.Clear();
                blob.Metadata.Add(metadata);
                blob.SetMetadata();

                return "true";
            }
            catch (StorageClientException SCEx)
            {
                if ((int)SCEx.StatusCode == 404)
                {
                    return "404";
                }

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="leaseAction"></param>
        /// <param name="timeout"></param>
        /// <param name="leaseId"></param>
        /// <returns></returns>
        public bool LeaseBlob(string containerName, string blobName, string leaseAction, int timeout, ref string leaseId)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);

                switch (leaseAction)
                {
                    case "acquire":                        
                        leaseId = AcquireLease(blob, timeout);
                        break;
                    case "release":
                        LeaseOperation(blob, leaseId, LeaseAction.Release, timeout);
                        break;
                    case "renew":
                        LeaseOperation(blob, leaseId, LeaseAction.Renew, timeout);
                        break;
                    case "break":
                        LeaseOperation(blob, null, LeaseAction.Break, timeout);
                        break;
                }

                return true;
            }
            catch (StorageClientException)
            {
                return false;
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private string AcquireLease(CloudBlob blob, int timeout)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var request = BlobRequest.Lease(transformedUri, timeout, LeaseAction.Acquire, null);
            blob.ServiceClient.Credentials.SignRequest(request);
            using (var response = request.GetResponse())
            {
                return response.Headers["x-ms-lease-id"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="leaseId"></param>
        /// <param name="action"></param>
        /// <param name="timeout"></param>
        private void LeaseOperation(CloudBlob blob, string leaseId, LeaseAction action, int timeout)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var request = BlobRequest.Lease(transformedUri, timeout, action, leaseId);
            creds.SignRequest(request);
            request.GetResponse().Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public MemoryStream GetBlobStream(string containerName, string blobName)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.DownloadToStream(stream);
                return stream;
            }
            catch (StorageClientException SCEx)
            {
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// (create or update) a blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">data in byte array.</param>
        /// <returns></returns>
        public bool UploadByteArray(string containerName, string blobName, string contentType, byte[] data)
        {
            try
            {
                CloudBlobContainer container = _blobStorage.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.Properties.ContentType = contentType;
                blob.UploadByteArray(data);
                return true;
            }
            catch (StorageClientException SCEx)
            {
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
