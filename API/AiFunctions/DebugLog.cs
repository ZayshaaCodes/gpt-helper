namespace GPT.API.AiFunctions
{
    [AutoLink]
    public class DebugLog : GptAction
    {
        public DebugLog()
        {
            name        = GetType().Name;
            description = "Logs a message console";
            requiresApproval = false;
            disabled    = true;
            var paramProps = new Dictionary<string, ParameterProperties>
            {
                {
                    "message",
                    new ParameterProperties()
                    {
                        type = "string",
                        description = "The message to log, can use rich text tags like colors"
                    }
                }
            };
            parameters = new()
            {
                type = "object",
                properties = paramProps,
                required = new[] { "message" }
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            if (args.TryGetValue("message", out string message))
                // Debug.Log($"<color=yellow>{message}</color>");
                Console.WriteLine(message);

            return "";
        }
    }
}