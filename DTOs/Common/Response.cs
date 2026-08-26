using Newtonsoft.Json;

namespace ExamSystem.DTOs.Common
{
    public class Response
    {
        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public object Meta { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public Error Errors { get; set; }

        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public object Links { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonIgnore]
        public bool IsSuccess => Errors == null;

        public static Response Success(object data, object meta = null, object links = null)
        {
            return new Response
            {
                Meta = meta,
                Data = data,
                Links = links
            };
        }

        public static Response Success(string message = null, object meta = null)
        {
            return new Response
            {
                Meta = meta,
                Message = message ?? "The request was processed successfully.",
            };
        }

        public static Response Error(Error error, object meta = null, object links = null)
        {
            return new Response
            {
                Meta = meta,
                Links = links,
                Errors = error
            };
        }
    }

    public class Error
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail", NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }

        [JsonProperty("invalid_params", NullValueHandling = NullValueHandling.Ignore)]
        public List<InvalidParameter> InvalidParams { get; set; }
    }

    public class InvalidParameter
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
