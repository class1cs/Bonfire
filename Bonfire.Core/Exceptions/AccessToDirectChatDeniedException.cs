using System.Net;

namespace Bonfire.Core.Exceptions;

public class AccessToDirectChatDeniedException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public AccessToDirectChatDeniedException() : base(HttpStatusCode.Forbidden, HttpErrors.AccessToDirectChatDenied)
    {
    }
}