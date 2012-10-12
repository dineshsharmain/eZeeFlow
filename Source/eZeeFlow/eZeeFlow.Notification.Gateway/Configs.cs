using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace eZeeFlow.Notification.Gateway
{
    public static class Configs
    {
        //Email
        public static string SmtpUrl = ConfigurationManager.AppSettings["SmtpUrl"];
        public static int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        public static string SmtpUserName = ConfigurationManager.AppSettings["SmtpUserName"];
        public static string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        public static string SmtpFrom = ConfigurationManager.AppSettings["MailFrom"];

        //SMS
        public static string twilioAccountSid = "";
        public static string twilioAuthToken = "";
        public static string twilioFromNumber = "";

    }
}
