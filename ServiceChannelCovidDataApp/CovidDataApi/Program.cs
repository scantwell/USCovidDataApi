using CovidDataApi.Data;
using CovidDataApi.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ICovidDataContext, CovidDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.")));
builder.Services.AddTransient<IRepoService, RepoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    var title = "Covid Daily Case API";
    var description = "Web API that exposes covid data in the US based on county or state.";

    opts.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = $"{title} v1",
        Description = description,
    });
});
builder.Services.AddApiVersioning(opts =>
{
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.DefaultApiVersion = new(1, 0);
    opts.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(opts =>
{

    opts.GroupNameFormat = "'v'VVV";
    opts.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<CovidDataContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        // Add new versions to the top. This keeps the initial version as the latest in the dropdown.
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
} 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CovidDataContext>();
    context.Database.EnsureCreated();
    var watch = System.Diagnostics.Stopwatch.StartNew();
    DbInitializer.Initialize(context, builder.Configuration);
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
