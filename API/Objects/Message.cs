using System;
using Newtonsoft.Json;

namespace GPT.API.Objects
{
    [Serializable]
    public class Message
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public RoleEnum? role;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public string? name;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public string? content;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public FunctionCall? function_call;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public string? finish_reason;

        internal void Add(Message message)
        {
            if (message.content != null)
            {
                if (content == null) content = "";
                content += message.content;
            }
            if (message.function_call != null)
            {
                if (function_call == null) function_call = new FunctionCall();
                function_call.Add(message.function_call);
            }
        }
    }
    public class MessageConverter : JsonConverter<Message>
    {
        public override void WriteJson(JsonWriter writer, Message value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value.name != null)
            {
                writer.WritePropertyName("name");
                writer.WriteValue(value.name);
            }

            if (value.role.HasValue)
            {
                writer.WritePropertyName("role");
                writer.WriteValue(value.role.Value.ToString().ToLower());
            }

            // if (value.content != null && value.content != "")
            // {
                writer.WritePropertyName("content");
                writer.WriteValue(value.content);
            // }

            if (value.function_call != null && value.function_call.name != "")
            {
                writer.WritePropertyName("function_call");
                writer.WriteStartObject();
                if (value.function_call.name != null)
                {
                    writer.WritePropertyName("name");
                    writer.WriteValue(value.function_call.name);
                }
                if (value.function_call.arguments != null)
                {
                    writer.WritePropertyName("arguments");
                    writer.WriteValue(value.function_call.arguments);
                }
                writer.WriteEndObject();
            }

            if (value.finish_reason != null)
            {
                writer.WritePropertyName("finish_reason");
                writer.WriteValue(value.finish_reason);
            }

            writer.WriteEndObject();
        }

        public override Message ReadJson(JsonReader reader, Type objectType, Message existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //map all the values
            var message = new Message();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value?.ToString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "name":
                            message.name = reader.Value?.ToString()!;
                            break;
                        case "role": // this is a string in the API, but an enum in the object
                            var roleVal = reader.Value?.ToString();
                            if (roleVal != null)
                                message.role = (RoleEnum)Enum.Parse(typeof(RoleEnum), roleVal);
                            break;
                        case "content":
                            message.content = reader.Value?.ToString()!;
                            break;
                        case "function_call":
                            message.function_call = serializer.Deserialize<FunctionCall>(reader)!;
                            break;
                        case "finish_reason":
                            message.finish_reason = reader.Value?.ToString()!;
                            break;
                    }
                }
            }

            return message;
        }
    }
}