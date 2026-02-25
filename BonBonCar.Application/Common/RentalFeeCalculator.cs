namespace BonBonCar.Application.Common
{
    public static class RentalFeeCalculator
    {
        public static decimal CalcTotalPrice(
            DateTime pickup,
            DateTime ret,
            decimal p4,
            decimal p8,
            decimal p12,
            decimal p24)
        {
            var totalHours = CalcTotalHoursCeil(pickup, ret);
            return CalcPrice(totalHours, p4, p8, p12, p24);
        }

        public static int CalcTotalHoursCeil(DateTime pickup, DateTime ret)
        {
            var minutes = (ret - pickup).TotalMinutes;
            if (minutes <= 0) return 0;
            return (int)Math.Ceiling(minutes / 60d);
        }

        public static decimal CalcPrice(
            int totalHours,
            decimal p4,
            decimal p8,
            decimal p12,
            decimal p24)
        {
            if (totalHours <= 0) return 0m;

            var days = totalHours / 24;
            var rem = totalHours % 24;
            decimal total = 0m;

            if (days > 0) total += days * p24;

            if (rem > 0)
            {
                if (rem <= 4) total += p4;
                else if (rem <= 8) total += p8;
                else if (rem <= 12) total += p12;
                else total += p24;
            }

            return total;
        }
    }
}

