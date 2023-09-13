namespace GPT.API.AiFunctions
{
    [AutoLink]
    public class End : GptFunction
    {
        public End()
        {
            name = GetType().Name;
            description = "Ends the conversation, call once task is complete";
            requiresApproval = false;
            parameters = new()
            {
                type = "object",
                properties = new Dictionary<string, ParameterProperties>()
            };
        }

        public override string Invoke(Dictionary<string, string> args)
        {
            return "[DONE]";
        }
    }

    //[AutoLink] // this one will create a file, path is relative to the project folder, content is the content of the file
    // public class CreateFile : GptFunction
    // {
    //     public CreateFile()
    //     {
    //         name        = GetType().Name;
    //         description = "Creates a file, if the file already exists it will be overwritten";
    //         var paramProps = new Dictionary<string, ParameterProperties>
    //         {
    //             {
    //                 "path",
    //                 new()
    //                 {
    //                     type = "string",
    //                     description = "The path to the file to create"
    //                 }
    //             },
    //             {
    //                 "content",
    //                 new()
    //                 {
    //                     type = "string",
    //                     description = "The content of the file to create"
    //                 }
    //             }
    //         };

    //         parameters = new ParameterInfo()
    //         {
    //             type = "object",
    //             properties = paramProps, 
    //             required = new[] { "path", "content" }
    //         };
    //     }

    //     public override string Invoke(Dictionary<string, string> args)
    //     {
    //         if (args.TryGetValue("path", out string path) && args.TryGetValue("content", out string content))
    //         {
    //             System.IO.File.WriteAllText(path, content);
    //         }

    //         return "";
    //     }
    // }

}