using BondroidQRHelper.Model;
using SkiaSharp;

namespace BondroidQRHelper.Interfaces;

/// <summary>
/// Utility to extract generate the Label with the TableName
/// </summary>
public interface ITextUtil
{
    /// <summary>
    /// Generates a bitmap containing a text label for a specific table.
    /// </summary>
    /// <param name="tableName">The TableName to render.</param>
    /// <param name="fontSize">The font size to use for the text.</param>
    /// <param name="horizontalPadding">The horizontal padding around the text.</param>
    /// <param name="verticalPadding">The vertical padding around the text.</param>
    /// <param name="lineSpacing">The vertical spacing between multiple lines of text.</param>
    /// <returns>A rendered SKBitmap representing the text label.</returns>
    SKBitmap GenerateLabel(
        string tableName,
        float fontSize = 140,
        int horizontalPadding = 20,
        int verticalPadding = 20,
        int lineSpacing = 5);
}
