using System.Collections.Generic;

namespace EmployeesApplication.Models
{
    public class ProgrammingLanguage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<EmployeeExperience> EmployeesExperience { get; set; }
    }
}