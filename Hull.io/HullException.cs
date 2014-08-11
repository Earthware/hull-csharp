namespace Hull.io
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    using RestSharp;

    [Serializable]
    public class HullException : Exception
    {
        public HullException()
        {
        }

        public HullException(string message)
            : base(message)
        {
        }

        public HullException(IRestResponse response, string message)
            : base(message)
        {
            this.StatusCode = response.StatusCode;
            this.StatusMessage = response.StatusDescription;
            this.ResponseContent = response.Content;
        }

        public HullException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected HullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string ResponseContent { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusMessage { get; set; }
    }
}