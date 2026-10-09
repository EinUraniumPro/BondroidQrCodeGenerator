using BondroidQRHelper.Model;

namespace BondroidQRHelper.Interfaces;

/// <summary>
/// Utility to extract the List of Tables from the CSV-File
/// </summary>
public interface ICsvUtil
{
    /// <summary>
    /// Loads all the Tables from the CSV exported by Bondroid and returns it as List
    /// </summary>
    /// <param name="csvPath">The path pointing to the Bondroid-exported CSV file.</param>
    /// <param name="ct">A cancellation token to cancel the enumeration early.</param>
    /// <returns>An async stream of <see cref="BondroidQRHelper.Model.Table"/> records read from the CSV.</returns>
    IAsyncEnumerable<Table> GetTablesFromBondroidCsv(string csvPath, CancellationToken ct = default);
}
