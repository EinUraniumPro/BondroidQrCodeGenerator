using BondroidQRHelper.Interfaces;
using BondroidQRHelper.Utils;
using SkiaSharp;

namespace BondroidQRHelper.Tests;

public class PdfUtilTests
{
    private static string GetTempPdfPath() =>
        Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

    private static SKBitmap CreateBitmap(int width = 100, int height = 100, SKColor? color = null)
    {
        SKBitmap bitmap = new(width, height);
        bitmap.Erase(color ?? SKColors.Red);
        return bitmap;
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateFile_WhenSingleBitmapProvided()
    {
        // Arrange
        IPdfUtil util    = new PdfUtil();
        string pdfPath   = GetTempPdfPath();
        using SKBitmap bm = CreateBitmap();

        // Act
        util.CreateQrGridPdf([bm], pdfPath);

        // Assert
        Assert.That(File.Exists(pdfPath), Is.True);

        // Cleanup
        File.Delete(pdfPath);
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateFile_WithMultipleBitmaps()
    {
        // Arrange
        IPdfUtil util  = new PdfUtil();
        string pdfPath = GetTempPdfPath();

        List<SKBitmap?> bitmaps =
        [
            CreateBitmap(color: SKColors.Red),
            CreateBitmap(color: SKColors.Green),
            CreateBitmap(color: SKColors.Blue),
            CreateBitmap(color: SKColors.Yellow),
            CreateBitmap(color: SKColors.Purple),
        ];

        // Act
        util.CreateQrGridPdf(bitmaps, pdfPath);

        // Assert
        Assert.That(File.Exists(pdfPath), Is.True);
        Assert.That(new FileInfo(pdfPath).Length, Is.GreaterThan(0));

        // Cleanup
        foreach (SKBitmap? bm in bitmaps)
        {
            bm?.Dispose();
        }

        File.Delete(pdfPath);
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateNonEmptyFile()
    {
        // Arrange
        IPdfUtil util    = new PdfUtil();
        string pdfPath   = GetTempPdfPath();
        using SKBitmap bm = CreateBitmap();

        // Act
        util.CreateQrGridPdf([bm], pdfPath);

        // Assert
        Assert.That(new FileInfo(pdfPath).Length, Is.GreaterThan(0));

        // Cleanup
        File.Delete(pdfPath);
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateLargerFile_WithMoreBitmaps()
    {
        // Arrange
        IPdfUtil util      = new PdfUtil();
        string pdfPath1    = GetTempPdfPath();
        string pdfPath2    = GetTempPdfPath();

        List<SKBitmap?> one  = [CreateBitmap()];
        List<SKBitmap?> many =
        [
            CreateBitmap(), CreateBitmap(), CreateBitmap(),
            CreateBitmap(), CreateBitmap(), CreateBitmap(),
            CreateBitmap(), CreateBitmap(), CreateBitmap(),
        ];

        // Act
        util.CreateQrGridPdf(one,  pdfPath1);
        util.CreateQrGridPdf(many, pdfPath2);

        // Assert
        Assert.That(new FileInfo(pdfPath2).Length, Is.GreaterThan(new FileInfo(pdfPath1).Length));

        // Cleanup
        foreach (SKBitmap? bm in one)  bm?.Dispose();
        foreach (SKBitmap? bm in many) bm?.Dispose();
        File.Delete(pdfPath1);
        File.Delete(pdfPath2);
    }

    [Test]
    public void CreateQrGridPdf_ShouldNotThrow_WhenAllBitmapsAreNull()
    {
        // Arrange
        IPdfUtil util  = new PdfUtil();
        string pdfPath = GetTempPdfPath();

        List<SKBitmap?> nullBitmaps = [null, null, null];

        // Act & Assert
        Assert.DoesNotThrow(() => util.CreateQrGridPdf(nullBitmaps, pdfPath));

        // Cleanup
        if (File.Exists(pdfPath))
        {
            File.Delete(pdfPath);
        }
    }

    [Test]
    public void CreateQrGridPdf_ShouldNotThrow_WithEmptyList()
    {
        // Arrange
        IPdfUtil util  = new PdfUtil();
        string pdfPath = GetTempPdfPath();

        // Act & Assert
        Assert.DoesNotThrow(() => util.CreateQrGridPdf([], pdfPath));

        // Cleanup
        if (File.Exists(pdfPath))
        {
            File.Delete(pdfPath);
        }
    }

    [Test]
    public void CreateQrGridPdf_ShouldNotThrow_WhenMixedNullAndValidBitmaps()
    {
        // Arrange
        IPdfUtil util    = new PdfUtil();
        string pdfPath   = GetTempPdfPath();
        using SKBitmap bm = CreateBitmap();

        List<SKBitmap?> mixed = [null, bm, null];

        // Act & Assert
        Assert.DoesNotThrow(() => util.CreateQrGridPdf(mixed, pdfPath));

        Assert.That(File.Exists(pdfPath), Is.True);

        // Cleanup
        File.Delete(pdfPath);
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateFile_WithCustomColumns()
    {
        // Arrange
        IPdfUtil util  = new PdfUtil();
        string pdfPath = GetTempPdfPath();

        List<SKBitmap?> bitmaps = Enumerable.Range(0, 6)
            .Select(_ => (SKBitmap?)CreateBitmap())
            .ToList();

        // Act & Assert
        Assert.DoesNotThrow(() => util.CreateQrGridPdf(bitmaps, pdfPath, columns: 2));

        Assert.That(File.Exists(pdfPath), Is.True);

        // Cleanup
        foreach (SKBitmap? bm in bitmaps) bm?.Dispose();
        File.Delete(pdfPath);
    }

    [Test]
    public void CreateQrGridPdf_ShouldCreateFile_WithZeroMarginAndPadding()
    {
        // Arrange
        IPdfUtil util    = new PdfUtil();
        string pdfPath   = GetTempPdfPath();
        using SKBitmap bm = CreateBitmap();

        // Act & Assert
        Assert.DoesNotThrow(() => util.CreateQrGridPdf([bm], pdfPath, margin: 0, cellPadding: 0));

        Assert.That(File.Exists(pdfPath), Is.True);

        // Cleanup
        File.Delete(pdfPath);
    }
}
