namespace Bonfire.Domain;

public static class HttpErrors
{
    public const string ConversationNotFound = "Conversation not found.";

    public const string AccessToMessageDenied = "Access to this message denied.";

    public const string AccessToConversationDenied = "Access to this conversation denied.";

    public const string ConversationAlreadyExists = "Conversation already exists.";

    public const string MessageNotFound = "Message not found.";
    
    public const string UserNotFound = "User not found.";

    public const string InvalidLoginCredentials = "Invalid login credentials.";

    public const string ReceiverEqualsSender = "Cannot message yourself.";

    public const string NicknameAlreadyExists = "Nickname already taken.";

    public const string MessageCannotBeEmpty = "Message cannot be empty.";

    public const string WrongConversationParticipants = "Recipients not found.";

    public const string InvalidRegistrationData = "Username or password required.";

    public const string AlreadyFriend = "This user is already your friend.";
    
    public const string FriendRequestAlreadySent = "Friend request is already sent to that user.";
}