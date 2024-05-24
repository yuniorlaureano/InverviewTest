using System.Security.Claims;

namespace InterviewTest.Service.Interfaces
{
    public interface IJwtService
    {
        string GenerateTokenAsync(Claim[] claims);
        Task<bool> ValidateTokenAsync(string token);
    }
}
