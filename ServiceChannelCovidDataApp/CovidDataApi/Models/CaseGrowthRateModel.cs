namespace CovidDataApi.Models
{
    public class CaseGrowthRateModel
    {
        public string County { get; set; }
        public string State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? GrowthRatePercent { get; set; }
    }
}
