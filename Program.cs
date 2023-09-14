using System.Reflection;
using GPT.API;
using GPT.API.Objects;
using Newtonsoft.Json.Linq;

namespace GptHelper
{
    class Program
    {
        static GptClient _client;
        static List<Message> _messages = new();
        static Dictionary<string, GptFunction> _funcs = new();

        static void Main(string[] args)
        {
            GptFunctionInfo.InitGptFunctionInfo();

            //load key and org from env vars
            string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            string org = Environment.GetEnvironmentVariable("OPENAI_ORG_ID");
            if (key == null || org == null)
            {
                Console.WriteLine("please set OPENAI_API_KEY and OPENAI_ORG_ID env vars");
                return;
            }
            _client = new GptClient(key, org);

            //just join all the args into a string



            _funcs = GptFunctionInfo.functions;

            _messages.Add(new Message()
            {
                role = RoleEnum.system,
                content = "you are a clever CLI bot tha works from the current working directory, you can manipulate the files. once given a task my the user, " +
                "do any needed tasks to achieve the goal. for example: if the user asks you to rename a file, 1 list the files and find the full name, 2 rename the file, 3 exit" +
                "dont make up any details about the systems state, use the functions to get info where needed." +
                "you are currently in the directory: " + Environment.CurrentDirectory + "\n"
            });

            var message = new Message()
            {
                role = RoleEnum.user,
                content = string.Join(" ", args).Trim()
            };
            //if message is empty, use a default message for testing
            if (message.content == "")
            {
                // message = "list the functions you have available and try out the rename function to retitle any video files to be in the format of 'title (year)'";
                message.content = PromptUser();
                if (message.content == "" || message.content == null)
                    message.content = "please rename my video files to be in the format of '<title> (<year>)', use an educated guess to get a year";
            }

            _messages.Add(message);

            MessageLoop().Wait();

        }

        private static string? PromptUser()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("?> ");
            var input = Console.ReadLine();
            return input;
        }

        static async Task MessageLoop()
        {
            while (true)
            {
                var c = 0;
                var thing = await _client.DoChatCompletionStreaming(_messages, _funcs.Values.ToList(), (delta) =>
                {
                    if (delta.function_call == null && delta.content != null && delta.content != "")
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(delta.content);
                        c++;
                        //if content contains a newline, reset c
                        if (delta.content.Contains("\n")) c = 0;
                    }
                    else if (delta.function_call != null)
                    {
                        if (!String.IsNullOrWhiteSpace(delta.function_call.name))
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("function call: " + delta.function_call.name);
                        }

                        if (delta.function_call.arguments != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(delta.function_call.arguments);
                            c++;
                        }
                    }

                    if (c > 20)
                    {
                        Console.WriteLine();
                        c = 0;
                    }

                });

                _messages.Add(thing);

                var hasFunction = thing.function_call != null;

                //if the thing has content, print it, else print the function call or finish reason
                if (thing.content != null)
                {
                    // Console.WriteLine("resulting content: \n" + thing.content);
                    _messages.Add(thing);

                    //read input from the user
                    Console.WriteLine();
                    if (!hasFunction)
                    {
                        var input = PromptUser();
                        if (input == "exit") break;
                        if (input == ">") continue;
                        _messages.Add(new Message()
                        {
                            role = RoleEnum.user,
                            content = input
                        });
                    }
                }
                if (hasFunction)
                {
                    var name = thing.function_call.name;
                    var args = thing.function_call.arguments;
                    //if no name, do nothing
                    if (name == null) continue;

                    JObject? jArgs = null;
                    if (args != null) jArgs = JObject.Parse(args);


                    if (!GptFunctionInfo.functions.ContainsKey(name))
                    {
                        Console.WriteLine("function not found");
                        continue;
                    }
                    GptFunction func = GptFunctionInfo.functions[name];

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (func.requiresApproval)
                    {
                        Console.WriteLine("press enter to continue, ctrl+c to cancel");
                        Console.ReadLine();
                    }

                    var argDict = jArgs?.ToObject<Dictionary<string, string>>();

                    var result = func.Invoke(argDict);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(result);

                    if (result == "[DONE]")
                    {
                        Console.WriteLine("done");
                        break;
                    }

                    _messages.Add(new Message()
                    {
                        role = RoleEnum.function,
                        name = name,
                        content = result
                    });
                }

            }
        }
    }
}