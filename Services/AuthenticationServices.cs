using CommonSpace.DatabaseEntity;
using CommonSpace.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationWebApi.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        public LibraryContext libDbContext { get; set; }

        public AuthenticationServices(LibraryContext libdbContext)
        {
            libDbContext = libdbContext;
        }

        public bool validateuser(Creds usersdetails)
        {
            var userObj = libDbContext.DigitalBooksUsers.Where(u => u.UserName == usersdetails.UserName && u.UserPass == usersdetails.UserPass).Count();
            if (userObj > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private TimeSpan ExpiryDuration = new TimeSpan(20, 30, 0);
        public string BuildToken(string key, string issuer, IEnumerable<string> audience, string userName)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim("RoleId","Author")

        };

            claims.AddRange(audience.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256Signature);
            var tokendescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokendescriptor);
        }

    }
}
