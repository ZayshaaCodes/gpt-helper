using System.Collections;
using System.Text;
using GPT.API.Objects;
using Newtonsoft.Json;

namespace GPT.API
{
    public class GptClient
    {
        private          string                 _key;
        private          string                 _org;
        private readonly JsonSerializerSettings _settings;
        private readonly OpenAiEndpointProvider _endpoints;

        public GptClient(string apiKey, string org = null)
        {
            _key = apiKey;
            _org = org;

            _settings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            _settings.Converters.Add(new MessageConverter());
            _endpoints          = new("v1");
        }

        public HttpClient GetClient()
        {
            HttpClient client             = new();
            client.BaseAddress = new("https://api.openai.com/");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_key}");
            if (_org != null)
            {
                client.DefaultRequestHeaders.Add("OpenAI-Organization", $"{_org}");
            }
            
            return client;
        }

        public async Task<Message> DoChatCompletionStreaming(List<Message> messages, List<GptFunction> functions, Action<Message> updateCallback)
        {
            var lineLog = new List<string>();
            ChatRequest chatReq = new ChatRequest()
            {
                messages = messages,
                model    = "gpt-4",
                stream   = true
            };
            if (functions != null)
            {
                chatReq.functions = functions;
            }

            string json = JsonConvert.SerializeObject(chatReq, Formatting.Indented, _settings);
            // File.WriteAllText("last_request.json", json);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoints.ChatCompletionCreate()) 
            {
                Content = content
            };

            var httpClient = GetClient();
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            Message totalMessage = new Message()
            {
                role = RoleEnum.assistant,
            };
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                    
                //example: data: {"id":"chatcmpl-7aYRPCnaEGSSnT2MOn0H0n1YeW0LS","object":"chat.completion.chunk","created":1688946555,"model":"gpt-4-0613","choices":[{"index":0,"delta":{"content":"User"},"finish_reason":null}]}
                //try parse the json with newtonsoft
                // need to prune the "data: " from the start of the line
                if (line!.StartsWith("data: "))
                {
                    var pruned = line.Substring(6);
                    lineLog.Add(pruned);

                    //if is says "[DONE]" then we're done and we can break out of the loop
                    if (pruned == "[DONE]")
                    {
                        break;
                    }
    
                    try
                    {
                        ChatCompletion? parsed = JsonConvert.DeserializeObject<ChatCompletion>(pruned, _settings);
    
                        if (parsed != null)
                        {
                            var finished = parsed.choices[0].finish_reason;
                            if (finished == null)
                            {
                                var delta = parsed.choices[0]!.delta!;
                                updateCallback(delta);
                                totalMessage.Add(delta);
                            }
                        }
                        else
                            Console.WriteLine(pruned);
                    }
                    catch (Exception e)
                    {
                        // Debug.Log(pruned + "\n" + e);
                        Console.WriteLine(pruned + "\n" + e);
                        // log all the lines to a file, json
                    }
                    

                }
            }

            httpClient.Dispose();
            //write line log to file
            // File.WriteAllText("last_response.json", String.Join("\n", lineLog));
            return totalMessage;
        }
        
    }
}