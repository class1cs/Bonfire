using System.Net;

namespace Bonfire.Core.Exceptions;

public class AccessToMessageDeniedException : BaseException
{
    /// <summary>
    ///     Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public AccessToMessageDeniedException() : base(HttpStatusCode.BadRequest, HttpErrors.PermissionToEditDenied)
    {
    }
}
