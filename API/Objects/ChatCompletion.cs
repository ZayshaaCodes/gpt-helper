using System;
using System.Collections.Generic;

namespace GPT.API.Objects
{
    [Serializable]
    public class ChatCompletion
    {
        public string          id;
        public string           @object;
        public long             created;
        public string           model;
        public List<ChatChoice> choices;
        public ChatUsage        usage;
    }
}