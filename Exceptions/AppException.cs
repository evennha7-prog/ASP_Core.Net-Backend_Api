using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Base application exception from which all domain-specific HTTP exceptions derive.
    /// Encapsulates an appropriate HTTP status code.
    /// </summary>
    public abstract class AppException : Exception
    {
        /// <summary>
        /// HTTP status code associated with this error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="AppException"/>.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Target HTTP status code.</param>
        protected AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AppException"/> with an inner exception.
        /// </summary>
        protected AppException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
