namespace Bonfire.Core.Entities;

public class User(Guid id, string nickName,  string passwordHash, List<DirectChat> directChats)
{
    public Guid Id { get; set; } = id;

    public string NickName { get; set; } = nickName;

    public string PasswordHash { get; set; } = passwordHash;

    public List<DirectChat> DirectChats { get; set; } = directChats;
}