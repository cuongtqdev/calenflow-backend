using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace CalenFlowApp.Controllers
{
    public class GuestController : Controller
    {
        private readonly IUserService userService;
        private readonly ICandidateService candidateService;
        private readonly IHiringService hiringService;
        public GuestController(IUserService userService, ICandidateService candidateService, IHiringService hiringService)
        {
            this.userService = userService;
            this.candidateService = candidateService;
            this.hiringService = hiringService;
        }
        public IActionResult Feature()
        {
            return View();
        }

        public async  Task<IActionResult> Home()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";
                var Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
                HttpContext.Session.Clear();
                if (role == "HR")
                {
                    return RedirectToAction("Dashboard", "Manager", new { email = Email });
                }
                else
                {
                    List<int> users = await userService.GetListUserIdByEmail(Email ?? "");
                    return RedirectToAction("Dashboard", "User", new { uids = users });
                }
            }
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                var email = HttpContext.Session.GetString("UserEmail") ?? "";
                var role = HttpContext.Session.GetString("UserRole") ?? "";
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (role == "HR")
                {
                    return RedirectToAction("Dashboard", "Manager", new { email = email });
                }
                else
                {
                    List<int> users = await userService.GetListUserIdByEmail(email ?? "");
                    return RedirectToAction("Dashboard", "User", new { uids = users });
                }
            }
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }
    }
}
