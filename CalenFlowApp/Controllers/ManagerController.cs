using System.Dynamic;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObjects.Models;
using CalenFlowApp.Hubs;
using CalenFlowApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service;

namespace CalenFlowApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IInterviewService _interviewService;
        private readonly ICandidateService candidateService;
        private readonly IRescheduleService rescheduleService;
        private readonly IUserService userService;
        private readonly IHiringService hiringService;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationService notificationService;
        public ManagerController(IInterviewService interviewService, ICandidateService candidateService, 
            IRescheduleService rescheduleService, IUserService userService, 
            IHiringService hiringService, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _interviewService = interviewService;
            this.candidateService = candidateService;
            this.rescheduleService = rescheduleService;
            this.userService = userService;
            this.hiringService = hiringService;
            this.hubContext = hubContext;
            this.notificationService = notificationService;
        }
        public string GetUserEmail()
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
            return email;
        }
        public async Task<IActionResult> Dashboard( string email  )
        {
            if (email == null)
            {
                email = GetUserEmail();
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            List<Interview> listInterview = await _interviewService.GetListInterviewByHiringId(uids[0]);
            if (!listInterview.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(listInterview);
                await _interviewService.UpdateStatusInterview(listInterview);
            }
            listInterview = await _interviewService.FilterListInterviewByDay(DateOnly.FromDateTime(DateTime.Now), listInterview);
            return View(listInterview);
        }
        public async Task<IActionResult> Calendar()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1 )
            {
                for ( int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            Hiring hr = await hiringService.GetHiringById(uids[0]);
            List<Interview> listInterview = await _interviewService.GetListInterviewByHiringId(hr.HiringId);
            if ( !listInterview.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(listInterview);
                await _interviewService.UpdateStatusInterview(listInterview);
            }
            return View(listInterview);
        }
        public async Task<IActionResult> Candidate()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            Hiring hr = await hiringService.GetHiringById(uids[0]);
            List<Interview> listInterview = await _interviewService.GetListInterviewByHiringId(hr.HiringId);
            if (!listInterview.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(listInterview);
                await _interviewService.UpdateStatusInterview(listInterview);
            }
            return View(listInterview);
        }
        public async Task<IActionResult> Reschedule()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            List<Reschedule> list = await rescheduleService.GetAll();
            List<Reschedule> listResult = new List<Reschedule>();
            foreach (var item in list)
            {
                if (item.HiringId == uids[0])
                {
                    listResult.Add(item);
                }
            }
            return View(listResult);
        }
        public async Task<IActionResult> Information()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            User hr = await userService.GetUserById(uids[0]);
            return View(hr);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInformationHiring([FromBody] UpdateHiringDto dto)
        {
            Console.WriteLine("dto: " + dto.Firstname + " " + dto.Lastname + " " + dto.Phone + " " + dto.Email + " " + dto.Bio);
            var email = GetUserEmail(); 
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            User hiring = await userService.GetUserById(uids[0]);
            if (hiring == null)
            {
                return Json(new { success = false, message = "Hiring not found" });
            }
            hiring.FirstName = dto.Firstname == "" ? hiring.FirstName : dto.Firstname;
            hiring.LastName = dto.Lastname == "" ? hiring.LastName : dto.Lastname;
            hiring.Phone = dto.Phone == "" ? hiring.Phone : dto.Phone;
            hiring.Email = dto.Email == "" ? hiring.Email : dto.Email;
            hiring.Bio = dto.Bio == "" ? hiring.Bio : dto.Bio;
            await userService.UpdateUser(hiring);
            return Json(new { success = true, message = "Update information successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> InviteCandidate([FromBody] InviteCandidatedto dto)
        {
            List<int> uids = await userService.GetListUserIdByEmail(dto.Email);
            var hiringemail = GetUserEmail();
            List<int> hiringuids = await userService.GetListUserIdByEmail(hiringemail);
            if (hiringuids.Count > 1)
            {
                for (int i = 1; i < hiringuids.Count; i++)
                {
                    await userService.DeleteUser(hiringuids[i]);
                }
            }
            User hiring = await userService.GetUserById(hiringuids[0]);
            if (uids.Count == 0)
            {
                return Json(new { success = false, message = "Candidate not found" });
            }
            foreach (var id in uids)
            {
                User user = await userService.GetUserById(id);
                if (user != null && user.Role == "Candidate" && user.CompanyId == hiring.CompanyId)
                {
                    return Json(new { success = false, message = "Candidate have already in meeting company" });
                }
            }
            User candi = await userService.GetUserById(uids[0]);
            User candiadd = new User();
            candiadd.FirstName = candi.FirstName;
            candiadd.LastName = candi.LastName;
            candiadd.Email = candi.Email;
            candiadd.Phone = candi.Phone;
            candiadd.UserName = candi.UserName;
            candiadd.PasswordHash = candi.PasswordHash;
            candiadd.Role = "Candidate";
            candiadd.Bio = candi.Bio;
            candiadd.CompanyId = hiring.CompanyId;
            await userService.AddUser(candiadd);
            List<User> users = await userService.GetListUserByEmail(dto.Email);
            Candidate candidate = new Candidate();
            foreach (var user in users)
            {
                if ( user.CompanyId == hiring.CompanyId && user.Role == "Candidate")
                {
                    candidate.CandidateId = user.UserId;
                    break;
                }
            }
            Candidate c = await candidateService.GetCandidateById(candi.UserId);
            candidate.UrlCv = c == null ? "" : c.UrlCv;
            await candidateService.AddCandidate(candidate);
            Notification noti = new Notification();
            noti.CandidateId = candidate.CandidateId;
            noti.Message = $"You have a new interview invite from {hiring.Email}";
            noti.Type = "Invite";
            noti.Title = "Invite Interview";
            noti.CreatedAt = DateTime.Now;
            noti.Status = "Sent";
            noti.HiringId = hiring.UserId;
            noti.WhoSend = hiring.UserId;
            await notificationService.AddNotification(noti);
            NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
            notificationSendCandi.HiringId = hiring.UserId;
            notificationSendCandi.Message = noti.Message;
            notificationSendCandi.Type = noti.Type;
            await hubContext.Clients.Group(dto.Email)
                                     .SendAsync("ReceiveNotification", notificationSendCandi);
            return Json(new { success = true, message = "Invite candidate successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCandidateStatus([FromBody] CandidateStatusUpdateDto dto)
        {
            var candidate = await candidateService.GetCandidateById(dto.CandidateId);
            if (candidate == null)
            {
                return Json(new { success = false, message = "Candidate not found" });
            }

            candidate.Status = dto.NewStatus;
            await candidateService.UpdateCandidate(candidate);
            //find hiring
            var hiringemail = GetUserEmail();
            List<int> hiringuids = await userService.GetListUserIdByEmail(hiringemail);
            if (hiringuids.Count > 1)
            {
                for (int i = 1; i < hiringuids.Count; i++)
                {
                    await userService.DeleteUser(hiringuids[i]);
                }
            }
            User hiring = await userService.GetUserById(hiringuids[0]);
            // notification session
            Notification noti = new Notification();
            noti.CandidateId = candidate.CandidateId;
            noti.Title = "Interview Result";
            noti.CreatedAt = DateTime.Now;
            noti.Type = dto.NewStatus;
            noti.Status = "Sent";
            noti.HiringId = hiring.UserId;
            noti.WhoSend = hiring.UserId;
            if ( dto.NewStatus == "hired" )
            {
                noti.Message = $"Congratulations! You have been hired for the position from {hiring.Email}";
            } else if ( dto.NewStatus == "rejected")
            {
                noti.Message = $"We regret to inform you that your application has been rejected by {hiring.Email}";
            }
            await notificationService.AddNotification(noti);
            NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
            notificationSendCandi.HiringId = hiring.UserId;
            notificationSendCandi.Message = noti.Message;
            notificationSendCandi.Type = noti.Type;
            await hubContext.Clients.Group(candidate.CandidateNavigation.Email)
                                     .SendAsync("ReceiveNotification", notificationSendCandi);
            return Json(new { success = true, message = $"Candidate {dto.NewStatus} successfully!" });
        }
        [HttpPost]
        public async Task<IActionResult> ResponseRequestMeeting([FromBody] RescheduleResponseDto dto)
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            if (uids.Count > 1)
            {
                for (int i = 1; i < uids.Count; i++)
                {
                    await userService.DeleteUser(uids[i]);
                }
            }
            dto.HiringId = uids[0];
            Console.Write("candidate id : " + dto.CandidateId + " hiring id : " + dto.HiringId + " is accepted : " + dto.IsAccepted + " new date : " + dto.NewDate);
            Reschedule reschedule = await rescheduleService.GetById(dto.CandidateId, dto.HiringId);
            if (reschedule == null)
            {
                return Json(new { success = false, message = "Reschedule request not found" });
            }
            Notification noti = new Notification();
            noti.CandidateId = dto.CandidateId;
            noti.CreatedAt = DateTime.Now;
            noti.Status = "Sent";
            noti.HiringId = dto.HiringId;
            noti.WhoSend = dto.HiringId;
            User hiring = await userService.GetUserById(dto.HiringId);
            User candi = await userService.GetUserById(dto.CandidateId);
            if (dto.IsAccepted)
            {
                List<Interview> interviews = await _interviewService.GetListInterviewByHiringId(dto.HiringId);
                var interview = interviews.FirstOrDefault(i => i.CandidateId == dto.CandidateId && i.HiringId == dto.HiringId);
                if (interview != null)
                {
                    interview.Date = DateOnly.FromDateTime(dto.NewDate);
                    interview.Time = TimeOnly.FromDateTime(dto.NewDate);
                    await _interviewService.UpdateInterview(interview);
                }
                reschedule.IsAccept = true;
                await rescheduleService.Update(reschedule);
                noti.Message = $"Your reschedule request with {hiring.Email} was approve";
                noti.Type = "RescheduleAccepted";
                noti.Title = "Reschedule Accepted";
                NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
                notificationSendCandi.HiringId = hiring.UserId;
                notificationSendCandi.Message = noti.Message;
                notificationSendCandi.Type = noti.Type;
                await notificationService.AddNotification(noti);
                await hubContext.Clients.Group(candi.Email)
                         .SendAsync("ReceiveNotification", notificationSendCandi);
                return Json(new { success = true, message = "Reschedule request accepted and interview updated successfully!" });
            }
            else
            {
                await rescheduleService.Delete(dto.CandidateId, dto.HiringId);
                noti.Message = $"Your reschedule request with {hiring.Email} was rejected";
                noti.Type = "RescheduleDeclined";
                noti.Title = "Reschedule Declined";
                NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
                notificationSendCandi.HiringId = hiring.UserId;
                notificationSendCandi.Message = noti.Message;
                notificationSendCandi.Type = noti.Type;
                await notificationService.AddNotification(noti);
                await hubContext.Clients.Group(candi.Email)
                         .SendAsync("ReceiveNotification", notificationSendCandi);
                return Json(new { success = true, message = "Reschedule request declined successfully!" });
            }
        }
    }
}
