using EmployeesApplication.Data;
using EmployeesApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesApplication
{
    public static class TimeStamper
    {
        public static async Task TimeStampAsync(ApplicationDbContext context, HttpContext httpContext) => await Task.Factory.StartNew(() =>
        {
            var currUser = httpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Name).FirstOrDefault();
            if (currUser == null) return;

            var currAccount = context.Accounts.Where(acc => acc.Login == currUser.Value).FirstOrDefault();

            currAccount.LastUsage = DateTime.Now;

            context.SaveChanges();
        });
    }

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            if (!context.Genders.Any())
                context.AddRange(new List<Gender>()
                {
                    new Gender() { Name = "Male" },
                    new Gender() { Name = "Female" },
                    new Gender() { Name = "Gender fluid" },
                });

            if (!context.States.Any())
                context.AddRange(new List<State>()
                {
                    new State() { Floor = 1, Name = "Frontend" },
                    new State() { Floor = 2, Name = "Backend" },
                    new State() { Floor = 3, Name = "Management" },
                });

            if (!context.ProgrammingLanguages.Any())
                context.AddRange(new List<ProgrammingLanguage>()
                {
                    new ProgrammingLanguage() { Name = "C#" },
                    new ProgrammingLanguage() { Name = "Scala" },
                    new ProgrammingLanguage() { Name = "Swift" },
                    new ProgrammingLanguage() { Name = "Go" }
                });

            context.SaveChanges();
        }
    }
}
