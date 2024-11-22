using Web.Core.Entities;

namespace Web.Application.Services.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}