using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public int UserID
        {
            get
            {
                return int.Parse(GetClaimValue("id"));
            }
        }

        public string UserName
        {
            get
            {
                return GetClaimValue("username");
            }
        }

        public string Role
        {
            get
            {
                return GetClaimValue("Role");
            }
        }

        public string FullName {
            get
            {
                return GetClaimValue("fullname");
            }
        }

        private string GetClaimValue(string claimType)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
