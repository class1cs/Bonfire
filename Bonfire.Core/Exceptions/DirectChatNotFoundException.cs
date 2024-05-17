using System.Net;

namespace Bonfire.Core.Exceptions;

public class DirectChatNotFoundException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public DirectChatNotFoundException() : base(HttpStatusCode.NotFound, HttpErrors.DirectChatNotFound)
    {
    }
}
