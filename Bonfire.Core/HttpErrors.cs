namespace Bonfire.Core;

public static class HttpErrors
{
    public static string DirectChatNotFound => "Дружище, этой переписки не существует!";

    public static string AccessToMessageDenied => "Эй! У тебя нет доступа к этому сообщению!";
    
    public static string AccessToDirectChatDenied => "Эй! У тебя нет доступа к этой переписке!";

    public static string DirectChatAlreadyExists => "Кажется, ты хочешь создать клона переписки! Но она уже есть!";
    
    public static string MessageNotFound => "Посмотри лучше! Такого сообщения не существует!";
}