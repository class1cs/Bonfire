namespace Bonfire.Domain.Entities;

public class User
{
    public User(string nickname, string passwordHash, ICollection<Conversation> conversations, ICollection<User> friends, ICollection<FriendRequest> friendRequests)
    {
        Nickname = nickname;
        PasswordHash = passwordHash;
        Conversations = conversations;
        FriendRequests = friendRequests;
        Friends = friends;
    }

    public User()
    {
    }

    public long Id { get; set; }

    public string Nickname { get; set; }

    public string PasswordHash { get; set; }

    public ICollection<Conversation> Conversations { get; set; }
    
    public ICollection<FriendRequest> FriendRequests { get; set; }

    public ICollection<User> Friends { get; set; }
}