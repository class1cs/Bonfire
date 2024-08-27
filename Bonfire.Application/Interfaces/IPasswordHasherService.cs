namespace Bonfire.Application.Interfaces;

public interface IPasswordHasherService
{
    public string HashPassword(string password);
}