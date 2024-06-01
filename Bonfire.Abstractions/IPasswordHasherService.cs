namespace Bonfire.Application.Services;

public interface IPasswordHasherService
{
    public string HashPassword(string password);
}