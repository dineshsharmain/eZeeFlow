using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

namespace eZeeFlow.StorageHelper
{
    public interface IBlobHelper
    {
        /// <summary>
        /// Enumerate the containers in a storage account. Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerList">The container list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool ListContainers(out List<CloudBlobContainer> containerList);

        /// <summary>
        /// return the blob container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        CloudBlobContainer GetBlobContainer(string containerName);

        /// <summary>
        /// Creates the container.Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool CreateContainer(string containerName);

        /// <summary>
        /// Gets the container access control.
        /// Return true on success, false if not found, throw exception on error.
        /// Access level set to container|blob|private.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="accessLevel">The access level.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool GetContainerACL(string containerName, out string accessLevel);

        /// <summary>
        /// Sets the container control.
        /// Return true on success, false if not found, throw exception on error.
        /// Set access level to container|blob|private.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="accessLevel">The access level.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool SetContainerAcl(string containerName, string accessLevel);

        /// <summary>
        /// Gets the container access policy.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policies">The policies.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool GetContainerAccessPolicy(string containerName, out SortedList<string, SharedAccessPolicy> policies);

        /// Sets the container access policy.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policies">The policies.</param>
        /// <returns></returns>
        bool SetContainerAccessPolicy(string containerName, SortedList<string, SharedAccessPolicy> policies);

        /// <summary>
        /// Generates the shared access signature.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        bool GenerateSharedAccessSignature(string containerName, SharedAccessPolicy policy, out string signature);

        /// <summary>
        /// Generates the shared access signature.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        bool GenerateSharedAccessSignature(string containerName, string policyName, out string signature);

        /// <summary>
        /// Gets the container properties.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        bool GetContainerProperties(string containerName, out BlobContainerProperties properties);

        /// <summary>
        /// Gets the container metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        bool GetContainerMetadata(string containerName, out NameValueCollection metadata);

        /// <summary>
        /// Sets the container metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        bool SetContainerMetadata(string containerName, NameValueCollection metadata);

        /// <summary>
        /// Deletes the container.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        bool DeleteContainer(string containerName);

        /// <summary>
        /// Lists the blobs in a container.
        /// Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobList">The BLOB list.</param>
        /// <returns></returns>
        bool ListBlobs(string containerName, out List<CloudBlob> blobList);

        /// <summary>
        /// (create or update) a blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        string PutBlob(string containerName, string blobName, string content);

        /// <summary>
        /// Put (create or update) a page blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="pageBlobSize">Size of the page BLOB.</param>
        /// <returns></returns>
        bool PutBlob(string containerName, string blobName, int pageBlobSize);

        /// <summary>
        /// Put (create or update) a blob conditionally based on expected ETag value.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <param name="expectedETag">The expected E tag.</param>
        /// <returns></returns>
        bool PutBlobIfUnchanged(string containerName, string blobName, string content);

        /// <summary>
        /// Put (create or update) a blob with an MD5 hash.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        bool PutBlobWithMD5(string containerName, string blobName, string content);

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
        bool GetPage(string containerName, string blobName, int pageOffset, int pageSize, out string content);

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
        bool PutPage(string containerName, string blobName, string content, int pageOffset, int pageSize);

        /// <summary>
        /// Retrieve the list of regions in use for a page blob. 
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="regions">The regions.</param>
        /// <returns></returns>
        bool GetPageRegions(string containerName, string blobName, out PageRange[] regions);

        /// <summary>
        /// Retrieve the list of uploaded blocks for a blob. 
        /// Return true on success, false if already exists, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="blockIds">The block ids.</param>
        /// <returns></returns>
        bool GetBlockList(string containerName, string blobName, out string[] blockIds);

        /// <summary>
        ///  Get (retrieve) a blob and return its content.
        ///  Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        string GetBlob(string containerName, string blobName, out string content);

        /// <summary>
        /// Deletes the BLOB.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <returns></returns>
        bool DeleteBlob(string containerName, string blobName);

        /// <summary>
        /// Gets the BLOB metadata.
        ///  Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        string GetBlobMetadata(string containerName, string blobName, out NameValueCollection metadata);

        /// <summary>
        /// Sets the BLOB metadata.
        /// Return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        string SetBlobMetadata(string containerName, string blobName, NameValueCollection metadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="leaseAction"></param>
        /// <param name="timeout"></param>
        /// <param name="leaseId"></param>
        /// <returns></returns>
        bool LeaseBlob(string containerName, string blobName, string leaseAction, int timeout, ref string leaseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        MemoryStream GetBlobStream(string containerName, string blobName);

        /// <summary>
        /// (create or update) a blob.
        /// Return true on success, false if unable to create, throw exception on error.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="content">data in byte array.</param>
        /// <returns></returns>
        bool UploadByteArray(string containerName, string blobName, string contentType, byte[] data);
    }

    public interface IBlobHelperFactory
    {
        IBlobHelper GetBlobHelper(string connString);
    }

}