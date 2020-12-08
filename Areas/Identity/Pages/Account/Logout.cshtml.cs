using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using EmployeesApplication.Data;

namespace EmployeesApplication.Areas.Identity.Pages.Account
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "def")]
    public class LogoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LogoutModel(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            HttpContext.Response.Cookies.Delete("AuthCookies");
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            HttpContext.Response.Cookies.Delete("AuthCookies");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
