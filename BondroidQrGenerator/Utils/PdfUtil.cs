using BondroidQRHelper.Interfaces;
using SkiaSharp;

namespace BondroidQRHelper.Utils;

/// <summary>
/// <inheritdoc/>
/// </summary>
public sealed class PdfUtil : IPdfUtil
{
    private static readonly SKSamplingOptions SamplingOptions = new(SKFilterMode.Nearest);

    private const float PageWidth = 595; // A4 Width in PDF-Points
    private const float PageHeight = 842; // A4 Height in PDF-Points

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void CreateQrGridPdf(
        IReadOnlyList<SKBitmap?> qrBitmaps,
        string outputPath,
        int columns = 3,
        float margin = 5,
        float cellPadding = 10)
    {
        using SKDocument document = SKDocument.CreatePdf(outputPath);

        float usableWidth = PageWidth - 2 * margin;
        float usableHeight = PageHeight - 2 * margin;

        float cellWidth = usableWidth / columns;

        float cellHeight = cellWidth;

        int rowsPerPage = Math.Max(1, (int)(usableHeight / cellHeight));
        int itemsPerPage = columns * rowsPerPage;

        SKCanvas? canvas = null;
        int renderedCount = 0;

        for (int i = 0; i < qrBitmaps.Count; i++)
        {
            SKBitmap? qrBitmap = qrBitmaps[i];
            if (qrBitmap is null)
            {
                continue;
            }

            if (renderedCount % itemsPerPage == 0)
            {
                if (canvas is not null)
                {
                    document.EndPage();
                }

                canvas = document.BeginPage(PageWidth, PageHeight);
                canvas.Clear(SKColors.White);
            }

            int indexOnPage = renderedCount % itemsPerPage;
            int row = indexOnPage / columns;
            int col = indexOnPage % columns;

            float cellX = margin + col * cellWidth;
            float cellY = margin + row * cellHeight;

            SKRect targetRect = new(
                cellX + cellPadding,
                cellY + cellPadding,
                cellX + cellWidth - cellPadding,
                cellY + cellHeight - cellPadding
            );

            DrawBitmapFitCentered(canvas!, qrBitmap, targetRect);
            renderedCount++;
        }

        if (canvas is not null)
        {
            document.EndPage();
        }

        document.Close();
    }

    private static void DrawBitmapFitCentered(
        SKCanvas canvas,
        SKBitmap bitmap,
        SKRect targetRect)
    {
        float scale = Math.Min(
            targetRect.Width / bitmap.Width,
            targetRect.Height / bitmap.Height
        );

        float drawWidth = bitmap.Width * scale;
        float drawHeight = bitmap.Height * scale;

        float x = targetRect.Left + (targetRect.Width - drawWidth) / 2f;
        float y = targetRect.Top + (targetRect.Height - drawHeight) / 2f;

        SKRect destRect = new(
            x,
            y,
            x + drawWidth,
            y + drawHeight
        );

        using SKPaint paint = new();
        paint.IsAntialias = false;

        using SKImage image = SKImage.FromBitmap(bitmap);
        canvas.DrawImage(image, destRect, SamplingOptions, paint);
    }
}
