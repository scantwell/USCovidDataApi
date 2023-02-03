using CovidDataApi.Models;

namespace CovidDataApi.Services
{
    public interface IRepoService
    {
        Task<CaseGrowthRateModel> GetCaseGrowthRateAsync(string location, DateTime? startDate, DateTime? endDate);
        Task<CaseTotalsModel> GetCaseNewAndTotalCasesPerDayAsync(string location, DateTime? startDate, DateTime? endDate);
        Task<CaseSummaryModel> GetMinMaxAvgCasesByDayAsync(string location, DateTime? startDate, DateTime? endDate);
        bool IsState(string location);
    }
}