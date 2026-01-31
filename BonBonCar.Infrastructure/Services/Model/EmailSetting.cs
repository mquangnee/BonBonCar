namespace BonBonCar.Infrastructure.Services.Model
{
    public class EmailSetting
    {
        public string From { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
