using Microsoft.EntityFrameworkCore;

namespace CovidDataApi.Models
{
    [Index(nameof(County), nameof(State), nameof(Date))]
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