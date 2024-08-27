using System.Net;

namespace Bonfire.Core.Exceptions;

public class NicknameAlreadyExistsException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public NicknameAlreadyExistsException() : base(HttpStatusCode.Conflict, HttpErrors.NicknameAlreadyExists)
    {
    }
}