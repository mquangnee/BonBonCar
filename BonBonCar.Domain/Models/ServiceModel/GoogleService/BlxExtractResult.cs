namespace BonBonCar.Domain.Models.ServiceModel.GoogleService
{
    public class BlxExtractResult
    {
        public string? LicenseNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? LicenseClass { get; set; }
        public string? ExpiryRaw { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
