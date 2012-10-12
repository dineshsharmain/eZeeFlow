using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Configuration;
using Twilio;

namespace eZeeFlow.Notification.Gateway
{
    public class SMSGateway:INotificationGateway
    {
        private string to = string.Empty;
        private string body = string.Empty;
        private string accountSid = Configs.twilioAccountSid;
        private string authToken = Configs.twilioAuthToken;
        private string fromNumber = Configs.twilioFromNumber;

        public string To
        {
            get { return to; }
            set { to = StripSpaceAndDash(value); }
        }

        public string Body { get; set; }

        public Collection<string> Attachments { get; set; }

        public string BodyContentPath { get; set; }

        
        private string StripSpaceAndDash(string number)
        {
            return Regex.Replace(number, @"[\(\)\s\-\.\,]*", "");
        }

        public void Notify(System.Collections.Generic.List<Common.Models.NotificationDetails> notificationDetails)
        {
            var twilioClient = new TwilioRestClient(accountSid, authToken);
            twilioClient.SendSmsMessage(fromNumber, to, body);
        }
    }
}
