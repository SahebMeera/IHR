using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public interface INotificationService
    {
        Task<Response<Notification>> SaveNotification(int Id, Notification obj);
    }
}
