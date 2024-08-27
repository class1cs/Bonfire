using System.Net;

namespace Bonfire.Core.Exceptions;

public class ConversationNotFoundException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public ConversationNotFoundException() : base(HttpStatusCode.NotFound, HttpErrors.ConversationNotFoundException)
    {
    }
}