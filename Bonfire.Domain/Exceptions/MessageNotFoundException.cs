using System.Net;

namespace Bonfire.Domain.Exceptions;

public class MessageNotFoundException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public MessageNotFoundException() : base(HttpStatusCode.NotFound, HttpErrors.MessageNotFound)
    {
    }
}