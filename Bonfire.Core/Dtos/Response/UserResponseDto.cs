namespace Bonfire.Core.Dtos.Response;

public class UserResponseDto(Guid id, string nickName)
{
    public Guid Id { get; private set; } = id;

    public string NickName { get; private set; } = nickName;
}