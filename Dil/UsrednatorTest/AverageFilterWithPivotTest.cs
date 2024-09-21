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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 402,00 1,00
            43 0 199,50 1,00
            43 400 599,50 1,00
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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 402,00 1,00
            43 0 100,00 1,00

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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 650,75 1,00
            43 0 350,00 1,00

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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 899,50 1,00
            43 0 1199,50 1,00
            43 400 1599,50 1,00
            43 800 1899,50 1,00
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
            42 0 249,50 1,00
            42 500 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_pivot_6()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("5\t1004\t3191,1 "),
                NumberDataItem.ParseOrThrow("5\t1006\t4047,5 "),
                NumberDataItem.ParseOrThrow("5\t1006\t4500,2 "),
                NumberDataItem.ParseOrThrow("5\t1006\t5467,9 "),
                NumberDataItem.ParseOrThrow("5\t1006\t6655,0 "),
                NumberDataItem.ParseOrThrow("5\t1006\t5882,1 "),
                NumberDataItem.ParseOrThrow("6\t0   \t5301,7 "),
                NumberDataItem.ParseOrThrow("6\t0   \t8972,9 "),
                NumberDataItem.ParseOrThrow("6\t1   \t13340,6"),
                NumberDataItem.ParseOrThrow("6\t2   \t14748,3"),
                NumberDataItem.ParseOrThrow("6\t3   \t14657,5"),
                NumberDataItem.ParseOrThrow("6\t3   \t1"),
                NumberDataItem.ParseOrThrow("6\t3   \t2"),
                NumberDataItem.ParseOrThrow("6\t4   \t6"),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 4, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            5 1004 47440,95
            6 0 100280,33
            6 4 6,00

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
            13 800 8,49 0,00
            13 900 9,50 0,00
            13 1000 10,99 0,00
            13 1100 12,50 0,00
            14 0 13,49 0,00
            14 100 14,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }

}