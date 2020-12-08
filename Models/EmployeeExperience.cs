namespace EmployeesApplication.Models
{
    public class EmployeeExperience
    { 
        public int Id { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; }

        public Employee Employee { get; set; }
    }
}