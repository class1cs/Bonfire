namespace Bonfire.Domain;

public static class HttpErrors
{
    public const string ConversationNotFoundException = "Conversation not found.";

    public const string AccessToMessageDenied = "Access to this message denied.";

    public const string AccessToConversationDenied = "Access to this conversation denied.";

    public const string ConversationAlreadyExistsException = "Conversation already exists.";

    public const string MessageNotFound = "Message not found.";

    public const string InvalidLoginCredentials = "Invalid login credentials.";

    public const string ReceiverEqualsSenderException = "Cannot message yourself.";

    public const string NicknameAlreadyExists = "Nickname already taken.";

    public const string MessageCannotBeEmpty = "Message cannot be empty.";

    public const string WrongConversationParticipants = "Recipients not found.";

    public const string InvalidRegistrationData = "Username or password required.";
}