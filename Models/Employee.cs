using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesApplication.Models
{
    public class Employee 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public State State { get; set; }

        public ICollection<EmployeeExperience> EmployeesExperience { get; set; }
    }
}
