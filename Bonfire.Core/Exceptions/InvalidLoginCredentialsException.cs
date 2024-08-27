using System.Net;

namespace Bonfire.Core.Exceptions;

public class InvalidLoginCredentialsException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public InvalidLoginCredentialsException() : base(HttpStatusCode.BadRequest, HttpErrors.InvalidLoginCredentials)
    {
    }
}