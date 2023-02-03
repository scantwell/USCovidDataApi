using Microsoft.AspNetCore.Mvc;
using CovidDataApi.Models;
using CovidDataApi.Services;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace CovidDataApi.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[ResponseCache(Duration = 60 * 60 * 24, Location = ResponseCacheLocation.Any, NoStore = false)]
public class CasesController : ControllerBase
{
    public class QueryParameters
    {
        [Required]
        public string Location { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    private readonly IRepoService _repo;
    private readonly ILogger _logger;

    public CasesController(IRepoService repo, ILogger<CasesController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    [HttpGet("Summary")]
    [ProducesResponseType(typeof(CaseSummaryModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CaseSummaryModel>> GetDailyTotalsModel([FromQuery] QueryParameters parameters)
    {
        _logger.LogInformation("GET: api/Summary");
        try
        {
            if ((parameters.StartDate != null && parameters.EndDate == null) || (parameters.StartDate == null && parameters.EndDate != null))
                return BadRequest("Both startDate and endDate are required.");
            var model = await _repo.GetMinMaxAvgCasesByDayAsync(parameters.Location, parameters.StartDate, parameters.EndDate);
            return Ok(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET: api/Summary call has failed.");
            return BadRequest();
        }
    }

    [HttpGet("Totals")]
    [ProducesResponseType(typeof(CaseTotalsModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CaseTotalsModel>> GetDailyBreakdown([FromQuery] QueryParameters parameters)
    {
        _logger.LogInformation("GET: api/Totals");
        try
        {
            if ((parameters.StartDate != null && parameters.EndDate == null) || (parameters.StartDate == null && parameters.EndDate != null))
                return BadRequest("Both startDate and endDate are required.");
            var model = await _repo.GetCaseNewAndTotalCasesPerDayAsync(parameters.Location, parameters.StartDate, parameters.EndDate);
            return Ok(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET: api/Totals call has failed.");
            return BadRequest();
        }
    }

    [HttpGet("GrowthRate")]
    [ProducesResponseType(typeof(CaseGrowthRateModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CaseGrowthRateModel>> GetGrowthRate([FromQuery] QueryParameters parameters)
    {
        _logger.LogInformation("GET: api/GrowthRate");
        try
        {
            if ((parameters.StartDate != null && parameters.EndDate == null) || (parameters.StartDate == null && parameters.EndDate != null))
                return BadRequest("Both startDate and endDate are required.");
            var model = await _repo.GetCaseGrowthRateAsync(parameters.Location, parameters.StartDate, parameters.EndDate);
            return Ok(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET: api/GrowthRate call has failed.");
            return BadRequest();
        }
    }
}
