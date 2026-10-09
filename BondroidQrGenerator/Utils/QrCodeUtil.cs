using BondroidQRHelper.Interfaces;
using QRCoder;
using SkiaSharp;

namespace BondroidQRHelper.Utils;

/// <summary>
/// <inheritdoc/>
/// </summary>
public sealed class QrCodeUtil : IQrCodeUtil
{
    private static readonly SKSamplingOptions SamplingOptions = new(SKFilterMode.Nearest);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SKBitmap GenerateQrCode(string qrCodeText)
    {
        using QRCodeGenerator qrGenerator = new();

        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(
            qrCodeText,
            QRCodeGenerator.ECCLevel.H,
            forceUtf8: true
        );

        SKBitmap qrBitmap = RenderQrCode(qrCodeData, pixelsPerModule: 40);

        return qrBitmap;
    }

    private static SKBitmap RenderQrCode(QRCodeData qrCodeData, int pixelsPerModule)
    {
        int moduleCount = qrCodeData.ModuleMatrix.Count;

        int bitmapSize = moduleCount * pixelsPerModule;

        SKBitmap bitmap = new(bitmapSize, bitmapSize);

        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.White);

        using SKPaint darkPaint = new();
        darkPaint.Color = SKColors.Black;
        darkPaint.Style = SKPaintStyle.Fill;
        darkPaint.IsAntialias = false;

        for (int y = 0; y < moduleCount; y++)
        {
            for (int x = 0; x < moduleCount; x++)
            {
                if (!qrCodeData.ModuleMatrix[y][x])
                {
                    continue;
                }

                float left = x * pixelsPerModule;
                float top = y * pixelsPerModule;

                SKRect rect = new(
                    left,
                    top,
                    left + pixelsPerModule,
                    top + pixelsPerModule
                );

                canvas.DrawRect(rect, darkPaint);
            }
        }

        canvas.Flush();
        return bitmap;
    }

    /// <summary>
    /// Saves a given <see cref="SKBitmap"/> as a lossless PNG file to the specified path.
    /// </summary>
    /// <param name="bitmap">The bitmap to encode and save.</param>
    /// <param name="outputPath">The destination file path including the file name and extension.</param>
    public void SavePng(SKBitmap bitmap, string outputPath)
    {
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

        using FileStream stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SKBitmap CombineQrCodeAndLabel(
        SKBitmap qrBitmap,
        SKBitmap labelBitmap,
        int spacing = 0,
        int paddingBottom = 0)
    {
        int width = Math.Max(qrBitmap.Width, labelBitmap.Width);
        int height = qrBitmap.Height + spacing + labelBitmap.Height + paddingBottom;

        SKBitmap result = new(width, height);

        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.White);

        using SKPaint paint = new();
        paint.IsAntialias = false;

        float qrX = (width - qrBitmap.Width) / 2f;
        float qrY = 0;

        float labelX = (width - labelBitmap.Width) / 2f;
        float labelY = qrBitmap.Height + spacing;

        using SKImage qrImage = SKImage.FromBitmap(qrBitmap);
        canvas.DrawImage(qrImage, qrX, qrY, SamplingOptions, paint);
        using SKImage labelImage = SKImage.FromBitmap(labelBitmap);
        canvas.DrawImage(labelImage, labelX, labelY, SamplingOptions, paint);

        canvas.Flush();

        return result;
    }
}
