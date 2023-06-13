namespace BookManager.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;

    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;

        public BasicAuthFilter(string realm)
        {
            _realm = realm;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthorized = IsAuthorized(context);
            if (!isAuthorized)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }

        private bool IsAuthorized(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return false;
            }

            // Authorization: Basic lifyhsdakihgaliuhtgiuaeherigtuhertieag
            var isValidBasicAuth = TryGetCredentials(authHeader, out var username, out var password);
            if (!isValidBasicAuth)
            {
                return false;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var storedUsername = configuration.GetValue<string>("BasicAuthentication:Username");
            var storedPassword = configuration.GetValue<string>("BasicAuthentication:Password");

            var isValidCredentials = username == storedUsername && password == storedPassword;
            if (!isValidCredentials)
            {
                return false;
            }

            return true;
        }

        private bool TryGetCredentials(string authorizationHeader, out string username, out string password)
        {
            var authHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);
            var isValidBasicAuthHeader = authHeaderValue
                .Scheme.Equals(AuthenticationSchemes.Basic.ToString(),
                    StringComparison.OrdinalIgnoreCase);

            if (isValidBasicAuthHeader)
            {
                var credentials =
                    Encoding.UTF8
                        .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                        .Split(new[] { ':' }, 2);
                if (credentials.Length == 2)
                {
                    username = credentials[0];
                    password = credentials[1];
                    return true;
                }
            }

            username = string.Empty;
            password = string.Empty;
            return false;
        }
    }
}
