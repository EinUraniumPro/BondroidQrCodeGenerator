using BondroidQRHelper.Interfaces;
using SkiaSharp;
using BondroidQRHelper.Model;

namespace BondroidQRHelper.Utils;

/// <summary>
/// <inheritdoc/>
/// </summary>
public sealed class TextUtil : ITextUtil
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SKBitmap GenerateLabel(
        string tableName,
        float fontSize = 140,
        int horizontalPadding = 20,
        int verticalPadding = 20,
        int lineSpacing = 5)
    {
        string[] lines = ["Tisch Nr.:", tableName];

        using SKTypeface typeface = SKTypeface.FromFamilyName(
            SKTypeface.Default.FamilyName,
            SKFontStyle.Bold
        );

        using SKFont font = new(typeface, fontSize);

        using SKPaint textPaint = new();
        textPaint.Color = SKColors.Black;
        textPaint.IsAntialias = true;

        font.GetFontMetrics(out SKFontMetrics metrics);

        float lineHeight = metrics.Descent - metrics.Ascent;
        float totalTextHeight =
            lines.Length * lineHeight +
            (lines.Length - 1) * lineSpacing;

        float maxTextWidth = 0;

        foreach (string line in lines)
        {
            font.MeasureText(line, out SKRect bounds);
            maxTextWidth = Math.Max(maxTextWidth, bounds.Width);
        }

        int bitmapWidth = (int)Math.Ceiling(maxTextWidth) + horizontalPadding * 2;
        int bitmapHeight = (int)Math.Ceiling(totalTextHeight) + verticalPadding * 2;

        SKBitmap result = new(bitmapWidth, bitmapHeight);

        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.White);

        float y = verticalPadding - metrics.Ascent;

        foreach (string line in lines)
        {
            font.MeasureText(line, out SKRect bounds);

            float x = (bitmapWidth - bounds.Width) / 2f - bounds.Left;

            canvas.DrawText(line, x, y, font, textPaint);

            y += lineHeight + lineSpacing;
        }

        canvas.Flush();

        return result;
    }
}
