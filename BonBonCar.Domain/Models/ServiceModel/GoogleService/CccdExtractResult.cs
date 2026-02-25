namespace BonBonCar.Domain.Models.ServiceModel.GoogleService
{
    public sealed class CccdExtractResult
    {
        public string? DocumentNumber { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? PlaceOfOrigin { get; set; }
        public string? PlaceOfResidence { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
