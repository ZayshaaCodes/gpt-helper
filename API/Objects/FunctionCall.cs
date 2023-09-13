namespace GPT.API.Objects
{
    [Serializable]
    public class FunctionCall
    {
        public string? name;
        public string? arguments;

        public FunctionCall()
        {
            name = "";
            arguments = "";
        }

        //overide + to add name and args together with another function call
        public void Add(FunctionCall b)
        {
            name += b.name;
            arguments += b.arguments;
        }

    }
}