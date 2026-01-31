namespace BonBonCar.Application.Common
{
    public class ErrorResult
    {
        public string? ErrorCode { get; set; }

        public IList<Error> Errors { get; set; }

        public ErrorResult()
        {
            Errors = new List<Error>();
        }
    }
}
