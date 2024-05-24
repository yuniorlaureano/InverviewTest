namespace InterviewTest.Common
{
    public record JwtOptions(string Secret, string Issuer, string Audience, int? ExpiresIn);

}
