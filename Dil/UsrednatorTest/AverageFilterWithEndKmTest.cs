using Dil.Core;
using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

public class AverageFilterWithEndKmTest
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
            UsrednatorLogic.ApproximationFilter(input, 400, false, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 200,00 1,00
            42 400 600,00 1,00
            42 800 250,62 1,00
            43 200 400,00 1,00
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

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 60,00 1,00
            42 100 180,00 1,00
            42 200 300,00 1,00
            42 300 420,00 1,00
            42 400 540,00 1,00
            42 500 540,00 1,00
            42 600 420,00 1,00
            42 700 300,00 1,00
            42 800 180,00 1,00
            42 900 60,00 1,00
            43 0 60,00 1,00
            43 100 180,00 1,00
            43 200 300,00 1,00
            43 300 420,00 1,00
            43 400 540,00 1,00
            43 500 560,00 1,00
            43 600 480,00 1,00
            43 700 400,00 1,00
            43 800 320,00 1,00
            43 900 240,00 1,00
            44 0 200,00 1,00

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

        var average = UsrednatorLogic.ApproximationFilter(input, 5, false, true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 590 1,50
            13 595 2,50
            13 600 3,00

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

        var average = UsrednatorLogic.ApproximationFilter(input, 100, false,true);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 800 8,50 0,00
            13 900 9,50 0,00
            13 1000 10,50 0,00
            13 1100 11,50 0,00
            13 1200 14,00 0,00
            14 0 16,50 0,00
            14 100 17,50 0,00
            14 200 18,00 0,00

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

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 400, true, true );
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 200,00 1,00
            42 400 600,00 1,00
            42 800 400,00 1,00
            43 0 200,00 1,00
            43 400 600,00 1,00
            43 800 800,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
}