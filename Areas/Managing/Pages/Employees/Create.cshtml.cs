using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeesApplication.Data;
using EmployeesApplication.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmployeesApplication.Areas.Managing.Pages.Employees
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "def")]
    public class CreateModel : PageModel
    {
        private readonly EmployeesApplication.Data.ApplicationDbContext _context;

        public CreateModel(EmployeesApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Age")]
            public int Age { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "State")]
            public string State { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            public static implicit operator InputModel(Employee employee) =>
                new InputModel() { Age = employee.Age, Name = employee.Name, Surname = employee.Surname };
        }

        public SelectList GendersList { get; set; }
        public SelectList StatesList { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public ICollection<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            GendersList = new SelectList((await _context.Genders.ToListAsync()).Select(gender => gender.Name));
            StatesList = new SelectList((await _context.States.ToListAsync()).Select(state => state.Name));
            ProgrammingLanguages = await _context.ProgrammingLanguages.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel input, string[] inputs)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            if (!ModelState.IsValid) return Page();

            var gender = await _context.Genders.Where(gender => gender.Name == input.Gender).FirstOrDefaultAsync();
            var state = await _context.States.Where(state => state.Name == input.State).FirstOrDefaultAsync();

            var newEmployee = new Employee()
            {
                Age = input.Age,
                Name = input.Name,
                Gender = gender,
                State = state,
                Surname = input.Surname
            };

            if (inputs != null && inputs.Length > 0)
            {
                var languages = await _context.ProgrammingLanguages.Where(lang => inputs.Contains(lang.Name)).ToListAsync();
                var employeeExperience = new List<EmployeeExperience>();

                foreach (var lang in languages)
                    employeeExperience.Add(new EmployeeExperience()
                    {
                        ProgrammingLanguage = lang,
                        Employee = newEmployee
                    });

                await _context.AddAsync<Employee>(newEmployee);
                await _context.AddRangeAsync(employeeExperience);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            await _context.AddRangeAsync(newEmployee);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}

// time stamps
// db seed
