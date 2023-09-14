using System.Diagnostics;

namespace GPT.API.AiFunctions
{
    [AutoLink]
    public class PythonExec : GptAction
    {
        public PythonExec()
        {
            name = GetType().Name;
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

    [AutoLink]
    public class ShellExec : GptAction
    {
        public ShellExec()
        {
            name = GetType().Name;
            description = "Executes a shell command";
            requiresApproval = true;
            var paramProps = new Dictionary<string, ParameterProperties>
            {
                {
                    "command",
                    new ParameterProperties()
                    {
                        type = "string",
                        description = "command to execute"
                    }
                }
            };
            parameters = new()
            {
                type = "object",
                properties = paramProps,
                required = new[] { "command" }
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            if (args.TryGetValue("command", out string command))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
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