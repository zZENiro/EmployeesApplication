using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesApplication.Data;
using EmployeesApplication.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmployeesApplication.Areas.Managing.Pages.Employees
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "def")]
    public class EditModel : PageModel
    {
        private readonly EmployeesApplication.Data.ApplicationDbContext _context;

        public EditModel(EmployeesApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "ID")]
            public int Id { get; set; }

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
                new InputModel() { Age = employee.Age, Name = employee.Name, Surname = employee.Surname, Id = employee.Id };
        }

        public SelectList GendersList { get; set; }
        public SelectList StatesList { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public ICollection<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            if (id == null) return NotFound();

            var employee = await _context.Employees
                                         .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null) return NotFound();

            Input = employee;

            GendersList = new SelectList((await _context.Genders.ToListAsync()).Select(gender => gender.Name));
            StatesList = new SelectList((await _context.States.ToListAsync()).Select(state => state.Name));
            ProgrammingLanguages = await _context.ProgrammingLanguages.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel input, string[] inputs)
        {
            await TimeStamper.TimeStampAsync(_context, HttpContext);

            if (!ModelState.IsValid) return Page();

            var editingEmployee = await _context.Employees
                                                .Include(prop => prop.Gender)
                                                .Include(prop => prop.State)
                                                .Include(prop => prop.EmployeesExperience)
                                                    .ThenInclude(prop => prop.ProgrammingLanguage)
                                                .Where(empl => empl.Id == input.Id).FirstOrDefaultAsync();

            var currentProgrammingLngs = editingEmployee.EmployeesExperience.Select(prop => prop.ProgrammingLanguage).ToList();

            if (ExperienceIsChanged(currentProgrammingLngs.Select(pl => pl.Name).ToList(), inputs))
            {
                var currEmployeeExp = editingEmployee.EmployeesExperience;
                _context.UsersExperience.RemoveRange(currEmployeeExp);
                await _context.SaveChangesAsync();

                var newEmployeeExperience = new List<EmployeeExperience>();

                var languages = await _context.ProgrammingLanguages.ToListAsync();
                languages = languages.Where(lang => inputs.Contains(lang.Name)).ToList();

                foreach (var lang in languages)
                    newEmployeeExperience.Add(new EmployeeExperience()
                    {
                        Employee = editingEmployee,
                        ProgrammingLanguage = lang
                    });

                _context.UsersExperience.UpdateRange(newEmployeeExperience);
                await _context.SaveChangesAsync();
            }

            if (editingEmployee.Gender.Name != input.Gender)
            {
                var updatedGender = await _context.Genders.Where(gender => gender.Name == input.Gender).FirstOrDefaultAsync();
                editingEmployee.Gender = updatedGender;
            }
            if (editingEmployee.State.Name != input.State)
            {
                var updatedState = await _context.States.Where(state => state.Name == input.State).FirstOrDefaultAsync();
                editingEmployee.State = updatedState;
            }

            editingEmployee.Age = input.Age;
            editingEmployee.Name = input.Name;
            editingEmployee.Surname = input.Surname;

            _context.Employees.Update(editingEmployee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }

        private bool ExperienceIsChanged(ICollection<string> current, ICollection<string> newValues) =>
            current.Any(cur => !newValues.Contains(cur));

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
