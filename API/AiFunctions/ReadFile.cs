namespace GPT.API.AiFunctions
{
    [AutoLink] // this one will create a folder, path is relative to the project folder
    public class ReadFile : GptFunction
    {
        public ReadFile()
        {
            name        = GetType().Name;
            description = "Reads a file";
            requiresApproval = false;
            var paramProps = new Dictionary<string, ParameterProperties>
            {
                {
                    "path",
                    new()
                    {
                        type = "string",
                        description = "The path to the file to read"
                    }
                }
            };

            parameters = new ParameterInfo()
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
                return System.IO.File.ReadAllText(path);
            }

            return "";
        }
    }

}