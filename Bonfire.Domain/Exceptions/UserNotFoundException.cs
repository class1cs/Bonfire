using System.Net;

namespace Bonfire.Domain.Exceptions;

public class UserNotFoundException: BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public UserNotFoundException() : base(HttpStatusCode.NotFound, HttpErrors.UserNotFound)
    {
    }
}