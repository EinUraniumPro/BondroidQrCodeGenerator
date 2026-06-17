using System.Globalization;
using System.Runtime.CompilerServices;
using BondroidQRHelper.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using BondroidQRHelper.Model;

namespace BondroidQRHelper.Utils;

/// <summary>
/// <inheritdoc/>
/// </summary>
public sealed class CsvUtil : ICsvUtil
{
    private static readonly CsvConfiguration CsvConfig = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";", HasHeaderRecord = true,
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async IAsyncEnumerable<Table> GetTablesFromBondroidCsv(string csvPath,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using StreamReader reader = new(csvPath);
        using CsvReader csvReader = new(reader, CsvConfig);

        IAsyncEnumerable<Table> records = csvReader.GetRecordsAsync<Table>(ct);

        await foreach (Table table in records)
        {
            yield return table;
        }
    }
}
