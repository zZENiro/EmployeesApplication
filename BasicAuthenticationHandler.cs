using EmployeesApplication.Data;
using EmployeesApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EmployeesApplication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;
        private Dictionary<string, Account> _usersCache;
        private PasswordHasher<Account> _passwordHasher;

        public BasicAuthenticationHandler(ApplicationDbContext context,
                                          IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock)
                                          : this(options, logger, encoder, clock)
        {
            _context = context;
        }

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock)
                                          : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Account currUser;
            string login = string.Empty;
            string password = string.Empty;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);

                login = credentials[0];
                password = credentials[1];

                currUser = _context.Accounts.Where(prop => prop.Login == login).FirstOrDefault();
            }
            catch
            {
                return AuthenticateResult.Fail("Error Occured.Authorization failed.");
            }

            if (currUser == null)
                return AuthenticateResult.Fail("Invalid Credentials");

            var result = _passwordHasher.VerifyHashedPassword(currUser, currUser.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
                return AuthenticateResult.Fail("Invalid Credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, currUser.Id.ToString()),
                new Claim(ClaimTypes.Name, currUser.Login),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}