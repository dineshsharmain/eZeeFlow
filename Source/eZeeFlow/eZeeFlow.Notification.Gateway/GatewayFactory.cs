using eZeeFlow.Common.Models;
using eZeeFlow.Notification.Gateway.Helper;
using System;
using System.Collections.Generic;
namespace eZeeFlow.Notification.Gateway
{

    public class GatewayFactory
    {
        Dictionary<string, INotificationGateway> gateways;

        public GatewayFactory()
        {
            gateways = new Dictionary<string, INotificationGateway>();
        }

        public INotificationGateway GetGateway(string gatewayCode)
        {
            switch (gatewayCode)
            {
                case Constants.EMail:
                    if (false == gateways.ContainsKey(Constants.EMail))
                        gateways.Add(Constants.EMail, new EmailGateway());
                    break;
                case Constants.SMS:
                    if (false == gateways.ContainsKey(Constants.SMS))
                        gateways.Add(Constants.SMS, new SMSGateway());
                    break;
                case Constants.HTTPRequest:
                    if (false == gateways.ContainsKey(Constants.HTTPRequest))
                        gateways.Add(Constants.HTTPRequest, new HttpGateway());
                    break;
                default:
                    throw new NotImplementedException();
            }
            return gateways[gatewayCode];
        }

        public List<INotificationGateway> GetNotificationsForTenant(string tenantId, string eventType)
        {
            DataAccessHelper dhelper = new DataAccessHelper();

            List<INotificationGateway> nGateWays = new List<INotificationGateway>();
            var notificationModes = dhelper.GetNotificationModesForTenant(tenantId, eventType);
            foreach (string nmode in notificationModes)
            {
                nGateWays.Add(GetGateway(nmode));
            }
            return nGateWays;
        }

        public bool WriteNotificationToStorage(List<NotificationDetails> notificationDetails, bool IsNotified, string notificationMode)
        {
            try
            {
                DataStorageHelper objStorageHelper = new DataStorageHelper();
                string tenantId;
                string fileID;
                foreach (var nd in notificationDetails)
                {
                    tenantId = nd.TenantID.ToString();
                    fileID = nd.FileID.ToString();
                    if (!string.IsNullOrEmpty(nd.NotificationMode))
                    {
                        if (nd.NotificationMode == notificationMode)
                        {
                            objStorageHelper.WriteNotificationToStorage(tenantId, fileID, nd.FileUploadStatus, nd.NotificationMode, nd.RecipientDetails, IsNotified);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log 
                return false;
            }

        }

        public bool UpdateNotificationStatus(List<NotificationDetails> notificationDetails, bool IsNotified)
        {
            try
            {
                DataAccessHelper objDbHelper = new DataAccessHelper();
                string tenantId;
                string fileID;
                foreach (var nd in notificationDetails)
                {
                    tenantId = nd.TenantID.ToString();
                    fileID = nd.FileID.ToString();
                    if (!string.IsNullOrEmpty(nd.NotificationMode))
                    {
                        objDbHelper.UpdateNotificationStatus(IsNotified, tenantId, fileID);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log 
                return false;
            }
        }

    }

}
