using System.Net;

namespace Bonfire.Domain.Exceptions;

public class AlreadyFriendException : BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public AlreadyFriendException() : base(HttpStatusCode.Conflict, HttpErrors.AlreadyFriend)
    {
    }
}