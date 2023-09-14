using Newtonsoft.Json;

namespace GPT.API.AiFunctions
{
    [AutoLink]
    public class RenameFiles : GptFunction
    {
        public RenameFiles()
        {
            name = GetType().Name;
            description = "Renames files in a directory, use json to pass in the original and new names";
            var paramProps = new Dictionary<string, ParameterProperties>();
            paramProps.Add("filesList", new ParameterProperties()
            {
                type = "string",
                description = "a list of kvp's in json format, where the key is the original name and the value is the new name"
            });
            parameters = new()
            {
                type = "object",
                properties = paramProps,
                required = new[] { "filesList" }
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            if (args.TryGetValue("filesList", out string pathList))
            {
                var paths = JsonConvert.DeserializeObject<Dictionary<string, string>>(pathList);
                foreach (var item in paths)
                {
                    if (File.Exists(item.Key))
                    {
                        File.Move(item.Key, item.Value);
                    }
                    else if (Directory.Exists(item.Key))
                    {
                        Directory.Move(item.Key, item.Value);
                    }
                }
            }
            return "done";
        }
    }
}