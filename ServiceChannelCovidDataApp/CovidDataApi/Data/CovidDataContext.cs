using Microsoft.EntityFrameworkCore;
using CovidDataApi.Models;

namespace CovidDataApi.Data
{
    public class CovidDataContext : DbContext, ICovidDataContext
    {
        public CovidDataContext(DbContextOptions<CovidDataContext> options)
            : base(options)
        {
        }

        public DbSet<CovidDataApi.Models.DailyCasesModel> DailyCasesModel { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyCasesModel>().ToTable("USDailyCovidCases");
        }
    }
}
