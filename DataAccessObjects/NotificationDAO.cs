using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class NotificationDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public NotificationDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<Notification>> GetListNotifications()
        {
            return await calenFlowContext.Notifications.Include(n => n.Candidate.CandidateNavigation).Include(n => n.Hiring.HiringNavigation.Company).ToListAsync();
        }
        public async Task<Notification?> GetNotificationById(int id)
        {
            return await calenFlowContext.Notifications.FindAsync(id);
        }
        public async Task<Notification> AddNotification(Notification notification)
        {
            calenFlowContext.Notifications.Add(notification);
            await calenFlowContext.SaveChangesAsync();
            return notification;
        }
        public async Task<Notification> UpdateNotification(Notification notification)
        {
            calenFlowContext.Notifications.Update(notification);
            await calenFlowContext.SaveChangesAsync();
            return notification;
        }
        public async Task DeleteNotification(int id)
        {
            var notification = await calenFlowContext.Notifications.FindAsync(id);
            if (notification != null)
            {
                calenFlowContext.Notifications.Remove(notification);
                await calenFlowContext.SaveChangesAsync();
            }
        }
    }
}
