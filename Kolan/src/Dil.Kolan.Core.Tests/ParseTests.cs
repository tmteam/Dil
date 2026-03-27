using Dil.Core.Models;

namespace Dil.Kolan.Core.Tests;

/// <summary>
/// Тесты парсинга входных данных — совместимость со старыми dil-приложениями.
/// </summary>
public class ParseTests
{
    // Используем тот же парсер что и в UI (через рефлексию, т.к. он private).
    // Вместо этого тестируем через PasteData в MainViewModel.
    // Но проще вынести парсер — пока тестируем паттерны напрямую.

    private static bool TryParseDouble(string s, out double value)
    {
        return double.TryParse(
            s.Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out value);
    }

    // ============================================
    // Десятичные разделители
    // ============================================

    [Fact]
    public void ParseDouble_DotSeparator()
    {
        Assert.True(TryParseDouble("10.64", out var v));
        Assert.Equal(10.64, v, precision: 10);
    }

    [Fact]
    public void ParseDouble_CommaSeparator()
    {
        Assert.True(TryParseDouble("10,64", out var v));
        Assert.Equal(10.64, v, precision: 10);
    }

    [Fact]
    public void ParseDouble_Integer()
    {
        Assert.True(TryParseDouble("20", out var v));
        Assert.Equal(20.0, v);
    }

    [Fact]
    public void ParseDouble_Negative()
    {
        Assert.True(TryParseDouble("-3.5", out var v));
        Assert.Equal(-3.5, v);
    }

    [Fact]
    public void ParseDouble_Zero()
    {
        Assert.True(TryParseDouble("0", out var v));
        Assert.Equal(0.0, v);
    }

    [Fact]
    public void ParseDouble_InvalidString_Fails()
    {
        Assert.False(TryParseDouble("abc", out _));
    }

    [Fact]
    public void ParseDouble_Empty_Fails()
    {
        Assert.False(TryParseDouble("", out _));
    }

    // ============================================
    // Разделители строк
    // ============================================

    [Fact]
    public void SplitLines_Unix_LF()
    {
        var text = "1\t0\t10.5\t8.3\n2\t1\t11.0\t9.0";
        var lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
    }

    [Fact]
    public void SplitLines_Windows_CRLF()
    {
        var text = "1\t0\t10.5\t8.3\r\n2\t1\t11.0\t9.0";
        var lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
    }

    [Fact]
    public void SplitLines_OldMac_CR()
    {
        var text = "1\t0\t10.5\t8.3\r2\t1\t11.0\t9.0";
        var lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
    }

    // ============================================
    // Разделители столбцов
    // ============================================

    [Fact]
    public void SplitColumns_Tab()
    {
        var parts = "1\t0\t10.5\t8.3".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(4, parts.Length);
    }

    [Fact]
    public void SplitColumns_Spaces()
    {
        var parts = "1 0 10.5 8.3".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(4, parts.Length);
    }

    [Fact]
    public void SplitColumns_MultipleSpaces()
    {
        var parts = "1   0   10.5   8.3".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(4, parts.Length);
    }

    [Fact]
    public void SplitColumns_MixedTabsAndSpaces()
    {
        var parts = "1\t 0 \t10.5\t8.3".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(4, parts.Length);
    }

    // ============================================
    // Дополнительные столбцы (как в старых приложениях — игнорируются)
    // ============================================

    [Fact]
    public void ExtraColumns_AreIgnored()
    {
        // В Excel может быть больше 4 столбцов — лишние игнорируем
        var parts = "1\t0\t10.5\t8.3\t99.9\textra".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.True(parts.Length >= 4);
        Assert.True(int.TryParse(parts[0], out _));
        Assert.True(int.TryParse(parts[1], out _));
        Assert.True(TryParseDouble(parts[2], out _));
        Assert.True(TryParseDouble(parts[3], out _));
    }

    // ============================================
    // Крайние случаи
    // ============================================

    [Fact]
    public void TooFewColumns_Detected()
    {
        var parts = "1\t0\t10.5".Split(new[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        Assert.True(parts.Length < 4);
    }

    [Fact]
    public void NonNumericKm_Detected()
    {
        Assert.False(int.TryParse("abc", out _));
    }

    [Fact]
    public void CommaInKm_Detected()
    {
        // "1,5" как километр — это не целое число
        Assert.False(int.TryParse("1,5", out _));
    }
}
