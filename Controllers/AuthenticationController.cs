using AuthenticationWebApi.Services;
using AuthorApi.Services;
using CommonSpace.DatabaseEntity;
using CommonSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IConfiguration _configuration;
        private readonly IAuthorServices _authorServices;
        public AuthenticationController(IConfiguration configuration, IAuthenticationServices authenticationServices, IAuthorServices authorServices)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _authenticationServices = authenticationServices;//?? throw new ArgumentNullException(nameof(authenticationServices));
            _authorServices = authorServices;
        }

        [HttpPost]
        public ActionResult <string> Validate(Creds usercreds)
        {
            try
            {
                if (_authorServices.Signin(usercreds) != null)
                {
                    var Token = _authenticationServices.BuildToken(_configuration["Jwt:Key"],
                                                    _configuration["Jwt:Issuer"],
                                                    new[]
                                                    {
                                             //     _configuration["Jwt:AudienceGateway"],
                                                  _configuration["Jwt:Audience"],
                                                        //    _configuration["Jwt:Audiencebook"]
                                                    },
                                                    usercreds.UserName
                                                    );
                    return Ok(new
                    {
                        Token = Token,
                        IsAuthenticated = true,
                    });
                }
                return Ok(new
                {
                    Token = string.Empty,
                    IsAuthenticated = false
                });

            }

            catch (Exception ex)
            {
                return Unauthorized();
            }

        }

    }
}
