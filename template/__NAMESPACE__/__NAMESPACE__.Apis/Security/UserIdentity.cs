using __NAMESPACE__.Repository.Abstractions.Security;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace __NAMESPACE__.Apis.Security
{
    public class UserIdentity(
        IHttpContextAccessor httpContextAccessor
    ) : IUserIdentity
    {

        public Guid GetSubject()
        {
            var claim = GetClaim(Claims.Subject);
            var parsed = Guid.TryParse(claim?.Replace("\"", string.Empty), out var subject);
            return parsed ? subject : Guid.Empty;
        }

        public string GetUserName()
        {
            var userName = GetClaim(Claims.Subject);
            return userName ?? "default";
        }

        private string GetClaim(string type)
        {
            var principal = httpContextAccessor.HttpContext!.User;
            return principal!.Claims.FirstOrDefault(x => x.Type == type)?.Value!;
        }
    }
}
