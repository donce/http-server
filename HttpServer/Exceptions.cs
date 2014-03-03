using System;

namespace HttpServer
{
    [Serializable]
    public class MethodException : Exception
    {
        public string Parameter { get; private set; }

        public MethodException()
        {
            //empty
        }

        public MethodException(string message)
        {
            //empty
        }

        public MethodException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }

    [Serializable]
    public class ProtocolException : Exception
    {
        public string Parameter { get; private set; }

        public ProtocolException()
        {
            //empty
        }

        public ProtocolException(string message)
        {
            //empty
        }

        public ProtocolException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }

    [Serializable]
    public class FilenameException : Exception
    {
        public string Parameter { get; private set; }

        public FilenameException()
        {
            //empty
        }

        public FilenameException(string message)
        {
            //empty
        }

        public FilenameException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }
}
