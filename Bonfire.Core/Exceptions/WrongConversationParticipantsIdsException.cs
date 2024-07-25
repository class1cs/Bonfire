using System.Net;

namespace Bonfire.Core.Exceptions;

public class WrongConversationParticipantsIdsException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public WrongConversationParticipantsIdsException() : base(HttpStatusCode.BadRequest, HttpErrors.WrongConversationParticipants)
    {
    }
}