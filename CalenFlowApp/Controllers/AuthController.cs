using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Service;
using BusinessObjects.Models;
using System.Data;
using CalenFlowApp.Models;
using System.Security.Cryptography.Xml;

namespace TestHtml.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService userService;
        private readonly ICandidateService candidateService;
        private readonly IHiringService hiringService;
        public AuthController(IUserService userService, ICandidateService candidateService, IHiringService hiringService)
        {
            this.userService = userService;
            this.candidateService = candidateService;
            this.hiringService = hiringService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignIndto dto )
        {
            List<int> uids = await userService.GetListUserIdByEmail(dto.Email);
            if (uids.Count == 0)
            {
                return Json(new { success = false, message = "username or password is incorrect" });
            }
            User user = await userService.GetUserById(uids[0]);
            if (user == null || user.PasswordHash != dto.Password)
            {
                return Json(new { success = false, message = "username or password is incorrect" });
            }
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, user.Email ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Role)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProps
            );
            // Lưu session nếu muốn
            HttpContext.Session.SetString("UserEmail", user.Email ?? "");
            HttpContext.Session.SetString("UserName", user.Email ?? "");
            HttpContext.Session.SetString("UserRole", user.Role ?? "");
            return Json(new { success = true, message = "Sign in successfully" });
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpdto dto)
        {
            if (dto.Username.IndexOf("@calenflow.vn") == -1)
            {
                return Json(new { success = false, message = "Username must follow by username@calenflow.vn" });
            }
            List<int> uids = await userService.GetListUserIdByEmail(dto.Username);
            if (uids.Count > 0)
            {
                return Json(new { success = false, message = "username already exists" });
            }
            User u = new User
            {
                Email = dto.Username,
                UserName = dto.Username,
                FirstName = dto.Username,
                LastName = "",
                PasswordHash = dto.Password,
                Role = "Candidate",
                CompanyId = 13
            };
            await userService.AddUser(u);
            uids = await userService.GetListUserIdByEmail(dto.Username);
            Candidate c = new Candidate
            {
                CandidateId = uids[0],
                UrlCv = "",
                Status = "",
            };
            await candidateService.AddCandidate(c);
            User user = await userService.GetUserById(uids[0]);
            // Tạo ClaimsIdentity để lưu vào cookie
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, user.Email ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""), 
                    new Claim(ClaimTypes.Role, user.Role)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProps
            );

            // Lưu session nếu muốn
            HttpContext.Session.SetString("UserEmail", user.Email ?? "");
            HttpContext.Session.SetString("UserName", user.Email ?? "hi");
            HttpContext.Session.SetString("UserRole", user.Role ?? "");
            return Json(new { success = true, message = "Sign up successfully" });
        }

        // Login bằng Google
        [HttpGet]
        public IActionResult LoginGoogle(string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            };
            return Challenge(props, "Google");
        }

        // Google callback
        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities.First().Claims.ToList();
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var avatar = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
                List<int> users = await userService.GetListUserIdByEmail(email ?? "");
                if (users.Count == 0)
                {
                    User newUser = new User
                    {
                        Email = email,
                        UserName = name,
                        FirstName = name,
                        LastName = "",
                        PasswordHash = "GoogleLogin",
                        Role = "Candidate",
                        CompanyId = 13
                    };
                    await userService.AddUser(newUser);
                    users = await userService.GetListUserIdByEmail(email ?? "");
                    Candidate candidate = new Candidate
                    {
                        CandidateId = users[0],
                        UrlCv = "",
                        Status = "",
                    };
                    await candidateService.AddCandidate(candidate);
                }
                User u = await userService.GetUserById(users[0]);
                if (u == null)
                {
                    return RedirectToAction("Home", "Guest");
                }
                // Tạo ClaimsIdentity để lưu vào cookie
                var claimsIdentity = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, name ?? ""),
                    new Claim(ClaimTypes.Email, email ?? ""),
                    new Claim("Avatar", avatar ?? ""),
                    new Claim(ClaimTypes.Role, u.Role)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProps = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProps
                );

                // Lưu session nếu muốn
                HttpContext.Session.SetString("UserEmail", email ?? "");
                HttpContext.Session.SetString("UserName", name ?? "");
                HttpContext.Session.SetString("UserRole", u.Role ?? "");
                if (u.Role == "Candidate")
                {
                    return RedirectToAction("Dashboard", "User", new { uids = users });
                } else if (u.Role == "HR")
                {
                    return RedirectToAction("Dashboard", "Manager", new { email = u.Email });
                } else
                {
                    return RedirectToAction("Home", "Guest");
                }
            }

            return RedirectToAction("Login");
        }

        // Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Home", "Guest");
        }

        // AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(); // tạo view thông báo không đủ quyền
        }
    }
}
