using System.Net;

namespace Bonfire.Domain.Exceptions;

public class AccessToConversationDeniedException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public AccessToConversationDeniedException() : base(HttpStatusCode.Forbidden, HttpErrors.AccessToConversationDenied)
    {
    }
}