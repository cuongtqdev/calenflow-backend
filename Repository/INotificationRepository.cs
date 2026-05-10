using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetListNotifications();
        Task<Notification?> GetNotificationById(int id);
        Task<Notification> AddNotification(Notification notification);
        Task<Notification> UpdateNotification(Notification notification);
        Task DeleteNotification(int id);
    }
}
