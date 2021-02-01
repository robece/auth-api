using Newtonsoft.Json;

namespace Auth.Function.Models.Responses
{
    public class AuthenticateActivityResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}