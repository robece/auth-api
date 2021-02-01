using Newtonsoft.Json;

namespace Auth.Function.Models.Responses
{
    public class MessageResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}