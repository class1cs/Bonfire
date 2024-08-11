using Bonfire.Abstractions;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class UserService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext) : IUserService
{
    public async Task<User> GetCurrentUser()
    {
        var currentUserIdString = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == "Id")!.Value;
        var currentUserIdLong = Convert.ToInt64(currentUserIdString);
        var currentUser = await appDbContext.Users.Include(x => x.Conversations).FirstOrDefaultAsync(x => x.Id == currentUserIdLong);
        return currentUser!;
    }
    
    
}