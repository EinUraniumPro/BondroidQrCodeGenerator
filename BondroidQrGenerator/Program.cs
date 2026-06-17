using System.CommandLine;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using BondroidQRHelper.Interfaces;
using BondroidQRHelper.Model;
using BondroidQRHelper.Utils;
using SkiaSharp;

ServiceCollection services = new();
services.AddSingleton<ICsvUtil, CsvUtil>();
services.AddSingleton<IQrCodeUtil, QrCodeUtil>();
services.AddSingleton<ITextUtil, TextUtil>();
services.AddSingleton<IPdfUtil, PdfUtil>();

await using ServiceProvider sp = services.BuildServiceProvider();

ICsvUtil csvUtil = sp.GetRequiredService<ICsvUtil>();
IPdfUtil pdfUtil = sp.GetRequiredService<IPdfUtil>();
IQrCodeUtil qrCodeUtil = sp.GetRequiredService<IQrCodeUtil>();
ITextUtil textUtil = sp.GetRequiredService<ITextUtil>();

Option<FileInfo> inputOption = new("--input", "-i")
{
    Description = "The path to the input CSV file.", 
    Required = true,
};
inputOption.Validators.Add(result =>
{
    if (result.GetValueOrDefault<FileInfo>() is { Exists: false } fileInfo)
    {
        result.AddError($"The input CSV file doesn't exist: {fileInfo.FullName}");
    }
});

Option<FileInfo> outputOption = new("--output", "-o")
{
    Description = "The path to the output PDF file. If not specified, a default name will be used.",
};

RootCommand rootCommand = new("Bondroid QR-Code Generator CLI can be used to generate Table QR-Codes from the CSV file")
{
    inputOption, 
    outputOption,
};

ParseResult parseResult = rootCommand.Parse(args);

if (parseResult.Errors.Count != 0 || parseResult.Action is not null)
{
    return await parseResult.InvokeAsync();
}

FileInfo? input = parseResult.GetValue(inputOption);
FileInfo? output = parseResult.GetValue(outputOption);

if (input is null)
{
    return 1;
}

string defaultOutput = Path.Combine(
    input.DirectoryName ?? "",
    $"bondroid_qr_codes_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf"
);
string outputFilename = output?.FullName ?? defaultOutput;
string csvPath = input.FullName;

List<Table> loadedTables;
Console.WriteLine($"Loading Tables from {csvPath}...");

Stopwatch sw = Stopwatch.StartNew();

try
{
    IAsyncEnumerable<Table> tableEnumerable = csvUtil.GetTablesFromBondroidCsv(csvPath);
    loadedTables = await tableEnumerable.ToListAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading csv '{csvPath}': {ex.Message}");
    return 1;
}

if (loadedTables.Count == 0)
{
    Console.WriteLine("No tables found or error reading the CSV. Aborting.");
    return 1;
}

Console.WriteLine($"Found {loadedTables.Count} tables. Generating QR codes...");

int failedTablesCount = 0;
SKBitmap?[] mergedQrCodes = new SKBitmap[loadedTables.Count];
ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

Parallel.For(0, loadedTables.Count, parallelOptions, i =>
{
    Table table = loadedTables[i];

    if (string.IsNullOrWhiteSpace(table.TableName) || string.IsNullOrWhiteSpace(table.QrCodeText))
    {
        return;
    }

    SKBitmap? qrBitmap = null;
    SKBitmap? labelBitmap = null;

    try
    {
        qrBitmap = qrCodeUtil.GenerateQrCode(table.QrCodeText);
        labelBitmap = textUtil.GenerateLabel(table.TableName);

        SKBitmap combined = qrCodeUtil.CombineQrCodeAndLabel(qrBitmap, labelBitmap);
        mergedQrCodes[i] = combined;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing table '{table.TableName}': {ex.Message}");
        Interlocked.Increment(ref failedTablesCount);
    }
    finally
    {
        qrBitmap?.Dispose();
        labelBitmap?.Dispose();
    }
});

Console.WriteLine($"Writing PDF to {outputFilename}...");
try
{
    pdfUtil.CreateQrGridPdf(mergedQrCodes, outputFilename);
}
catch (Exception ex)
{
    Console.WriteLine($"Error creating PDF-File '{outputFilename}': {ex.Message}");
    return 1;
}

sw.Stop();

foreach (SKBitmap? bitmap in mergedQrCodes)
{
    bitmap?.Dispose();
}

Console.WriteLine(failedTablesCount == 0
    ? "Generation successfully finished!"
    : $"Generation failed for {failedTablesCount} Tables.");

Console.WriteLine($"Generation took {sw.Elapsed}");
Console.WriteLine($"PDF saved as:\n{Path.GetFullPath(outputFilename)}");

return 0;
