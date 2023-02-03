using CovidDataApi.Models;
using CsvHelper;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CovidDataApi.Data;

public static class DbInitializer
{
    public static void Initialize(CovidDataContext context, IConfiguration config)
    {
        if (context.DailyCasesModel.Any())
        {
            return;   // DB has been seeded
        }
        string csvFilePath = config.GetValue<string>("CovidDataCsvFilePath");
        List<DailyCasesModel> records = ParseCovidDataCsvFile(csvFilePath);
        BulkLoadIntoSQLDb(context.Database.GetConnectionString(), records);
    }

    private static void BulkLoadIntoSQLDb(string connectionString, List<DailyCasesModel> records)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using (var bcp = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.TableLock))
        {
            bcp.ColumnMappings.Add("County", "County");
            bcp.ColumnMappings.Add("State", "State");
            bcp.ColumnMappings.Add("Latitude", "Latitude");
            bcp.ColumnMappings.Add("Longitude", "Longitude");
            bcp.ColumnMappings.Add("Date", "Date");
            bcp.ColumnMappings.Add("TotalDailyCases", "TotalDailyCases");
            bcp.DestinationTableName = "USDailyCovidCases";
            bcp.BulkCopyTimeout = 120;
            bcp.WriteToServer(ObjectReader.Create(records));
        }
    }

    private static List<DailyCasesModel> ParseCovidDataCsvFile(string csvFile)
    {

        List<DailyCasesModel> records = new();
        using (var reader = new StreamReader(csvFile))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var County = csv.GetField<string>("Admin2");
                var State = csv.GetField<string>("Province_State");
                var Country = csv.GetField<string>("Country_Region");
                var Latitude = csv.GetField<double>("Lat");
                var Longitude = csv.GetField<double>("Long_");
                for (int i = 11; i < csv.HeaderRecord.Length; i++)
                {
                    var record = new DailyCasesModel
                    {
                        County = County,
                        State = State,
                        Latitude = Latitude,
                        Longitude = Longitude,
                        Date = DateTime.Parse(csv.HeaderRecord[i]),
                        TotalDailyCases = csv.GetField<int>(csv.HeaderRecord[i])
                    };
                    records.Add(record);
                }
            }
        }
        return records;
    }
}

