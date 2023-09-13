using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GPT.API.Objects
{
    [Serializable]
    public class ChatChoice
    {
        public int? index;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public Message? delta;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public Message? message;
        public string? finish_reason;
    }
}