using System;

namespace EmployeesApplication.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public DateTime LastUsage { get; set; }
    }
}
