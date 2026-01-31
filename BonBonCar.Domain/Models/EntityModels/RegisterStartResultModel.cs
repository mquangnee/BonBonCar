namespace BonBonCar.Domain.Models.EntityModels
{
    public class RegisterStartResultModel
    {
        public Guid RegisterSessionId { get; set; }
        public int ExpiresInSeconds { get; set; }
        public string MaskedEmail { get; set; } = default!;
    }
}
