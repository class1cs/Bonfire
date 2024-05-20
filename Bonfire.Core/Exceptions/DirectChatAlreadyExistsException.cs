using System.Net;

namespace Bonfire.Core.Exceptions;

public class DirectChatAlreadyExistsException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public DirectChatAlreadyExistsException() : base(HttpStatusCode.Conflict, HttpErrors.DirectChatAlreadyExists)
    {
    }
}
