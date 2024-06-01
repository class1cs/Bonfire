namespace Bonfire.Core.Entities;

public class User
{
    public User(string nickName, string passwordHash, List<DirectChat> directChats)
    {
        NickName = nickName;
        PasswordHash = passwordHash;
        DirectChats = directChats;
    }

    public User()
    {
        
    }

    public Guid Id { get; set; }

    public string NickName { get; set; }

    public string PasswordHash { get; set; }

    public List<DirectChat> DirectChats { get; set; }
}