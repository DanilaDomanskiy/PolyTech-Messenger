using Web.Core.Entities;

namespace Web.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}