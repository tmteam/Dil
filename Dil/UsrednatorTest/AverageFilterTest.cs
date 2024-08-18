using System.Diagnostics.CodeAnalysis;
using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

[SuppressMessage("Assertion",
    "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class AverageFilterTest
{
    [Test]
    public void Test_1()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 200 200 1", " "),
                NumberDataItem.ParseOrThrow("42 400 400 1", " "),
                NumberDataItem.ParseOrThrow("42 600 600 1", " "),
                NumberDataItem.ParseOrThrow("42 800 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 200 200 1", " "),
                NumberDataItem.ParseOrThrow("43 400 400 1", " "),
                NumberDataItem.ParseOrThrow("43 600 600 1", " "),
                NumberDataItem.ParseOrThrow("43 800 800 1", " "),
            };

        var average = UsrednatorLogic.AverageFilter(input, 400, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 100,00 1,00
            42 400 500,00 1,00
            42 800 266,67 1,00
            43 200 300,00 1,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_2()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 200 200 1", " "),
                NumberDataItem.ParseOrThrow("42 400 400 1", " "),
                NumberDataItem.ParseOrThrow("42 600 600 1", " "),
                NumberDataItem.ParseOrThrow("42 800 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 200 200 1", " "),
                NumberDataItem.ParseOrThrow("43 400 400 1", " "),
                NumberDataItem.ParseOrThrow("43 600 600 1", " "),
                NumberDataItem.ParseOrThrow("43 800 800 1", " "),
                NumberDataItem.ParseOrThrow("44 0 200 1", " "),
            };

        var average = UsrednatorLogic.AverageFilter(input, 400, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 100,00 1,00
            42 400 500,00 1,00
            42 800 266,67 1,00
            43 200 300,00 1,00
            43 600 700,00 1,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_3()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 500 600 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 500 600 1", " "),
                NumberDataItem.ParseOrThrow("44 0 200 1", " "),
            };

        var average = UsrednatorLogic.AverageFilter(input, 100, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    
    [Test]
    public void Test_pivot_1()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 200 200 1", " "),
                NumberDataItem.ParseOrThrow("42 400 400 1", " "),
                NumberDataItem.ParseOrThrow("42 600 600 1", " "),
                NumberDataItem.ParseOrThrow("42 800 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 200 200 1", " "),
                NumberDataItem.ParseOrThrow("43 400 400 1", " "),
                NumberDataItem.ParseOrThrow("43 600 600 1", " "),
                NumberDataItem.ParseOrThrow("43 800 800 1", " "),
            };

        var average = UsrednatorLogic.AverageFilter(input, 400, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 100,00 1,00
            42 400 600,00 1,00
            43 0 66,67 1,00
            43 400 500,00 1,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_5()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 2.532793  2.875357", " "),
                NumberDataItem.ParseOrThrow("13 591 2.263264  2.278803", " "),
                NumberDataItem.ParseOrThrow("13 593 2.227859  3.141387", " "),
                NumberDataItem.ParseOrThrow("13 593 2.133996  2.680726", " "),
                NumberDataItem.ParseOrThrow("13 595 2.376947  2.87413", " "),
                NumberDataItem.ParseOrThrow("13 596 2.276814  2.061938", " "),
                NumberDataItem.ParseOrThrow("13 597 2.264396  2.655259", " "),
                NumberDataItem.ParseOrThrow("13 597 1.94501   2.576675", " "),
                NumberDataItem.ParseOrThrow("13 600 2.72      2.87", " "),

            };

        var average = UsrednatorLogic.AverageFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13  590  2.53  2.88
            13  591  2.26  2.28
            13  593  2.13  2.68
            13  597  1.95  2.58
            13  598  2.52  2.93
            13  599  3.11  2.62
            13  600  2.72  2.87
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_6()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 2.532793  2.875357", " "),
                NumberDataItem.ParseOrThrow("13 600 2.72      2.87", " "),
            };

        var average = UsrednatorLogic.AverageFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13  590  2.53  2.88
            13  591  2.26  2.28
            13  593  2.13  2.68
            13  597  1.95  2.58
            13  598  2.52  2.93
            13  599  3.11  2.62
            13  600  2.72  2.87

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
}