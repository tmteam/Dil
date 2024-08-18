using System.Diagnostics.CodeAnalysis;
using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

[SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class AverageFilterWithPivotTest
{
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

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 400, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 0 0,00 1,00
            43 400 400,00 1,00
            43 800 800,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_2()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 800 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 200 200 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 400, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 0 0,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_3()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 800 800 1", " "),
                NumberDataItem.ParseOrThrow("43 200 200 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 400, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 0 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_4()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0 0   1", " "),
                NumberDataItem.ParseOrThrow("43 0 1000 1", " "),
                NumberDataItem.ParseOrThrow("44 0 2000 1", " "),

            };

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 400, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 0 1000,00 1,00
            43 400 1400,00 1,00
            43 800 1800,00 1,00
            44 0 2000,00 1,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_5()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0 0   1", " "),
                NumberDataItem.ParseOrThrow("42 500 500 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 500, true);
        var formated = 
            average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 500 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_bad_km_1()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 800 8  0", " "),
                NumberDataItem.ParseOrThrow("13 900 9  0", " "),
                NumberDataItem.ParseOrThrow("13 1000 10  0", " "),
                NumberDataItem.ParseOrThrow("13 1100 12  0", " "),
                NumberDataItem.ParseOrThrow("14 000 13  0", " "),
                NumberDataItem.ParseOrThrow("14 100 14  0", " "),

            };

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 800 8,00 0,00
            13 900 9,00 0,00
            13 1000 10,00 0,00
            13 1100 12,00 0,00
            14 0 13,00 0,00
            14 100 14,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }

}