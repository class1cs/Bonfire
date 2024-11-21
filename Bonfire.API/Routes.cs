namespace Bonfire.API;

public static class Routes
{
    
    public static class Users 
    {
        
        public const string SearchUsers = "api/users/{searchRequest}";
        
        public const string GetCurrentUserInfo = "api/users";
    }
    
    public static class Auth 
    {
        
        public const string Login = "api/auth/login";
        
        public const string Register = "api/auth/register";
    }
    
    public static class Chat 
    {
        public const string SendMessage = "api/chat/conversation/{conversationId:long}/messages";
        
        public const string GetMessages = "api/chat/conversation/{conversationId:long}/messages";

        public const string EditMessage = "api/chat/conversation/{conversationId:long}/messages/{messageId:long}";
        
        public const string RemoveMessage = "api/chat/conversation/{conversationId:long}/messages/{messageId:long}";

        public const string CreateConversation = "api/chat/conversation";
        
        public const string GetConversations = "api/chat/conversation";
    }
}