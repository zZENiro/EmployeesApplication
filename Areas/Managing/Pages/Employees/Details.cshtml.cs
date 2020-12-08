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
    public class DetailsModel : PageModel
    {
        private readonly EmployeesApplication.Data.ApplicationDbContext _context;

        public DetailsModel(EmployeesApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }

        public ICollection<EmployeeExperience> Experience { get; set; } 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            if (id == null)
            {
                return NotFound();
            }

            Employee = await _context.Employees
                                     .Include(prop => prop.Gender)
                                     .Include(prop => prop.State)
                                     .FirstOrDefaultAsync(m => m.Id == id);

            Experience = await _context.UsersExperience
                                       .Include(prop => prop.ProgrammingLanguage)
                                       .Where(prop => prop.Employee == Employee).ToListAsync();

            if (Employee == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
