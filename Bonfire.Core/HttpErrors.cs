namespace Bonfire.Core;

public class HttpErrors
{
    public const string ConversationNotFoundException = "Дружище, этой переписки не существует!";

    public const string AccessToMessageDenied = "Эй! У тебя нет доступа к этому сообщению!";
    
    public const string AccessToConversationDenied = "Эй! У тебя нет доступа к этой переписке!";

    public const string ConversationAlreadyExistsException = "Кажется, ты хочешь создать клона переписки! Но она уже есть!";
    
    public const string MessageNotFound = "Посмотри лучше! Такого сообщения не существует!";
    
    public const string InvalidLoginCredentials = "Проверь, правильные-ли данные ты ввёл, дружище!";
    
    public const string ReceiverEqualsSenderException = "Ты не можешь написать самому себе!";

    public const string NicknameAlreadyExists = "Этот никнейм уже занят, придумай новый!";
    
    public const string MessageCannotBeEmpty = "Текст сообщения не может быть пустым. Напиши что-нибудь!";

    public const string WrongConversationParticipants =
        "Один или несколько пользователей из списка участников беседы не найдены!";
}