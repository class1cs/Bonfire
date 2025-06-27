using System.Net;

namespace Bonfire.Domain.Exceptions;

public class FriendRequestAlreadySentException : BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public FriendRequestAlreadySentException() : base(HttpStatusCode.Conflict, HttpErrors.FriendRequestAlreadySent)
    {
    }
}