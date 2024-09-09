using System.Net;

namespace Beam.Core.Exceptions;

public abstract class ExceptionBase:Exception
{
    protected HttpStatusCode HttpStatusCode { get; }

    protected ExceptionBase(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }
}