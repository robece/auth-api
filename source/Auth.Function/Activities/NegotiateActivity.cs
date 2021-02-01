using Auth.Function.Helpers;
using Auth.Function.Models.Responses;
using Auth.Function.Providers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Auth.Function.Models
{
    public class NegotiateActivity
    {
        private readonly IJwtProvider _jwtProvider;

        public NegotiateActivity(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        [FunctionName(nameof(Negotiate))]
        public HttpResponseMessage Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "1.0/negotiate")]
            HttpRequestMessage req)
        {
            req.Headers.TryGetValues(HeaderNames.Authorization, out IEnumerable<string> authorizationEnumerable);
            if (authorizationEnumerable == null)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            var authorizationList = authorizationEnumerable.ToList();
            if (authorizationList.Count == 0)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            var (isValidToken, claims) = _jwtProvider.ValidateToken(authorizationList[0], Settings.AuthorizationKey);
            if (!isValidToken)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            claims.TryGetValue("userID", out var userID);
            if (string.IsNullOrEmpty(userID))
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.BadRequest, "Missing parameter: userID");

            return ResponseBuilderHelper.BuildResponse(HttpStatusCode.OK, new NegotiateActivityResponse() { UserID = userID });
        }
    }
}