using Newtonsoft.Json;

namespace Auth.Function.Models.Responses
{
    public class NegotiateActivityResponse
    {
        [JsonProperty("userID")]
        public string UserID { get; set; }
    }
}