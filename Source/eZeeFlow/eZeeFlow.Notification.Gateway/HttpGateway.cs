using eZeeFlow.Common;
using eZeeFlow.Common.Models;
using eZeeFlow.Logging;
using eZeeFlow.Notification.Gateway.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using eZeeFlow.Common.Enums;

namespace eZeeFlow.Notification.Gateway
{
    public class HttpGateway : INotificationGateway
    {
        //To DO: Will fetch the URL from the DB based on the user input from the user's notification preference page. 
        private const string BaseAPIUri = @"http://eZeeFlownotificationtest.cloudapp.net/api/";
        //private const string BaseAPIUri = @"http://localhost:82/api/";
        private static string PostFileUploadedSuccessUri = BaseAPIUri + @"Notification/InitiateHTTPRequest";

        public HttpGateway()
        {
            //TO DO: Properties to initialize
        }

        public void Notify(List<Common.Models.NotificationDetails> notificationDetails)
        {
            eZeeFlowTraceListener traceListener = new eZeeFlowTraceListener();
            DataStorageHelper objStorageHelper = new DataStorageHelper();
            try
            {
                foreach (var nd in notificationDetails)
                {
                    if (!string.IsNullOrEmpty(nd.NotificationMode))
                    {
                        if (nd.NotificationMode == eZeeFlow.Notification.Gateway.Constants.HTTPRequest)
                        {
                            nd.RecipientDetails = PostFileUploadedSuccessUri;
                            var tenantId = nd.TenantID.ToString();

                            ProcessedFileDetail objFileDetail = new ProcessedFileDetail();
                            objFileDetail.FileId = nd.FileID.ToString();
                            objFileDetail.FileUrl = nd.FileURI;

                            HttpWebRequest httpWebRequest = WebRequest.Create(PostFileUploadedSuccessUri) as HttpWebRequest;
                            httpWebRequest.ContentType = "text/json";
                            httpWebRequest.Method = "POST";
                            string serializedObj = JsonConvert.SerializeObject(objFileDetail).Replace(@"\", "-");

                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(serializedObj);
                            }

                            string response = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()).ReadToEnd();

                            traceListener.WriteLog(Categories.Info, "Eventing - HTTP Notification", response, string.Empty, tenantId);

                        }
                    }
                }

                GatewayFactory objGF = new GatewayFactory();
                objGF.WriteNotificationToStorage(notificationDetails, true, Constants.HTTPRequest);

                //// TODO: Though we send out multiple notifications, there is a single column for the status -- db structure needs to be updated to support granular tracking.
                var isNotificationUpdated = objGF.UpdateNotificationStatus(notificationDetails, true);

            }
            catch (Exception ex)
            {
                traceListener.WriteLog(Categories.Error, "Eventing", ex.Message.ToString(), ex.StackTrace.ToString());
            }

        }
    }

}
