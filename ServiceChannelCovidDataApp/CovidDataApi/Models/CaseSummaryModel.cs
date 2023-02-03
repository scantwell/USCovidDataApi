using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.X86;

namespace CovidDataApi.Models
{
    public class CaseSummaryModel
    {        
        public DailyCasesModel Max { get; set; }
        public DailyCasesModel Min { get; set; }
        public double AverageTotalCases { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
