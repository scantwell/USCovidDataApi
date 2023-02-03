namespace CovidDataApi.Models
{
    public class CaseTotalsModel
    {
        public record DailyBreakdownRecord(int NewCases, int TotalCases, DateTime Date);

        public string? County { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<DailyBreakdownRecord> Totals { get; set; } = new List<DailyBreakdownRecord>();

    }
}
