using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Service;
using BusinessObjects.Models;
using CalenFlowApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace CalenFlowApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;
        public NotificationController(INotificationService notificationService, IUserService userService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserEmail()
        {
            var email = "";
            if (User.Identity.IsAuthenticated)
            {
                email = User.FindFirstValue(ClaimTypes.Email);
            }
            if (email == "")
            {
                email = HttpContext.Session.GetString("UserEmail");
            }
            var rs = new { email = email };
            return Json(rs);
        } 
        [HttpPost]
        public async Task<IActionResult> GetUserNotification([FromBody] NotificationEmaildto emaildto)
        {
            string email = emaildto.Email;
            if (email == null || email == "")
            {
                return Json(new { status = "error", message = "Email is required" });
            }
            List<int> users = await userService.GetListUserIdByEmail(email);
            if (users.Count == 0)
            {
                return Json(new { status = "error", message = "User not found" });
            }
            IEnumerable<Notification> notifications = await notificationService.GetListNotifications();
            List<Notification> rs = new List<Notification>();
            foreach (var notification in notifications)
            {
                if ( users.Contains((notification.CandidateId).Value) && notification.WhoSend != notification.CandidateId)
                {
                    rs.Add(notification);
                }
            }
            List<NotificationSenddto> dto = new List<NotificationSenddto>();
            dto = rs.Select(n => new NotificationSenddto
            {
                Id = n.NotificationId,
                HiringName = n.Hiring.HiringNavigation.Email,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status == "Viewed",
                CreatedAt = n.CreatedAt,
                CompanyName = n.Hiring.HiringNavigation.Company.Name
            }).ToList();
            return Json(new { status = "success", data = dto });
        }
        [HttpPost]
        public async Task<IActionResult> GetNewNotification([FromBody] NotificationEmaildto emaildto)
        {
            string email = emaildto.Email;
            if (email == null || email == "")
            {
                return Json(new { status = "error", message = "Email is required" });
            }
            List<int> users = await userService.GetListUserIdByEmail(email);
            if (users.Count == 0)
            {
                return Json(new { status = "error", message = "User not found" });
            }
            IEnumerable<Notification> notifications = await notificationService.GetListNotifications();
            Notification rs = new Notification();
            foreach (var notification in notifications)
            {
                if (users.Contains((notification.CandidateId).Value) && notification.HiringId == emaildto.HiringId && notification.Type == emaildto.Type)
                {
                    rs = notification;
                    break;
                }
            }
            NotificationSenddto dto = new NotificationSenddto();
            dto.Id = rs.NotificationId;
            dto.HiringName = rs.Hiring.HiringNavigation.Email;
            dto.Title = rs.Title;
            dto.Message = rs.Message;
            dto.Type = rs.Type;
            dto.Status = rs.Status == "Viewed";
            dto.CreatedAt = rs.CreatedAt;
            dto.CompanyName = rs.Hiring.HiringNavigation.Company.Name;
            return Json(new { status = "success", data = dto });
        }
        [HttpPost]
        public async Task<IActionResult> GetHRNotification([FromBody] NotificationEmaildto emaildto)
        {
            string email = emaildto.Email;
            if (email == null || email == "")
            {
                return Json(new { status = "error", message = "Email is required" });
            }
            List<int> users = await userService.GetListUserIdByEmail(email);
            if (users.Count == 0)
            {
                return Json(new { status = "error", message = "User not found" });
            }
            IEnumerable<Notification> notifications = await notificationService.GetListNotifications();
            List<Notification> rs = new List<Notification>();
            foreach (var notification in notifications)
            {
                if (users.Contains((notification.HiringId).Value) && notification.WhoSend != notification.HiringId)
                {
                    rs.Add(notification);
                }
            }
            List<NotificationSendManagerdto> dto = new List<NotificationSendManagerdto>();
            dto = rs.Select(n => new NotificationSendManagerdto
            {
                Id = n.NotificationId,
                CandidateName = n.Candidate.CandidateNavigation.FirstName + " " + n.Candidate.CandidateNavigation.LastName,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status == "Viewed",
                CreatedAt = n.CreatedAt,
                Email = n.Candidate.CandidateNavigation.Email
            }).ToList();
            return Json(new { status = "success", data = dto });
        }
        [HttpPost]
        public async Task<IActionResult> GetNewNotificationManager([FromBody] NotificationEmailManagerdto emaildto)
        {
            string email = emaildto.Email;
            if (email == null || email == "")
            {
                return Json(new { status = "error", message = "Email is required" });
            }
            List<int> users = await userService.GetListUserIdByEmail(email);
            if (users.Count == 0)
            {
                return Json(new { status = "error", message = "User not found" });
            }
            IEnumerable<Notification> notifications = await notificationService.GetListNotifications();
            Notification rs = new Notification();
            foreach (var notification in notifications)
            {
                if (users.Contains((notification.HiringId).Value) && notification.CandidateId == emaildto.CandidateId && notification.Type == emaildto.Type)
                {
                    rs = notification;
                    break;
                }
            }
            NotificationSendManagerdto dto = new NotificationSendManagerdto();
            dto.Id = rs.NotificationId;
            dto.CandidateName = rs.Candidate.CandidateNavigation.FirstName + " " + rs.Candidate.CandidateNavigation.LastName;
            dto.Title = rs.Title;
            dto.Message = rs.Message;
            dto.Type = rs.Type;
            dto.Status = rs.Status == "Viewed";
            dto.CreatedAt = rs.CreatedAt;
            dto.Email = rs.Candidate.CandidateNavigation.Email;
            return Json(new { status = "success", data = dto });
        }
        [HttpGet]
        public async Task<IActionResult> MarkAsRead( int id )
        {
            Notification? notification = await notificationService.GetNotificationById(id);
            if (notification == null)
            {
                return Json(new { status = "error", message = "Notification not found" });
            }
            notification.Status = "Viewed";
            await notificationService.UpdateNotification(notification);
            return Json(new { status = "success", message = "Notification marked as read" });
        }
    }
}
