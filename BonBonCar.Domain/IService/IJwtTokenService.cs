namespace BonBonCar.Domain.IService
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(Guid userId, string? userName, string email, string role);
    }
}
