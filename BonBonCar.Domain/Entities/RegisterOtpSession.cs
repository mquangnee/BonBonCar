namespace BonBonCar.Domain.Entities
{
    public class RegisterOtpSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string OtpHash { get; set; } = default!;
        public DateTime ExpiredAt { get; set; }
        public int FailedCount { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSentAt { get; set; }
    }
}
