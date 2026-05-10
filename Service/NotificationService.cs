using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository notificationRepository;
        public NotificationService(INotificationRepository repository)
        {
            notificationRepository = repository;
        }
        public async Task<IEnumerable<Notification>> GetListNotifications()
        {
            return await notificationRepository.GetListNotifications();
        }
        public async Task<Notification?> GetNotificationById(int id)
        {
            return await notificationRepository.GetNotificationById(id);
        }
        public async Task<Notification> AddNotification(Notification notification)
        {
            return await notificationRepository.AddNotification(notification);
        }
        public async Task<Notification> UpdateNotification(Notification notification)
        {
            return await notificationRepository.UpdateNotification(notification);
        }
        public async Task DeleteNotification(int id)
        {
            await notificationRepository.DeleteNotification(id);
        }
    }
}
