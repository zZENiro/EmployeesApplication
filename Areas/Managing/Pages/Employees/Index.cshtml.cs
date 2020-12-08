using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmployeesApplication.Data;
using EmployeesApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmployeesApplication.Areas.Managing.Pages.Employees
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "def")]
    public class IndexModel : PageModel
    {
        private readonly EmployeesApplication.Data.ApplicationDbContext _context;

        public IndexModel(EmployeesApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get;set; }

        public async Task OnGetAsync()
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            Employees = await _context.Employees.ToListAsync();
        }
    }
}
