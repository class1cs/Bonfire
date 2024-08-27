using System.Net;

namespace Bonfire.Core.Exceptions;

public class ConversationAlreadyExistsException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public ConversationAlreadyExistsException() : base(HttpStatusCode.Conflict,
        HttpErrors.ConversationAlreadyExistsException)
    {
    }
}