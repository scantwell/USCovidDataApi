using Microsoft.EntityFrameworkCore;

namespace CovidDataApi.Models
{
    [Index(nameof(County), nameof(TotalDailyCases), nameof(Date), IsDescending = new[] { false, true, false})]
    [Index(nameof(State), nameof(TotalDailyCases), nameof(Date), IsDescending = new[] { false, true, false })]
    public class DailyCasesModel
    {
        public int ID { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public int TotalDailyCases { get; set; }
    }
}