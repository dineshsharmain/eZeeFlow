using eZeeFlow.Common;
using eZeeFlow.Logging;
using eZeeFlow.SqlService.Context;
using eZeeFlow.Common.Models;
using eZeeFlow.SqlService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eZeeFlow.Common.Enums;

namespace eZeeFlow.Notification.Gateway.Helper
{
    public class DataAccessHelper
    {
        private eZeeFlowContext context = new eZeeFlowContext();

        public List<string> GetNotificationModesForTenant(string tenantID, string eventType)
        {
            Guid tenantId = Guid.Parse(tenantID);
            string strfileUploadStatus = eventType;

            if (tenantId != Guid.Parse("{00000000-0000-0000-0000-000000000000}"))
            {
                try
                {
                    var notificationModes = from nots in context.Notification
                                            join fus in context.FileUploadStatus
                                            on nots.StatusId equals fus.Id
                                            join nm in context.NotificationMode
                                            on nots.NotificationModeId equals nm.Id
                                            where nots.TenantId == tenantId && fus.UploadStatus == strfileUploadStatus
                                            select nm.Mode;

                    return notificationModes.ToList<string>();
                }
                catch (Exception ex)
                {
                    eZeeFlowTraceListener traceListener = new eZeeFlowTraceListener();
                    traceListener.WriteLog(Categories.Error, "Eventing", ex.Message.ToString(), ex.StackTrace.ToString(), tenantId.ToString());

                }
            }
            return null;
        }

        public bool UpdateNotificationStatus(bool notificationStatus, string strTenantID, string strFileID)
        {
            Guid tenantId = Guid.Parse(strTenantID);
            Guid fileId = Guid.Parse(strFileID);
            if (tenantId != Guid.Parse("{00000000-0000-0000-0000-000000000000}") && fileId != Guid.Parse("{00000000-0000-0000-0000-000000000000}"))
            {
                try
                {
                    var fileUploadDetails = (FileUploadDetails)context.FileUploadDetails.Where(t => t.TenantId == tenantId && t.FileId == fileId).FirstOrDefault();
                    fileUploadDetails.IsNotified = notificationStatus;
                    context.SaveChanges();
                    return true;

                }
                catch (Exception ex)
                {
                    eZeeFlowTraceListener traceListener = new eZeeFlowTraceListener();
                    traceListener.WriteLog(Categories.Error, "Eventing", ex.Message.ToString(), ex.StackTrace.ToString(), strTenantID.ToString());
                }

            }
            return false;
        }


    }
}
