using BondroidQRHelper.Interfaces;
using BondroidQRHelper.Model;
using BondroidQRHelper.Utils;

namespace BondroidQRHelper.Tests;

public class CsvUtilTests
{
    private static async Task<List<T>> ToListAsync<T>(
        IAsyncEnumerable<T> source,
        CancellationToken ct = default)
    {
        List<T> result = [];
        await foreach (T item in source.WithCancellation(ct))
        {
            result.Add(item);
        }

        return result;
    }

    private static string CreateTempCsv(string content)
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        File.WriteAllText(path, content);
        return path;
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldReadValidCsv_WithTwoRows()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string csvPath = CreateTempCsv(
            """
            ID;Bereich;Tisch;QR Text
            1;Terrasse;Tisch A;id:120
            2;Innen;Tisch B;id:121
            """
        );

        // Act
        List<Table> tables = await ToListAsync(
            csvUtil.GetTablesFromBondroidCsv(csvPath)
        );

        // Assert
        Assert.That(tables, Has.Count.EqualTo(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tables[0].Id,          Is.EqualTo(1));
            Assert.That(tables[0].Area,        Is.EqualTo("Terrasse"));
            Assert.That(tables[0].TableName,   Is.EqualTo("Tisch A"));
            Assert.That(tables[0].QrCodeText,  Is.EqualTo("id:120"));

            Assert.That(tables[1].Id,          Is.EqualTo(2));
            Assert.That(tables[1].Area,        Is.EqualTo("Innen"));
            Assert.That(tables[1].TableName,   Is.EqualTo("Tisch B"));
            Assert.That(tables[1].QrCodeText,  Is.EqualTo("id:121"));
        }
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldReadSingleRow()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string csvPath = CreateTempCsv(
            """
            ID;Bereich;Tisch;QR Text
            42;VIP;Stammtisch;id:120
            """
        );

        // Act
        List<Table> tables = await ToListAsync(
            csvUtil.GetTablesFromBondroidCsv(csvPath)
        );

        // Assert
        Assert.That(tables, Has.Count.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tables[0].Id,         Is.EqualTo(42));
            Assert.That(tables[0].Area,       Is.EqualTo("VIP"));
            Assert.That(tables[0].TableName,  Is.EqualTo("Stammtisch"));
            Assert.That(tables[0].QrCodeText, Is.EqualTo("id:120"));
        }
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldReturnEmptyList_WhenCsvOnlyHasHeader()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string csvPath = CreateTempCsv(
            """
            ID;Bereich;Tisch;QR Text
            """
        );

        // Act
        List<Table> tables = await ToListAsync(
            csvUtil.GetTablesFromBondroidCsv(csvPath)
        );

        // Assert
        Assert.That(tables, Is.Empty);
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldHandleOptionalNullableColumns_WhenFieldsAreEmpty()
    {
        // Arrange – Area, TableName and QrCodeText are nullable; only ID is required
        ICsvUtil csvUtil = new CsvUtil();
        string csvPath = CreateTempCsv(
            """
            ID;Bereich;Tisch;QR Text
            7;;;
            """
        );

        // Act
        List<Table> tables = await ToListAsync(
            csvUtil.GetTablesFromBondroidCsv(csvPath)
        );

        // Assert
        Assert.That(tables, Has.Count.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tables[0].Id, Is.EqualTo(7));
            // Empty CSV fields are deserialized as empty strings by CsvHelper
            Assert.That(tables[0].Area,       Is.Empty.Or.Null);
            Assert.That(tables[0].TableName,  Is.Empty.Or.Null);
            Assert.That(tables[0].QrCodeText, Is.Empty.Or.Null);
        }
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldThrow_WhenFileDoesNotExist()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string missingPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

        // Act + Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await ToListAsync(csvUtil.GetTablesFromBondroidCsv(missingPath));
        });
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldBeLazy_AndNotThrowBeforeEnumeration()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string missingPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

        // Act
        IAsyncEnumerable<Table> enumerable = csvUtil.GetTablesFromBondroidCsv(missingPath);

        // Assert
        Assert.That(enumerable, Is.Not.Null);

        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await ToListAsync(enumerable);
        });
    }

    [Test]
    public async Task GetTablesFromBondroidCsv_ShouldThrow_WhenCancellationTokenIsAlreadyCancelled()
    {
        // Arrange
        ICsvUtil csvUtil = new CsvUtil();
        string csvPath = CreateTempCsv(
            """
            ID;Bereich;Tisch;QR Text
            1;Terrasse;Tisch A;id:120
            2;Innen;Tisch B;id:121
            """
        );

        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await ToListAsync(
                csvUtil.GetTablesFromBondroidCsv(csvPath, cts.Token),
                cts.Token
            );
        });
    }
}
