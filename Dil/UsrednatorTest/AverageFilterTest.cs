using System.Diagnostics.CodeAnalysis;
using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

[SuppressMessage("Assertion",
    "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class AverageFilterNoPivotTest
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

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 400, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 200 200,00 1,00
            43 600 600,00 1,00
            
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
                NumberDataItem.ParseOrThrow("44 0   200 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 400, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 400 400,00 1,00
            42 800 800,00 1,00
            43 200 200,00 1,00
            43 600 600,00 1,00
            44 0 200,00 1,00
            
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

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 100 120,00 1,00
            42 200 240,00 1,00
            42 300 360,00 1,00
            42 400 480,00 1,00
            42 500 600,00 1,00
            42 600 480,00 1,00
            42 700 360,00 1,00
            42 800 240,00 1,00
            42 900 120,00 1,00
            43 0 0,00 1,00
            43 100 120,00 1,00
            43 200 240,00 1,00
            43 300 360,00 1,00
            43 400 480,00 1,00
            43 500 600,00 1,00
            43 600 520,00 1,00
            43 700 440,00 1,00
            43 800 360,00 1,00
            43 900 280,00 1,00
            44 0 200,00 1,00

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
                NumberDataItem.ParseOrThrow("13 590 253  28", " "),
                NumberDataItem.ParseOrThrow("13 591 226  22", " "),
                NumberDataItem.ParseOrThrow("13 593 222  31", " "),
                NumberDataItem.ParseOrThrow("13 593 213  26", " "),
                NumberDataItem.ParseOrThrow("13 595 237  28", " "),
                NumberDataItem.ParseOrThrow("13 596 227  20", " "),
                NumberDataItem.ParseOrThrow("13 597 226  26", " "),
                NumberDataItem.ParseOrThrow("13 597 194  25", " "),
                NumberDataItem.ParseOrThrow("13 600 272  287", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 253,00 28,00
            13 591 226,00 22,00
            13 592 224,00 26,50
            13 593 222,00 31,00
            13 594 225,00 27,00
            13 595 237,00 28,00
            13 596 227,00 20,00
            13 597 226,00 26,00
            13 598 220,00 112,33
            13 599 246,00 199,67
            13 600 272,00 287,00
            
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

        var average = UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 2,53 2,88
            13 591 2,55 2,87
            13 592 2,57 2,87
            13 593 2,59 2,87
            13 594 2,61 2,87
            13 595 2,63 2,87
            13 596 2,65 2,87
            13 597 2,66 2,87
            13 598 2,68 2,87
            13 599 2,70 2,87
            13 600 2,72 2,87

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_7()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 1", " "),
                NumberDataItem.ParseOrThrow("13 600 3", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 5, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 1,00
            13 595 2,00
            13 600 3,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_8()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 1", " "),
                NumberDataItem.ParseOrThrow("13 600 3", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 10, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 1,00
            13 600 3,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_9()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 600 0", " "),
                NumberDataItem.ParseOrThrow("13 601 1", " "),
                NumberDataItem.ParseOrThrow("13 602 2", " "),
                NumberDataItem.ParseOrThrow("13 603 3", " "),
                NumberDataItem.ParseOrThrow("13 604 4", " "),
                NumberDataItem.ParseOrThrow("13 605 5", " "),
                NumberDataItem.ParseOrThrow("13 606 6", " "),
                NumberDataItem.ParseOrThrow("13 607 7", " "),
                NumberDataItem.ParseOrThrow("13 608 8", " "),
                NumberDataItem.ParseOrThrow("13 609 9", " "),
                NumberDataItem.ParseOrThrow("13 610 10", " "),

            };

        var average = UsrednatorLogic.ApproximationFilter(input, 5, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 600 0,00
            13 605 5,00
            13 610 10,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_10()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 600 0", " "),
                NumberDataItem.ParseOrThrow("13 602 2", " "),
                NumberDataItem.ParseOrThrow("13 604 4", " "),
                NumberDataItem.ParseOrThrow("13 606 6", " "),
                NumberDataItem.ParseOrThrow("13 608 8", " "),
                NumberDataItem.ParseOrThrow("13 610 10", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 5, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 600 0,00
            13 605 5,00
            13 610 10,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_11()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 500 500 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 500 500 1", " "),
                NumberDataItem.ParseOrThrow("44 0 0 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 250, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 250 250,00 1,00
            42 500 500,00 1,00
            42 750 250,00 1,00
            43 0 0,00 1,00
            43 250 250,00 1,00
            43 500 500,00 1,00
            43 750 250,00 1,00
            44 0 0,00 1,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_12()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 500 500 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 500 500 1", " "),
                NumberDataItem.ParseOrThrow("44 0 0 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 500, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 0,00 1,00
            42 500 500,00 1,00
            43 0 0,00 1,00
            43 500 500,00 1,00
            44 0 0,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_13()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 500 500 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 500 500 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 250, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 500 500,00 1,00
            42 750 250,00 1,00
            43 0 0,00 1,00
            43 250 250,00 1,00
            43 500 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_14()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 750 250 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 250 250 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 250, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 750 250,00 1,00
            43 0 0,00 1,00
            43 250 250,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_15()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 0  3", " "),
                NumberDataItem.ParseOrThrow("13 591 1  2", " "),
                NumberDataItem.ParseOrThrow("13 593 3  0", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 0,00 3,00
            13 591 1,00 2,00
            13 592 2,00 1,00
            13 593 3,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_16()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 590 0  3", " "),
                NumberDataItem.ParseOrThrow("13 591 1  2", " "),
                NumberDataItem.ParseOrThrow("13 593 3  0", " "),
                NumberDataItem.ParseOrThrow("13 593 4  4", " "),

            };

        var average = UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 0,00 3,00
            13 591 1,00 2,00
            13 592 2,00 1,00
            13 593 3,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_17()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 500 500 1", " "),
                NumberDataItem.ParseOrThrow("43 0   0   1", " "),
                NumberDataItem.ParseOrThrow("43 500 500 1", " "),
            };

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 250, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 500 500,00 1,00
            42 750 250,00 1,00
            43 0 0,00 1,00
            43 250 250,00 1,00
            43 500 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_bad_km_1()
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
    
    [Test]
    public void Test_bad_km_2()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 800 8  0", " "),
                NumberDataItem.ParseOrThrow("13 1200 12  0", " "),
                NumberDataItem.ParseOrThrow("14 000 16  0", " "),
                NumberDataItem.ParseOrThrow("14 200 18  0", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 800 8,00 0,00
            13 900 9,00 0,00
            13 1000 10,00 0,00
            13 1100 11,00 0,00
            13 1200 12,00 0,00
            13 1300 13,00 0,00
            13 1400 14,00 0,00
            13 1500 15,00 0,00
            14 0 16,00 0,00
            14 100 17,00 0,00
            14 200 18,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_bad_km_3()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 800 8  0", " "),
                NumberDataItem.ParseOrThrow("13 1200 12  0", " "),
                NumberDataItem.ParseOrThrow("14 000 14  0", " "),
                NumberDataItem.ParseOrThrow("14 200 16  0", " "),
                NumberDataItem.ParseOrThrow("14 800 16  0", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 800 8,00 0,00
            13 900 9,00 0,00
            13 1000 10,00 0,00
            13 1100 11,00 0,00
            13 1200 12,00 0,00
            13 1300 12,50 0,00
            13 1400 13,00 0,00
            13 1500 13,50 0,00
            14 0 14,00 0,00
            14 100 15,00 0,00
            14 200 16,00 0,00
            14 300 16,00 0,00
            14 400 16,00 0,00
            14 500 16,00 0,00
            14 600 16,00 0,00
            14 700 16,00 0,00
            14 800 16,00 0,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
}