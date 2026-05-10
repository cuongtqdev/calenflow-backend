using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO notificationDAO;
        public NotificationRepository(NotificationDAO dao)
        {
            notificationDAO = dao;
        }
        public async Task<IEnumerable<Notification>> GetListNotifications()
        {
            return await notificationDAO.GetListNotifications();
        }
        public async Task<Notification?> GetNotificationById(int id)
        {
            return await notificationDAO.GetNotificationById(id);
        }
        public async Task<Notification> AddNotification(Notification notification)
        {
            return await notificationDAO.AddNotification(notification);
        }
        public async Task<Notification> UpdateNotification(Notification notification)
        {
            return await notificationDAO.UpdateNotification(notification);
        }
        public async Task DeleteNotification(int id)
        {
            await notificationDAO.DeleteNotification(id);
        }
    }
}
