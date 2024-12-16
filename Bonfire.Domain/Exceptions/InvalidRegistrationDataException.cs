using System.Net;

namespace Bonfire.Domain.Exceptions;

public class InvalidRegistrationDataException : BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public InvalidRegistrationDataException() : base(HttpStatusCode.BadRequest, HttpErrors.InvalidRegistrationData)
    {
    }
}