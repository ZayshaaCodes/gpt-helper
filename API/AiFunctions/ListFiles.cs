using System.Collections.Generic;

namespace GPT.API.AiFunctions
{

    [AutoLink] // this one will just list files, path is relative to the project folder
    public class ListFiles : GptFunction
    {
        public ListFiles()
        {
            name = GetType().Name;
            description = "Lists files. default path is .";
            var paramProps = new Dictionary<string, ParameterProperties>();
            requiresApproval = false;
            paramProps.Add("path", new ParameterProperties()
            {
                type = "string",
                description = "relative or absolute path, optional"
            });
            parameters = new()
            {
                type = "object",
                properties = paramProps
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            var s = "";
            var path = ".";
            if (args.TryGetValue("path", out var argPath))
                path = argPath;

            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                s += file + "\n";
            }

            foreach (var dir in dirs)
            {
                s += dir + "\n";
            }

            //remove the last newline
            s = s.Substring(0, s.Length - 1);

            return s;
        }
    }

}