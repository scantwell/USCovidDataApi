using CovidDataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CovidDataApi.Data
{
    public interface ICovidDataContext
    {
        DbSet<DailyCasesModel> DailyCasesModel { get; set; }
    }
}