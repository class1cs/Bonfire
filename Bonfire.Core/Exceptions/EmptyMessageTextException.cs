using System.Net;

namespace Bonfire.Core.Exceptions;

public class EmptyMessageTextException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public EmptyMessageTextException() : base(HttpStatusCode.BadRequest, HttpErrors.MessageCannotBeEmpty)
    {
    }
}