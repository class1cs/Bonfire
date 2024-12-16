using System.Net;

namespace Bonfire.Domain.Exceptions;

public class AccessToMessageDeniedException : BaseException
{
    /// <summary>
    /// Конструктор исключения, в котором указывается код и текст ошибки.
    /// </summary>
    public AccessToMessageDeniedException() : base(HttpStatusCode.Forbidden, HttpErrors.AccessToMessageDenied)
    {
    }
}