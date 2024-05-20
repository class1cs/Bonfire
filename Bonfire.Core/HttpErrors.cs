namespace Bonfire.Core;

public static class HttpErrors
{
    public static string DirectChatNotFound => "Дружище, этой переписки не существует!";

    public static string PermissionToEditDenied => "Эй! Это не твое сообщение!";

    public static string DirectChatAlreadyExists => "Кажется, ты хочешь создать клона переписки! Но она уже есть!";
    
    public static string MessageNotFound => "Посмотри лучше! Это не твоё сообщение!";
}