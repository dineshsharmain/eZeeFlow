using eZeeFlow.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eZeeFlow.Notification.Gateway
{
    public interface INotificationGateway
    {
        void Notify(List<NotificationDetails> notificationDetails);
    }
}
