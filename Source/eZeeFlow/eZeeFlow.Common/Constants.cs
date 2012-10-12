using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZeeFlow.Common
{
    public class Constants
    {
        public static string CertificateIssuerName = "Aditi Technologies Pvt. Ltd.";
        public static string ProcessFileUploadStatusQueueName = "datahubservicequeue";
        public static string notificationStatusTableName = "NotificationDispatchDetails";
        public static int QueueMessageVisibilityTime = 120;
        public static int queuePollingInterval = 1;
        public static int queuePollingMinInterval = 0;
        public static int queuePollingMaxInterval = 10;
        public static int queuePollingIntervalExponent = 2;
        public const string StorageConnectionString = "DataConnectionString";
        public const string LogConnectionString = "LogConnectionString";
        public const string MSIContainerName = "datahubsetup";
        public const string MSIFileName = "datahubsetup";
        public const string NotificationModeSMS = "SMS";
        public const string NotificationModeEMail = "EMail";
        public const string NotificationModeHTTP = "HTTPRequest";
        public const string MailNotificationSubject = "[DataHub] - File upload status notification.";
        public const string ContainerPolicy = "datahubpolicy";
    }
}
