using System;

namespace GPT.API.Objects
{
    [Serializable]
    public class Delta
    {
        // public string content;
        public Message content;

        //tostring
        public override string ToString()
        {
            return $"Delta: {content}";

        }
        
    }
    
}