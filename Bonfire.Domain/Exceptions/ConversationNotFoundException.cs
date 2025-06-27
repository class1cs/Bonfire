using System.Net;

namespace Bonfire.Domain.Exceptions;

public class ConversationNotFoundException : BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public ConversationNotFoundException() : base(HttpStatusCode.NotFound, HttpErrors.ConversationNotFound)
    {
    }
}