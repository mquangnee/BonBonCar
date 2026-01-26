namespace BonBonCar.Domain.IService
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    }
}
