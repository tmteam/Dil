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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 250,75 1,00
            43 200 399,50 1,00
            43 600 700,00 1,00
            
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
            42 0 199,50 1,00
            42 400 599,50 1,00
            42 800 250,75 1,00
            43 200 399,50 1,00
            43 600 600,50 1,00
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
            42 0 59,40 1,00
            42 100 179,40 1,00
            42 200 299,40 1,00
            42 300 419,40 1,00
            42 400 539,40 1,00
            42 500 540,60 1,00
            42 600 420,60 1,00
            42 700 300,60 1,00
            42 800 180,60 1,00
            42 900 60,60 1,00
            43 0 59,40 1,00
            43 100 179,40 1,00
            43 200 299,40 1,00
            43 300 419,40 1,00
            43 400 539,40 1,00
            43 500 560,40 1,00
            43 600 480,40 1,00
            43 700 400,40 1,00
            43 800 320,40 1,00
            43 900 240,40 1,00
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
            13 592 221,75 25,25
            13 593 217,50 28,50
            13 594 227,25 28,25
            13 595 237,00 28,00
            13 596 227,00 20,00
            13 597 210,00 25,50
            13 598 230,67 112,67
            13 599 251,33 199,83
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
            13 590 1,40
            13 595 2,40
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
            13 590 1,90
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
            13 600 2,00
            13 605 7,00
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
            13 600 2,00
            13 605 7,00
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
            42 0 124,50 1,00
            42 250 374,50 1,00
            42 500 375,50 1,00
            42 750 125,50 1,00
            43 0 124,50 1,00
            43 250 374,50 1,00
            43 500 375,50 1,00
            43 750 125,50 1,00
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
            42 0 249,50 1,00
            42 500 250,50 1,00
            43 0 249,50 1,00
            43 500 250,50 1,00
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
            42 500 375,50 1,00
            42 750 125,50 1,00
            43 0 124,50 1,00
            43 250 374,50 1,00
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
            42 750 125,50 1,00
            43 0 124,50 1,00
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
            13 592 2,25 2,00
            13 593 3,50 2,00

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
            42 500 375,50 1,00
            42 750 125,50 1,00
            43 0 124,50 1,00
            43 250 374,50 1,00
            43 500 500,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_18()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("1 0 10", " "),
                NumberDataItem.ParseOrThrow("1 1 20", " "),
                NumberDataItem.ParseOrThrow("1 2 40", " "),
                NumberDataItem.ParseOrThrow("1 4 10", " "),
                NumberDataItem.ParseOrThrow("1 5 20", " "),
                NumberDataItem.ParseOrThrow("1 8 30", " "),
                NumberDataItem.ParseOrThrow("1 9 40", " "),
                NumberDataItem.ParseOrThrow("1 10 50", " "),
                NumberDataItem.ParseOrThrow("1 11 60", " "),
                NumberDataItem.ParseOrThrow("1 12 40", " "),
            };

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            1 0 10,00
            1 1 20,00
            1 2 40,00
            1 3 25,00
            1 4 10,00
            1 5 20,00
            1 6 23,33
            1 7 26,67
            1 8 30,00
            1 9 40,00
            1 10 50,00
            1 11 60,00
            1 12 40,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_19()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("1 0 10", " "),
                NumberDataItem.ParseOrThrow("1 1 20", " "),
                NumberDataItem.ParseOrThrow("1 2 40", " "),
                NumberDataItem.ParseOrThrow("1 4 10", " "),
                NumberDataItem.ParseOrThrow("1 5 20", " "),
                NumberDataItem.ParseOrThrow("1 8 30", " "),
                NumberDataItem.ParseOrThrow("1 9 40", " "),
                NumberDataItem.ParseOrThrow("1 10 50", " "),
                NumberDataItem.ParseOrThrow("1 11 60", " "),
                NumberDataItem.ParseOrThrow("1 12 40", " "),
            };
      
        var average = 
            UsrednatorLogic.ApproximationFilter(input, 4, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            1 0 23,75
            1 4 20,00
            1 8 45,00
            1 12 40,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_20()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 2 200 1", " "),
                NumberDataItem.ParseOrThrow("42 4 400 1", " "),
                NumberDataItem.ParseOrThrow("42 6 600 1", " "),
                NumberDataItem.ParseOrThrow("42 8 800 1", " "),
            };

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 4, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 150,00 1,00
            42 4 550,00 1,00
            42 8 800,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_21()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 0   0   1", " "),
                NumberDataItem.ParseOrThrow("42 20 200 1", " "),
                NumberDataItem.ParseOrThrow("42 40 400 1", " "),
                NumberDataItem.ParseOrThrow("42 60 600 1", " "),
                NumberDataItem.ParseOrThrow("42 80 800 1", " "),
            };

        var average = 
            UsrednatorLogic.ApproximationFilter(input, 40, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 0 195,00 1,00
            42 40 595,00 1,00
            42 80 800,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_22()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 940 400 1", " "),
                NumberDataItem.ParseOrThrow("42 960 600 1", " "),
                NumberDataItem.ParseOrThrow("42 980 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 20 200 1", " "),
                NumberDataItem.ParseOrThrow("43 40 400 1", " "),
                NumberDataItem.ParseOrThrow("43 60 600 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 40, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 940 595,00 1,00
            42 980 257,50 1,00
            43 20 395,00 1,00
            43 60 600,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    
    [Test]
    public void Test_23()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("42 994 400 1", " "),
                NumberDataItem.ParseOrThrow("42 996 600 1", " "),
                NumberDataItem.ParseOrThrow("42 998 800 1", " "),
                NumberDataItem.ParseOrThrow("43 000 000 1", " "),
                NumberDataItem.ParseOrThrow("43 002 200 1", " "),
                NumberDataItem.ParseOrThrow("43 004 400 1", " "),
                NumberDataItem.ParseOrThrow("43 006 600 1", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 4, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            42 994 550,00 1,00
            42 998 325,00 1,00
            43 2 350,00 1,00
            43 6 600,00 1,00

            """,
            formated,
            $"Actual is \r\n{formated}");
    }
    /*
     *
     * 5	1004	3191,1	3,7	1182,1	786,7	1392,3	2,6	725,2	391,9
5	1006	4047,5	4,7	1152,1	773,4	1172,9	2,9	460,4	496,5
5	1006	4500,2	5,4	888,5	840,8	1292,0	2,5	501,6	574,9
5	1006	5467,9	6,6	870,3	869,1	980,0	2,3	401,2	452,1
5	1006	6655,0	8,6	754,3	801,3	1525,5	3,0	619,3	389,9
5	1006	5882,1	8,0	689,0	699,4	2705,0	4,1	732,8	523,0
6	0	5301,7	7,3	562,7	784,6	3040,2	4,7	781,7	599,3
6	0	8972,9	9,6	1058,2	843,1	2885,9	4,6	547,9	582,7
6	1	13340,6	13,3	1112,1	795,9	3958,9	7,6	765,0	446,0
6	2	14748,3	15,2	1141,5	771,9	4016,1	7,7	755,1	443,4
6	3	14657,5	15,0	1192,3	705,2	4239,1	9,1	649,5	494,3
6	3	13828,3	14,5	1138,2	758,6	4475,1	9,2	509,5	481,5
6	3	12876,5	14,2	1111,0	802,0	4516,8	8,6	866,7	414,9
6	4	9626,1	13,4	1014,5	734,6	4529,0	8,9	664,5	466,6
6	4	11077,9	13,3	1168,1	660,4	4299,9	8,2	816,5	445,3
     */
    
    [Test]
    public void Test_24()
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
    public void Test_25()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("5 0 1"),
                NumberDataItem.ParseOrThrow("5 0 2"),
                NumberDataItem.ParseOrThrow("5 0 3"),
                NumberDataItem.ParseOrThrow("5 1 2"),
                NumberDataItem.ParseOrThrow("5 1 3"),
                NumberDataItem.ParseOrThrow("5 1 4"),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 1, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            5 0 2,00
            5 1 3,00
            
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
            13 800 8,49 0,00
            13 900 9,50 0,00
            13 1000 10,49 0,00
            13 1100 11,50 0,00
            13 1200 12,50 0,00
            13 1300 13,49 0,00
            13 1400 14,49 0,00
            13 1500 15,50 0,00
            14 0 16,50 0,00
            14 100 17,50 0,00
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
            13 800 8,49 0,00
            13 900 9,50 0,00
            13 1000 10,49 0,00
            13 1100 11,50 0,00
            13 1200 12,25 0,00
            13 1300 12,75 0,00
            13 1400 13,25 0,00
            13 1500 13,75 0,00
            14 0 14,49 0,00
            14 100 15,50 0,00
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
    
    [Test]
    public void Test_bad_km_4()
    {
        var input =
            new[]
            {
                NumberDataItem.ParseOrThrow("13 998 8  0", " "),
                NumberDataItem.ParseOrThrow("13 999 9  0", " "),
                NumberDataItem.ParseOrThrow("13 1000 10  0", " "),
                NumberDataItem.ParseOrThrow("13 1001 12  0", " "),
                NumberDataItem.ParseOrThrow("13 1002 12  0", " "),
                NumberDataItem.ParseOrThrow("13 1003 12  0", " "),
                NumberDataItem.ParseOrThrow("14 000 13  0", " "),
                NumberDataItem.ParseOrThrow("14 001 14  0", " "),
                NumberDataItem.ParseOrThrow("14 002 13  0", " "),
                NumberDataItem.ParseOrThrow("14 003 14  0", " "),
            };

        var average = UsrednatorLogic.ApproximationFilter(input, 2, false);
        var formated = average.ToFormattedString(" ");
        Assert.AreEqual(
            """
            13 998 8,50 0,00
            13 1000 11,00 0,00
            13 1002 12,00 0,00
            14 0 13,50 0,00
            14 2 13,50 0,00
            
            """,
            formated,
            $"Actual is \r\n{formated}");
    }

}