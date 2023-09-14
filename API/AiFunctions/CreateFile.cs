namespace GPT.API.AiFunctions
{
    [AutoLink] // this one will create a file, path is relative to the working dir, content is the content of the file. not to be used for writing more than 100 lines
    public class CreateFile : GptFunction
    {
        public CreateFile()
        {
            name        = GetType().Name;
            description = "Creates a file, if the file already exists it will be overwritten";
            var paramProps = new Dictionary<string, ParameterProperties>
            {
                {
                    "path",
                    new()
                    {
                        type = "string",
                        description = "The path to the file to create"
                    }
                },
                {
                    "content",
                    new()
                    {
                        type = "string",
                        description = "The content of the file to create"
                    }
                }
            };

            parameters = new ParameterInfo()
            {
                type = "object",
                properties = paramProps, 
                required = new[] { "path", "content" }
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            if (args.TryGetValue("path", out string path) && args.TryGetValue("content", out string content))
            {
                File.WriteAllText(path, content);
            }

            return "";
        }
    }

}