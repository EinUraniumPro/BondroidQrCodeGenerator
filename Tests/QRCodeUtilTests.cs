using BondroidQRHelper.Interfaces;
using BondroidQRHelper.Utils;
using SkiaSharp;

namespace BondroidQRHelper.Tests;

public class QRCodeUtilTests
{
    [Test]
    public void GenerateQrCode_ShouldReturnNonNullBitmap_ForValidText()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act
        using SKBitmap bitmap = util.GenerateQrCode("id:120");

        // Assert
        Assert.That(bitmap, Is.Not.Null);
    }

    [Test]
    public void GenerateQrCode_ShouldReturnSquareBitmap()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act
        using SKBitmap bitmap = util.GenerateQrCode("id:120");

        // Assert
        Assert.That(bitmap.Width, Is.EqualTo(bitmap.Height));
    }

    [Test]
    public void GenerateQrCode_ShouldReturnBitmapWithPositiveSize()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act
        using SKBitmap bitmap = util.GenerateQrCode("Hello World");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width,  Is.GreaterThan(0));
            Assert.That(bitmap.Height, Is.GreaterThan(0));
        }
    }

    [Test]
    public void GenerateQrCode_ShouldProduceDifferentBitmaps_ForDifferentTexts()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act
        using SKBitmap bitmap1 = util.GenerateQrCode("id:120");
        using SKBitmap bitmap2 = util.GenerateQrCode("id:121");

        // Assert
        using SKImage image1 = SKImage.FromBitmap(bitmap1);
        using SKImage image2 = SKImage.FromBitmap(bitmap2);
        byte[] bytes1 = image1.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        byte[] bytes2 = image2.Encode(SKEncodedImageFormat.Png, 100).ToArray();

        Assert.That(bytes1, Is.Not.EqualTo(bytes2));
    }

    [Test]
    public void GenerateQrCode_ShouldProduceIdenticalBitmaps_ForSameText()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();
        const string text = "id:120";

        // Act
        using SKBitmap bitmap1 = util.GenerateQrCode(text);
        using SKBitmap bitmap2 = util.GenerateQrCode(text);

        // Assert
        using SKImage image1 = SKImage.FromBitmap(bitmap1);
        using SKImage image2 = SKImage.FromBitmap(bitmap2);
        byte[] bytes1 = image1.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        byte[] bytes2 = image2.Encode(SKEncodedImageFormat.Png, 100).ToArray();

        Assert.That(bytes1, Is.EqualTo(bytes2));
    }

    [Test]
    public void GenerateQrCode_ShouldSupportEmptyString()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using SKBitmap bitmap = util.GenerateQrCode(string.Empty);
        });
    }

    [Test]
    public void GenerateQrCode_ShouldSupportUnicodeText()
    {
        // Arrange
        IQrCodeUtil util = new QrCodeUtil();

        // Act & Assert – Unicode / emoji content must not throw (forceUtf8: true)
        Assert.DoesNotThrow(() =>
        {
            using SKBitmap bitmap = util.GenerateQrCode("Tisch Nr.: Stammtisch");
        });
    }

    [Test]
    public void CombineQrCodeAndLabel_ShouldReturnNonNullBitmap()
    {
        // Arrange
        IQrCodeUtil util    = new QrCodeUtil();
        ITextUtil textUtil  = new TextUtil();

        using SKBitmap qr    = util.GenerateQrCode("id:120");
        using SKBitmap label = textUtil.GenerateLabel("Tisch A");

        // Act
        using SKBitmap combined = util.CombineQrCodeAndLabel(qr, label);

        // Assert
        Assert.That(combined, Is.Not.Null);
    }

    [Test]
    public void CombineQrCodeAndLabel_ShouldHaveCorrectHeight_WithoutSpacingOrPadding()
    {
        // Arrange
        IQrCodeUtil util    = new QrCodeUtil();
        ITextUtil textUtil  = new TextUtil();

        using SKBitmap qr    = util.GenerateQrCode("id:120");
        using SKBitmap label = textUtil.GenerateLabel("Tisch A");

        int expectedHeight = qr.Height + label.Height;

        // Act
        using SKBitmap combined = util.CombineQrCodeAndLabel(qr, label, spacing: 0, paddingBottom: 0);

        // Assert
        Assert.That(combined.Height, Is.EqualTo(expectedHeight));
    }

    [Test]
    public void CombineQrCodeAndLabel_ShouldIncludeSpacingInHeight()
    {
        // Arrange
        IQrCodeUtil util    = new QrCodeUtil();
        ITextUtil textUtil  = new TextUtil();

        using SKBitmap qr    = util.GenerateQrCode("id:120");
        using SKBitmap label = textUtil.GenerateLabel("Tisch A");

        const int spacing       = 20;
        const int paddingBottom = 10;
        int expectedHeight      = qr.Height + spacing + label.Height + paddingBottom;

        // Act
        using SKBitmap combined = util.CombineQrCodeAndLabel(qr, label, spacing, paddingBottom);

        // Assert
        Assert.That(combined.Height, Is.EqualTo(expectedHeight));
    }

    [Test]
    public void CombineQrCodeAndLabel_WidthShouldBeMaxOfQrAndLabel()
    {
        // Arrange
        IQrCodeUtil util    = new QrCodeUtil();
        ITextUtil textUtil  = new TextUtil();

        using SKBitmap qr    = util.GenerateQrCode("id:120");
        using SKBitmap label = textUtil.GenerateLabel("Tisch A");

        int expectedWidth = Math.Max(qr.Width, label.Width);

        // Act
        using SKBitmap combined = util.CombineQrCodeAndLabel(qr, label);

        // Assert
        Assert.That(combined.Width, Is.EqualTo(expectedWidth));
    }
}
