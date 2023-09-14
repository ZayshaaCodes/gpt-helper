using System.Diagnostics;

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

    [AutoLink]
    public class PythonExec : GptAction
    {
        public PythonExec()
        {
            name        = GetType().Name;
            description = "Executes python code given a path";
            requiresApproval = true;
            var paramProps = new Dictionary<string, ParameterProperties>
            {
                {
                    "path",
                    new ParameterProperties()
                    {
                        type = "string",
                        description = "path of the python file to execute"
                    }
                }
            };
            parameters = new()
            {
                type = "object",
                properties = paramProps,
                required = new[] { "path" }
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            if (args.TryGetValue("path", out string path))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var process = Process.Start(psi);
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                if (error != "")
                    // Debug.LogError(error);
                    Console.WriteLine(error);
                else
                    // Debug.Log(output);
                    Console.WriteLine(output);
                    return output;
            }

            return "";
        }
    }
}