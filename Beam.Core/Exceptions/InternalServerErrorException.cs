using System.Net;

namespace Beam.Core.Exceptions;

public class InternalServerErrorException : ExceptionBase
{
    public InternalServerErrorException(string message) : base(message, HttpStatusCode.InternalServerError)
    {
        
    }
}