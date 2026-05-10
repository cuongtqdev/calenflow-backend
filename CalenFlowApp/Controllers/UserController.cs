using System.Collections;
using System.ComponentModel.Design;
using System.Security.Claims;
using Azure.Core;
using BusinessObjects.Models;
using CalenFlowApp.Hubs;
using CalenFlowApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using Service;

namespace CalenFlowApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IInterviewService interviewService;
        private readonly ICandidateService candidateService;
        private readonly IUserService userService;
        private readonly IHiringService hiringService;
        private readonly IRescheduleService rescheduleService;
        private readonly INotificationService notificationService;
        private readonly IHubContext<NotificationHub> hubContext;

        public UserController(IInterviewService service, ICandidateService candidateService, IUserService userService, 
            IHiringService hiringService, IRescheduleService rescheduleService, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            interviewService = service;
            this.candidateService = candidateService;
            this.userService = userService;
            this.hiringService = hiringService;
            this.rescheduleService = rescheduleService;
            this.notificationService = notificationService;
            this.hubContext = hubContext;
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
        public async Task<IActionResult> Dashboard(List<int> uids)
        {
            if ( uids.Count == 0 )
            {
                var email = GetUserEmail();
                if ( email == null )
                {
                    return Json(new { success = false, message = "Cannot find session email" });
                }
                uids = await userService.GetListUserIdByEmail(email);
            }
            List<Interview> interviews = new List<Interview>();
            DateTime now = DateTime.Now;
            foreach ( int i in uids )
            {
                Interview inter = await interviewService.GetInterviewByCandidateId(i);
                if (inter!= null)
                {
                    DateTime interviewTime = inter.Date.ToDateTime(inter.Time);
                    if (interviewTime >= now)
                    {
                        interviews.Add(inter);
                    }
                }
            }
            if ( !interviews.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(interviews);
                await interviewService.UpdateStatusInterview(interviews);
            }
            return View(interviews);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequestMeeting([FromBody] SendRequestMeetingDto dto)
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            int userId = -1;
            foreach ( int k in uids )
            {
                User u = await userService.GetUserById(k);
                if (u.CompanyId == dto.CompanyId)
                {
                    userId = u.UserId;
                }
            }
            if (userId == -1)
            {
                return Json(new { success = false, message = "You dont have permission to send request meeting to this company" });
            }
            HiringAvailable ha = await hiringService.GetHiringAvailableById(dto.HiringAvailableId);
            Hiring hiring = await hiringService.GetHiringById(ha.HiringId);
            IEnumerable<Interview> i = await interviewService.GetListInterviews();
            foreach( Interview inter in i )
            {
                if (inter.CandidateId == userId && inter.Hiring.HiringNavigation.CompanyId == hiring.HiringNavigation.CompanyId && (inter.Status == "Pending" || inter.Status == "Active"))
                {
                    return Json(new { success = false, message = "You have already sent a request meeting to this company" });
                }
            }
            Interview newRequest = new Interview
            {
                CandidateId = userId,
                HiringId = hiring.HiringId,
                Position = dto.PositionRole,
                Type = dto.MeetingType,
                Date = ha.Date,
                Time = ha.Time,
                Status = "Active",
                LinkMeet = dto.MeetingType == "Google-Meet" ? "https://meet.google.com/hnr-tumc-deb" : "",
                Description = dto.AdditionalMessage
            };
            await interviewService.AddInterview(newRequest);
            await hiringService.DeleteHiringAvailable(ha.HiringAvailableId);
            User candidate = await userService.GetUserById(userId);
            Notification noti = new Notification();
            noti.CandidateId = candidate.UserId;
            noti.Message = $"Candidate {candidate.FirstName} {candidate.LastName} has sent a new meeting request.";
            noti.Type = "NewMeetingRequest";
            noti.Title = "New Meeting Request";
            noti.CreatedAt = DateTime.Now;
            noti.Status = "Sent";
            noti.HiringId = hiring.HiringId;
            noti.WhoSend = candidate.UserId;
            await notificationService.AddNotification(noti);
            NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
            notificationSendCandi.HiringId = candidate.UserId;
            notificationSendCandi.Message = noti.Message;
            notificationSendCandi.Type = noti.Type;
            await hubContext.Clients.Group(hiring.HiringNavigation.Email)
                                     .SendAsync("ReceiveNotification", notificationSendCandi);
            return Json(new { success = true, message = "Send request meeting successfully" });
        }


        public async Task<IActionResult> RequestMeeting()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email);
            List<Interview> interviews = new List<Interview>();
            foreach (int i in uids)
            {
                Interview interview = await interviewService.GetInterviewByCandidateId(i);
                if (interview != null)
                {
                    interviews.Add(interview);
                }
            }
            if (!interviews.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(interviews);
                await interviewService.UpdateStatusInterview(interviews);
            }
            return View();
        }
        public async Task<IActionResult> Reschedule()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email); 
            List<Interview> interviews = new List<Interview>();
            foreach (int i in uids)
            {
                Interview interview = await interviewService.GetInterviewByCandidateId(i);
                if (interview != null)
                {
                    interviews.Add(interview);
                }
            }
            if (!interviews.IsNullOrEmpty())
            {
                await candidateService.UpdateStatusCandidate(interviews);
                await interviewService.UpdateStatusInterview(interviews);
            }
            return View(interviews);
        }
        [HttpPost]
        public async Task<IActionResult> Reschedule([FromBody] RescheduleDto dto)
        {
            if (dto.HiringId == null || dto.NewDate == null || dto.Reason == null)
            {
                return Json(new { success = false, message = "Invalid data" });
            }
            Console.WriteLine("hiring id : " + dto.HiringId + " new date : " + dto.NewDate + " reason : " + dto.Reason);
            Hiring hr = await hiringService.GetHiringById(dto.HiringId);
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email); 
            int userId = -1;
            foreach (int k in uids)
            {
                User u = await userService.GetUserById(k);
                if (u.CompanyId == hr.HiringNavigation.CompanyId)
                {
                    userId = u.UserId;
                }
            }
            if (userId == -1)
            {
                return Json(new { success = false, message = "You dont have permission to reschedule interview to this company" });
            }
            Interview interview = await interviewService.GetInterviewByCandidateId(userId);
            if (interview == null)
            {
                return Json(new { success = false, message = "Interview not found" });
            }
            if (interview.Status != "Active")
            {
                return Json(new { success = false, message = "You can only reschedule active interviews" });
            }
            Reschedule r = await rescheduleService.GetById(userId, dto.HiringId);
            if (r != null)
            {
                return Json(new { success = false, message = "You have already sent a reschedule request to this interview" });
            }
            r = new Reschedule();
            r.CandidateId = userId;
            r.HiringId = dto.HiringId;
            r.RescheduleDate = dto.NewDate;
            r.Reason = dto.Reason;
            r.OrginalDate = interview.Date.ToDateTime(interview.Time);
            r.IsAccept = false;
            await rescheduleService.Add(r);
            User candidate = await userService.GetUserById(userId);
            Notification noti = new Notification();
            noti.CandidateId = candidate.UserId;
            noti.Message = $"Candidate {candidate.FirstName} {candidate.LastName} has requested to reschedule the interview to {dto.NewDate}.";
            noti.Type = "RescheduleRequest";
            noti.Title = "Reschedule Request";
            noti.CreatedAt = DateTime.Now;
            noti.Status = "Sent";
            noti.HiringId = hr.HiringId;
            noti.WhoSend = candidate.UserId;
            await notificationService.AddNotification(noti);
            NotificationSendCandi notificationSendCandi = new NotificationSendCandi();
            notificationSendCandi.HiringId = candidate.UserId;
            notificationSendCandi.Message = noti.Message;
            notificationSendCandi.Type = noti.Type;
            await hubContext.Clients.Group(hr.HiringNavigation.Email)
                                     .SendAsync("ReceiveNotification", notificationSendCandi);
            return Json(new { success = true, message = "Reschedule interview successfully" });
        }

        public async Task<IActionResult> Information()
        {
            var email = GetUserEmail();
            if (email == null)
            {
                return Json(new { success = false, message = "Cannot find session email" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(email); 
            User u = await userService.GetUserById(uids[0]);
            Console.WriteLine("user: " + u.FirstName + " " + u.LastName + " " + u.Phone + " " + u.Email + " " + u.Bio);
            return View(u);
        }
        public IActionResult ScanCV()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInformationCandidate([FromBody] UpdateCandidateDto dto)
        {
            Console.WriteLine("dto: " + dto.Firstname + " " + dto.Lastname + " " + dto.Phone + " " + dto.Email + " " + dto.Bio);
            List<int> uids = await userService.GetListUserIdByUserNameAndPassword("candidate_11", "hash001");
            foreach (int k in uids)
            {
                User candidate = await userService.GetUserById(k);
                if (candidate == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                candidate.FirstName = dto.Firstname == "" ? candidate.FirstName : dto.Firstname;
                candidate.LastName = dto.Lastname == "" ? candidate.LastName : dto.Lastname;
                candidate.Phone = dto.Phone == "" ? candidate.Phone : dto.Phone;
                candidate.Email = dto.Email == "" ? candidate.Email : dto.Email;
                candidate.Bio = dto.Bio == "" ? candidate.Bio : dto.Bio;
                await userService.UpdateUser(candidate);
            }
            return Json(new { success = true, message = "Update information successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadCV(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file uploaded" });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cvCandidateUpload");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var dbFilePath = "/images/cvCandidateUpload/" + uniqueFileName;
            User u = await userService.GetUserById(userId);
            List<int> uids = await userService.GetListUserIdByUserNameAndPassword(u.UserName, u.PasswordHash);
            foreach (int k in uids)
            {
                Candidate c = await candidateService.GetCandidateById(k);
                c.UrlCv = dbFilePath;
                await candidateService.UpdateCandidate(c);
            }
            return Json(new { success = true, filePath = dbFilePath });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCV([FromBody] DeleteCVdto dto)
        {
            User u = await userService.GetUserById(dto.CandidateId);
            List<int> uids = await userService.GetListUserIdByUserNameAndPassword(u.UserName, u.PasswordHash);
            foreach (int k in uids)
            {
                Candidate c = await candidateService.GetCandidateById(k);
                c.UrlCv = "";
                await candidateService.UpdateCandidate(c);
            }
            return Json(new { success = true,message = "Delete CV successfully" });
        }
    }
}
