using BaltaWeb.Models;
using System.Security.Claims;

namespace BaltaWeb.Extensions
{
    public static class RolesClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>()
            {
                new( ClaimTypes.Name, user.Email)
            };

            result.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug)));

            return result;
        }
    }
}




