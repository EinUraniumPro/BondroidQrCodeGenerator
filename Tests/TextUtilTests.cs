using BondroidQRHelper.Interfaces;
using BondroidQRHelper.Utils;
using SkiaSharp;

namespace BondroidQRHelper.Tests;

public class TextUtilTests
{
    [Test]
    public void GenerateLabel_ShouldReturnNonNullBitmap_ForValidTableName()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap bitmap = util.GenerateLabel("Tisch A");

        // Assert
        Assert.That(bitmap, Is.Not.Null);
    }

    [Test]
    public void GenerateLabel_ShouldReturnBitmapWithPositiveDimensions()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap bitmap = util.GenerateLabel("Stammtisch");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width,  Is.GreaterThan(0));
            Assert.That(bitmap.Height, Is.GreaterThan(0));
        }
    }

    [Test]
    public void GenerateLabel_LargerFontSize_ShouldProduceLargerBitmap()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap small = util.GenerateLabel("Tisch A", fontSize: 50);
        using SKBitmap large = util.GenerateLabel("Tisch A", fontSize: 200);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(large.Width,  Is.GreaterThan(small.Width));
            Assert.That(large.Height, Is.GreaterThan(small.Height));
        }
    }

    [Test]
    public void GenerateLabel_LargerHorizontalPadding_ShouldProduceWiderBitmap()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap narrow = util.GenerateLabel("Tisch A", horizontalPadding: 5);
        using SKBitmap wide   = util.GenerateLabel("Tisch A", horizontalPadding: 100);

        // Assert
        Assert.That(wide.Width, Is.GreaterThan(narrow.Width));
    }

    [Test]
    public void GenerateLabel_LargerVerticalPadding_ShouldProduceTallerBitmap()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap small = util.GenerateLabel("Tisch A", verticalPadding: 5);
        using SKBitmap large = util.GenerateLabel("Tisch A", verticalPadding: 100);

        // Assert
        Assert.That(large.Height, Is.GreaterThan(small.Height));
    }

    [Test]
    public void GenerateLabel_LargerLineSpacing_ShouldProduceTallerBitmap()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap noSpacing   = util.GenerateLabel("Tisch A", lineSpacing: 0);
        using SKBitmap withSpacing = util.GenerateLabel("Tisch A", lineSpacing: 50);

        // Assert
        Assert.That(withSpacing.Height, Is.GreaterThan(noSpacing.Height));
    }

    [Test]
    public void GenerateLabel_ShouldProduceDifferentBitmaps_ForDifferentTableNames()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act
        using SKBitmap bitmap1 = util.GenerateLabel("Tisch A");
        using SKBitmap bitmap2 = util.GenerateLabel("Stammtisch");

        // Assert
        using SKImage img1 = SKImage.FromBitmap(bitmap1);
        using SKImage img2 = SKImage.FromBitmap(bitmap2);
        byte[] bytes1 = img1.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        byte[] bytes2 = img2.Encode(SKEncodedImageFormat.Png, 100).ToArray();

        Assert.That(bytes1, Is.Not.EqualTo(bytes2));
    }

    [Test]
    public void GenerateLabel_ShouldNotThrow_ForEmptyTableName()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using SKBitmap bitmap = util.GenerateLabel(string.Empty);
        });
    }

    [Test]
    public void GenerateLabel_ShouldNotThrow_ForUnicodeTableName()
    {
        // Arrange
        ITextUtil util = new TextUtil();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using SKBitmap bitmap = util.GenerateLabel("Stammtisch");
        });
    }

    [Test]
    public void GenerateLabel_ShouldNotThrow_ForVeryLongTableName()
    {
        // Arrange
        ITextUtil util       = new TextUtil();
        string longName      = new ('A', 200);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using SKBitmap bitmap = util.GenerateLabel(longName);
        });
    }
}
