using System.Net;

namespace Bonfire.Domain.Exceptions;

/// <summary>
///     Класс-обертка над базовым исключением, содержащий описание ошибки и ее код.
/// </summary>
public class BaseException : Exception
{
    /// <summary>
    ///     Конструктор базового исключения, содержащего код ошибки и ее описание.
    /// </summary>
    /// <param name="errorCode">Код ошибки.</param>
    /// <param name="message">Описание ошибки.</param>
    public BaseException(HttpStatusCode errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    ///     Код ответа сервера.
    /// </summary>
    public HttpStatusCode ErrorCode { get; }
}