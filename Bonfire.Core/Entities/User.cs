namespace Bonfire.Core.Entities;

public class User(Guid id, string nickName,  string passwordHash, List<DirectChat> directChats)
{
    public Guid Id { get; private set; } = id;

    public string NickName { get; private set; } = nickName;

    public string PasswordHash { get; private set; } = passwordHash;

    public List<DirectChat> DirectChats { get; private set; } = directChats;
}