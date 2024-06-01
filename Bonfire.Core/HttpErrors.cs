namespace Bonfire.Core;

public class HttpErrors
{
    public const string DirectChatNotFound = "Дружище, этой переписки не существует!";

    public const string AccessToMessageDenied = "Эй! У тебя нет доступа к этому сообщению!";
    
    public const string AccessToDirectChatDenied = "Эй! У тебя нет доступа к этой переписке!";

    public const string DirectChatAlreadyExists = "Кажется, ты хочешь создать клона переписки! Но она уже есть!";
    
    public const string MessageNotFound = "Посмотри лучше! Такого сообщения не существует!";
    
    public const string InvalidLoginCredentials = "Проверь, правильные-ли данные ты ввёл, дружище!";
    
    public const string ReceiverEqualsSenderException = "Ты не можешь написать самому себе!";
}