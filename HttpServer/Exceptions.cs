using System;

namespace HttpServer
{
    /// <summary>
    /// The exception class for illegal or invalid methods
    /// </summary>
    [Serializable]
    public class MethodException : Exception
    {
        public string Parameter { get; private set; }

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public MethodException()
        {
            //empty
        }

        /// <summary>
        /// Constructor with the message parameter
        /// </summary>
        /// <param name="message">Message string</param>
        public MethodException(string message)
        {
            //empty
        }

        /// <summary>
        /// Constructor with the message and the parameter string
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="parameter">Parameter string</param>
        public MethodException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }

    /// <summary>
    /// Exception class for illegal or invalid protocols
    /// </summary>
    [Serializable]
    public class ProtocolException : Exception
    {
        public string Parameter { get; private set; }

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public ProtocolException()
        {
            //empty
        }

        /// <summary>
        /// Constructor with the message parameter
        /// </summary>
        /// <param name="message">Message string</param>
        public ProtocolException(string message)
        {
            //empty
        }

        /// <summary>
        /// Constructor with the message and the parameter string
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="parameter">Parameter string</param>
        public ProtocolException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }
}