using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IUserNotificationService
    {
        Task<Response<IEnumerable<UserNotification>>> GetUserNotifications(int UserId);

        Task<Response<UserNotification>> UpdateUserNotifications(int NotificationID, UserNotification notification);
    }
}
