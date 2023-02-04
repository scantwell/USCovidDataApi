using CovidDataApi.Data;
using CovidDataApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CovidDataApi.Controllers.v1.CasesController;

namespace CovidDataApi.Services;

public class RepoService : IRepoService
{
    private readonly ICovidDataContext _context;

    public RepoService(ICovidDataContext _context)
    {
        this._context = _context;
    }

    public async Task<CaseSummaryModel> GetMinMaxAvgCasesByDayAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        var max = await GetMaxAsync(location, startDate, endDate);
        var min = await GetMinAsync(location, startDate, endDate);
        var avg = await GetAverageAsync(location, startDate, endDate);

        avg = Math.Round(avg, 1);

        bool isState = IsState(location);

        return new CaseSummaryModel()
        {
            Max = max,
            Min = min,
            AverageTotalCases = avg,
            County = isState ? "" : location,
            State = max.State,
            Latitude = isState ? "" : max.Latitude.ToString(),
            Longitude = isState ? "" : max.Longitude.ToString(),
        };
    }

    public async Task<CaseTotalsModel> GetCaseNewAndTotalCasesPerDayAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        bool isState = IsState(location);
        var query = GetBaseQuery(location, startDate, endDate, isState);
        DailyCasesModel? info = null;
        if (!isState)
        {
            info = query.Take(1).ToList().FirstOrDefault();
        }
        query = GetBaseQuery(location, startDate, endDate, isState);
        var records = await query.TagWith("GetTotalCases")
            .OrderBy(c => c.Date).Select(c => new
            {
                c.Date,
                c.TotalDailyCases
            })
            .ToListAsync();
            
        int total = 0;
        var model = new CaseTotalsModel()
        {
            County = location,
            State = info?.State,
            Latitude = info?.Latitude,
            Longitude = info?.Longitude,

        };

        foreach (var record in records)
        {
            total += record.TotalDailyCases;
            model.Totals.Add(new CaseTotalsModel.DailyBreakdownRecord(record.TotalDailyCases, total, record.Date));
        }

        return model;
    }


    public async Task<CaseGrowthRateModel> GetCaseGrowthRateAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        bool isState = IsState(location);
        var query = GetBaseQuery(location, startDate, endDate, isState);

        var first = (await query.TagWith("GetGrowthRate").OrderBy(c => c.Date).Take(1).ToListAsync()).FirstOrDefault<DailyCasesModel>();
        var last = (await query.TagWith("GetGrowthRate").OrderByDescending(c => c.Date).Take(1).ToListAsync()).FirstOrDefault<DailyCasesModel>();

        double? growthRate = CalculateGrowthRate(first, last);

        return new CaseGrowthRateModel()
        {
            GrowthRatePercent = growthRate,
            County = isState ? "" : location,
            State = first.State,
            Latitude = isState ? null : first?.Latitude,
            Longitude = isState ? null : first?.Longitude
        };
    }

    public bool IsState(string location)
    {
        return !_context.DailyCasesModel.TagWith("GetIsState").Any(t => t.County == location);
    }

    private double? CalculateGrowthRate(DailyCasesModel first, DailyCasesModel last)
    {
        if (first.TotalDailyCases == 0)
        {
            return null;
        }
        double growthRate = ((Convert.ToDouble(last.TotalDailyCases) - first.TotalDailyCases) / first.TotalDailyCases) * 100;
        return Math.Round(growthRate, 2);
    }

    private async Task<DailyCasesModel> GetMaxAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        var query = GetBaseQuery(location, startDate, endDate, IsState(location));
        return await query.TagWith("GetMaxQuery").OrderByDescending(c => c.TotalDailyCases).ThenBy(c => c.Date).FirstAsync();
    }

    private async Task<DailyCasesModel> GetMinAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        var query = GetBaseQuery(location, startDate, endDate, IsState(location));
        return await query.TagWith("GetMinQuery").OrderBy(c => c.TotalDailyCases).ThenBy(c => c.Date).FirstAsync();
    }


    private async Task<double> GetAverageAsync(string location, DateTime? startDate, DateTime? endDate)
    {
        var query = GetBaseQuery(location, startDate, endDate, IsState(location));
        return await query.TagWith("GetAvgQuery").AverageAsync(c => c.TotalDailyCases);
    }

    private IQueryable<DailyCasesModel> GetBaseQuery(string location, DateTime? startDate, DateTime? endDate, bool isState)
    {
        IQueryable<DailyCasesModel> query = _context.DailyCasesModel.Where(c => location == c.County);
        if (isState)
        {
            query = _context.DailyCasesModel.Where(c => location == c.State);
        }

        if (startDate != null && endDate != null)
        {
            query = query.Where(c => startDate.Value <= c.Date && c.Date <= endDate.Value);

        }
        return query;
    }
}
