using Aditi.WindowsAzure.StorageHelper;
using DataHub.Common;
using DataHub.Common.Models;
using DataHub.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHub.Common.Enums;

namespace DataHub.Notification.Gateway.Helper
{
    public class DataStorageHelper
    {

        #region Notification Methods
        /// <summary>
        /// Write Notification Details To Table Storage
        /// </summary>
        /// <param name="tenantClient"></param>
        /// <param name="fileID"></param>
        /// <param name="fileUploadStatus"></param>
        public bool WriteNotificationToStorage(string TenantId,String fileID, String fileUploadStatus, string notificationMode, string recipientDetail, bool isNotified)
        {
            try
            {
                bool notifyStatus = false;
                string notificationStatusTableName = Common.Constants.notificationStatusTableName;
                string timeString = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
                TenantEntity tenant = new TenantEntity(TenantId, timeString);
                tenant.FileID = fileID;
                tenant.FileUploadStatus = fileUploadStatus;
                tenant.NotificationMode = notificationMode;
                tenant.RecipientDetail = recipientDetail;
                tenant.IsNotified = isNotified;
              
                TableHelper _tableHelper = new TableHelper(Common.Constants.StorageConnectionString);
                notifyStatus = _tableHelper.InsertEntity(notificationStatusTableName, tenant);

                return notifyStatus;
            }
            catch (Exception ex)
            {
                DataHubTraceListener traceListener = new DataHubTraceListener();
                traceListener.WriteLog(Categories.Error, "Eventing", ex.Message.ToString(), ex.StackTrace.ToString());
                return false;
            }
        }

        #endregion
    }
}
