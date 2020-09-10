using Newtonsoft.Json;

namespace CommonEntity.Business
{
    public class ImageResult
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}