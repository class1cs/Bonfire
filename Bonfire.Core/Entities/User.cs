namespace Bonfire.Core.Entities;

public class User
{
    public User(string nickname, string passwordHash, ICollection<Conversation> conversations)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        Conversations = conversations;
    }
    
    public User()
    {
        
    }
    
    public long Id { get; set; }
    
    public string Nickname { get; set; }
    
    public string PasswordHash { get; set; }

    public ICollection<Conversation> Conversations { get; set; }
}