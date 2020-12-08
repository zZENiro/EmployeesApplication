using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.Extensions.Primitives;
using EmployeesApplication.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmployeesApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Text)]
            public string Login { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {

                var account = await _context.Accounts.Where(prop => prop.Login == Input.Login).FirstOrDefaultAsync();

                if (account == null)
                {
                    ModelState.AddModelError(string.Empty, "Такого пользователя не существует.");
                    return Page();
                }

                var result = new PasswordHasher<EmployeesApplication.Models.Account>().VerifyHashedPassword(
                    user: account,
                    hashedPassword: account.PasswordHash,
                    providedPassword: Input.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "Не правильный пароль");
                    return Page();
                }

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, account.Login),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                return RedirectToPage("/Home/Index");
            }

            return Page();
        }
    }
}
