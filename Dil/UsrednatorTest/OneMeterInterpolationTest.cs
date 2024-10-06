using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

public class OneMeterInterpolationTest
{
    [Test]
    public void Test_1() =>
        AssertInterpolation(
            [
                NumberDataItem.ParseOrThrow("13 0 1"),
                NumberDataItem.ParseOrThrow("13 2 3")
            ],
            [
                NumberDataItem.ParseOrThrow("13 0 1"),
                NumberDataItem.ParseOrThrow("13 1 2"),
                NumberDataItem.ParseOrThrow("13 2 3")
            ]
        );

    [Test]
    public void Test_2() => AssertInterpolation(
        [
            NumberDataItem.ParseOrThrow("13 998  1"),
            NumberDataItem.ParseOrThrow("13 1001 4"),
            NumberDataItem.ParseOrThrow("14 001 6")
        ],
        [
            NumberDataItem.ParseOrThrow("13 998 1"),
            NumberDataItem.ParseOrThrow("13 999 2"),
            NumberDataItem.ParseOrThrow("13 1000 3"),
            NumberDataItem.ParseOrThrow("13 1001 4"),
            NumberDataItem.ParseOrThrow("14 000 5"),
            NumberDataItem.ParseOrThrow("14 001 6")
        ]
    );

    [Test]
    public void Test_3()
    {
        AssertInterpolation([
                NumberDataItem.ParseOrThrow("5 1004 1"),
                NumberDataItem.ParseOrThrow("5 1006 3"),
                NumberDataItem.ParseOrThrow("6 1   5"),
                NumberDataItem.ParseOrThrow("6 2   6")
            ],
            [
                NumberDataItem.ParseOrThrow("5 1004 1"),
                NumberDataItem.ParseOrThrow("5 1005 2"),
                NumberDataItem.ParseOrThrow("5 1006 3"),
                NumberDataItem.ParseOrThrow("6 0   4 "),
                NumberDataItem.ParseOrThrow("6 1   5"),
                NumberDataItem.ParseOrThrow("6 2   6")
            ]);
    }
    
    [Test]
    public void Test_4()
    {
        AssertInterpolation([
                NumberDataItem.ParseOrThrow("5 999 1"),
                NumberDataItem.ParseOrThrow("5 1001 1"),
                NumberDataItem.ParseOrThrow("6 1   1"),
            ],
            [
                NumberDataItem.ParseOrThrow("5 999 1"),
                NumberDataItem.ParseOrThrow("5 1000 1"),
                NumberDataItem.ParseOrThrow("5 1001 1"),
                NumberDataItem.ParseOrThrow("6 0   1"),
                NumberDataItem.ParseOrThrow("6 1   1"),
            ]);
    }

    void AssertInterpolation(NumberDataItem[] origin, NumberDataItem[] result)
    {
        var actual = UsrednatorLogic.OneMeterInterpolation(origin);

        var actualAsStr = actual.ToFormattedString();
        var expectedAsStr = result.ToFormattedString();
        Console.WriteLine($"Actual:\r\n{actualAsStr}");
        Assert.AreEqual(expectedAsStr, actualAsStr);
    }
}