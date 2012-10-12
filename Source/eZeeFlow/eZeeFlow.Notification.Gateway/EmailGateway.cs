using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Net.Mime;
using DataHub.Logging;
using DataHub.Common;
using System.Collections.Generic;
using DataHub.Common.Models;
using DataHub.Notification.Gateway.Helper;
using System.Text;
using DataHub.Common.Enums;

namespace DataHub.Notification.Gateway
{
    public class EmailGateway : INotificationGateway
    {
        #region "Private members"

        private string _to = string.Empty;
        private string _cc = String.Empty;
        private string _bcc = String.Empty;
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private readonly string _smtpGatewayUrl = string.Empty;
        private readonly int _smtpGatewayPort;
        private readonly string _smtpUserName = string.Empty;
        private readonly string _smtpPassword = string.Empty;
        private readonly string _smtpMailFrom = string.Empty;
        private string _messageToLog = string.Empty;
        private readonly Collection<string> _attachments = new Collection<string>();

        #endregion

        #region "Public members"

        /// <summary>
        /// In the format Display Name &lt;Email Id&gt,Display Name &lt;Email Id&gt;
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// In the format Display Name &lt;Email Id&gt,Display Name &lt;Email Id&gt;
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// In the format Display Name &lt;Email Id&gt,Display Name &lt;Email Id&gt;
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Mail subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// List of attachment paths. 
        /// </summary>
        public Collection<string> Attachments
        {
            get { return _attachments; }
        }
        //TODO: Placeholder if in future we store content of emails as a files
        /// <summary>
        /// Not implemented. For future use
        /// </summary>
        public string BodyContentPath { get; set; }

        #endregion

        #region "Public methods"

        /// <summary>
        /// 
        /// </summary>
        public EmailGateway()
        {
            _smtpGatewayUrl = Configs.SmtpUrl;
            _smtpGatewayPort = Configs.SmtpPort;
            _smtpUserName = Configs.SmtpUserName;
            _smtpPassword = Configs.SmtpPassword;
            _smtpMailFrom = Configs.SmtpFrom;
        }

        /// <summary>
        /// Sends a mail and logs the specified message when the send operation is complete
        /// </summary>
        /// <param name="messageToLog">Message to log</param>
        public void NotifyAndLog(string messageToLog, List<NotificationDetails> notificationDetails)
        {
            _messageToLog = messageToLog;
            Notify(notificationDetails);
        }

        /// <summary>
        /// Sends a mail
        /// </summary>
        public void Notify(List<NotificationDetails> notificationDetails)
        {
            DataHubTraceListener traceListener = new DataHubTraceListener();
            try
            {
                var mail = new MailMessage();
                var subject = string.Empty;
                var body = string.Empty;
                if (!string.IsNullOrEmpty(_smtpMailFrom)) mail.From = new MailAddress(_smtpMailFrom);
                foreach (var notificationDetail in notificationDetails)
                {
                    if (!string.IsNullOrEmpty(notificationDetail.NotificationMode))
                    {
                        if (notificationDetail.NotificationMode == DataHub.Notification.Gateway.Constants.EMail)
                        {
                            mail.To.Add(new MailAddress(notificationDetail.RecipientDetails));
                        }
                    }
                }

                mail.Subject = GetSubjectForMail(notificationDetails);
                mail.Body = CreateMailBody(notificationDetails);

                var client = new SmtpClient(_smtpGatewayUrl, _smtpGatewayPort);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                var credential = new System.Net.NetworkCredential(_smtpUserName, _smtpPassword);
                client.Credentials = credential;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                GatewayFactory objGF = new GatewayFactory();
                objGF.WriteNotificationToStorage(notificationDetails, true, Constants.EMail);

                //// TODO: Though we send out multiple notifications, there is a single column for the status -- db structure needs to be updated to support granular tracking.

                var isNotificationUpdated = objGF.UpdateNotificationStatus(notificationDetails, true);


                if (!string.IsNullOrEmpty(_messageToLog))
                {
                    traceListener.WriteLog(Categories.Error, "Eventing", "[Notification] Message is empty.");
                }
                mail.Dispose();
            }
            catch (Exception ex)
            {
                traceListener.WriteLog(Categories.Error, "Eventing", ex.Message.ToString(), ex.StackTrace.ToString());
            }
        }
        #endregion

        #region "Private methods"
    
        private string GetSubjectForMail(List<NotificationDetails> notificationDetails)
        {
            //TO DO: Need to fetch from table based on the notification type.
            var subject = Common.Constants.MailNotificationSubject;
            return subject;
        }

        private string CreateMailBody(List<NotificationDetails> notificationDetails)
        {
            //TO DO: need to create a table for message body template and fetch according to the notification type
            try
            {
                StringBuilder messageBody = new StringBuilder();

                messageBody = messageBody.Append(" -File Name : " + notificationDetails[0].FileName + Environment.NewLine);
                messageBody = messageBody.Append(" -FileSize : " + notificationDetails[0].FileSizeInKB + " KB " + Environment.NewLine);
                messageBody = messageBody.Append(" -File Upload Status : " + notificationDetails[0].FileUploadStatus + Environment.NewLine);

                return messageBody.ToString();
            }
            catch (Exception ex)
            {
                //ToDo: Log 
                return null;
            }
        }
              
        private static MailAddress GetMailAddress(string mailAddress)
        {
            var address = mailAddress.IndexOf('<') > 0
                                      ? new MailAddress(mailAddress.Split('<')[1].Replace('>', ' '), mailAddress.Split('<')[0])
                                      : new MailAddress(mailAddress);

            return address;
        }

        private static void SendCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            //Logger.Info((string) e.UserState);
        }

        #endregion
    }
}
