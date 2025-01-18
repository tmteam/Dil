using Dil.Core;
using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

public class IntervalsTest
{
    [Test]
    public void Test_1() => AssertInterval(400, false,
    [
        Kmm(42, 0),
            Kmm(42, 200),
            Kmm(42, 400),
            Kmm(42, 600),
            Kmm(42, 800),
            Kmm(43, 000),
            Kmm(43, 0),
            Kmm(43, 200),
            Kmm(43, 400),
            Kmm(43, 600),
            Kmm(43, 800)
    ], [
            Kmm(42, 0),
            Kmm(42, 400),
            Kmm(42, 800),
            Kmm(43, 200),
            Kmm(43, 600)
        ]);

    [Test]
    public void Test_2() => AssertInterval(400, false,
    [
        Kmm(42, 0),  
        Kmm(42, 200),
        Kmm(42, 400),
        Kmm(42, 600),
        Kmm(42, 800),
        Kmm(43, 000),
        Kmm(43, 0 ), 
        Kmm(43, 200),
        Kmm(43, 400),
        Kmm(43, 600),
        Kmm(43, 800),
        Kmm(44, 000)
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 200),
        Kmm(43, 600),
        Kmm(44, 000)
    ]);
    
    [Test]
    public void Test_3() => AssertInterval(100, false,
    [
        Kmm(42, 0),  
        Kmm(42, 500),
        Kmm(43, 0),
        Kmm(43, 500),
        Kmm(44, 00),
    ], [
        Kmm(42, 0),
        Kmm(42, 100),
        Kmm(42, 200),
        Kmm(42, 300),
        Kmm(42, 400),
        Kmm(42, 500),
        Kmm(42, 600),
        Kmm(42, 700),
        Kmm(42, 800),
        Kmm(42, 900),
        Kmm(43, 0),
        Kmm(43, 100),
        Kmm(43, 200),
        Kmm(43, 300),
        Kmm(43, 400),
        Kmm(43, 500),
        Kmm(43, 600),
        Kmm(43, 700),
        Kmm(43, 800),
        Kmm(43, 900),
        Kmm(44, 000),
    ]);
    
    [Test]
    public void Test_4() => AssertInterval(5, false,
    [
        Kmm(13, 600),  
        Kmm(13, 602),
        Kmm(13, 604),
        Kmm(13, 606),
        Kmm(13, 608),
        Kmm(13, 610),
    ], [
        Kmm(13, 600),
        Kmm(13, 605),
        Kmm(13, 610),
    ]);
    
    [Test]
    public void Test_5() => AssertInterval(250, false,
    [
        Kmm(42, 0),  
        Kmm(42, 500),
        Kmm(43, 0),
        Kmm(43, 500),
        Kmm(44, 0),
    ], [
        Kmm(42, 0),
        Kmm(42, 250),
        Kmm(42, 500),
        Kmm(42, 750),
        Kmm(43, 0),
        Kmm(43, 250),
        Kmm(43, 500),
        Kmm(43, 750),
        Kmm(44, 000),
    ]);
    
    [Test]
    public void Test_6() => AssertInterval(400, false,
    [
        Kmm(42, 0),
        Kmm(42, 200),
        Kmm(42, 400),
        Kmm(42, 600),
        Kmm(42, 800),
        Kmm(43, 000),
        Kmm(43, 0),
        Kmm(43, 200),
        Kmm(43, 400),
        Kmm(43, 600),
        Kmm(43, 800)
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 200),
        Kmm(43, 600),
    ]);
    
    [Test]
    public void Test_7() => AssertInterval(250, false,
    [
        Kmm(42, 500),
        Kmm(43, 0),
        Kmm(43, 500),
    ], [
        Kmm(42, 500),
        Kmm(42, 750),
        Kmm(43, 0),
        Kmm(43, 250),
        Kmm(43, 500),
    ]);

    [Test]
    public void Test_bad_km_1() => AssertInterval(100, false,
        [
            Kmm(13,800),
            Kmm(13,900),
            Kmm(13,1000),
            Kmm(13,1100),
            Kmm(14,000),
            Kmm(14,100),
        ],
        [
            Kmm(13,800),
            Kmm(13,900),
            Kmm(13,1000),
            Kmm(13,1100),
            Kmm(14,000),
            Kmm(14,100),
        ]
    );
    
    [Test]

    public void Test_bad_km_2() => AssertInterval(1, false,
        [
            Kmm(1,970),
            Kmm(1,973),
            Kmm(2,000),
            Kmm(2,003),
        ],
        [
            Kmm(1,970),
            Kmm(1,971),
            Kmm(1,972),
            Kmm(1,973),
            Kmm(2,0),
            Kmm(2,1),
            Kmm(2,2),
            Kmm(2,3),
        ]
    );
    
    [Test]
    public void Sample_1() => AssertInterval(4, false,
    [
        Kmm(5, 1004),  
        Kmm(5, 1006),
        Kmm(5, 1006),
        Kmm(5, 1006),
        Kmm(5, 1006),
        Kmm(5, 1006),
        Kmm(6, 0),
        Kmm(6, 0),
        Kmm(6, 1),
        Kmm(6, 2),
        Kmm(6, 3),
        Kmm(6, 3),
        Kmm(6, 3),
        Kmm(6, 4),
    ], [
        Kmm(5, 1004),
        Kmm(6, 0),
        Kmm(6, 4),
    ]);
    
    [Test]
    public void Pivot_1() => AssertInterval(400, true,
    [
        Kmm(42, 0),
        Kmm(42, 200),
        Kmm(42, 400),
        Kmm(42, 600),
        Kmm(42, 800),
        Kmm(43, 000),
        Kmm(43, 0),
        Kmm(43, 200),
        Kmm(43, 400),
        Kmm(43, 600),
        Kmm(43, 800)
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 0),
        Kmm(43, 400),
        Kmm(43, 800)

    ]);
    
    [Test]
    public void Pivot_2() => AssertInterval(400, true,
    [
        Kmm(42, 0),
        Kmm(42, 800),
        Kmm(43, 000),
        Kmm(43, 0),
        Kmm(43, 200),
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 0),
    ]);
    
    [Test]
    public void Pivot_3() => AssertInterval(400, true,
    [
        Kmm(42, 0),
        Kmm(42, 800),
        Kmm(43, 200),
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 0),
    ]);
    
    [Test]
    public void Pivot_4() => AssertInterval(400, true,
    [
        Kmm(42, 0),
        Kmm(43, 0),
        Kmm(44, 0),
    ], [
        Kmm(42, 0),
        Kmm(42, 400),
        Kmm(42, 800),
        Kmm(43, 0),
        Kmm(43, 400),
        Kmm(43, 800),
        Kmm(44, 0),

    ]);
    
    [Test]
    public void Pivot_5() => AssertInterval(500, true,
    [
        Kmm(42, 0),
        Kmm(42, 500),
    ], [
        Kmm(42, 0),
        Kmm(42, 500),
    ]);
    
    [Test]
    public void Pivot_6() => AssertInterval(100, true,
    [
        Kmm(13, 800),
        Kmm(13, 901),
        Kmm(13, 1004),
        Kmm(13, 1100),
        Kmm(14, 100),
    ], [
        Kmm(13, 800),
        Kmm(13, 900),
        Kmm(13, 1000),
        Kmm(13, 1100),
        Kmm(14, 0),
        Kmm(14, 100),
    ]);

    [Test]
    public void Pivot_8() => AssertInterval(2, true,
    [
        Kmm(13, 996),
        Kmm(13, 1006),
        Kmm(14, 6),
    ], [
        Kmm(13, 996),
        Kmm(13, 998),
        Kmm(13, 1000),
        Kmm(13, 1002),
        Kmm(13, 1004),
        Kmm(13, 1006),
        Kmm(14, 0),
        Kmm(14, 2),
        Kmm(14, 4),
        Kmm(14, 6),
    ]);

    [Test]
    public void Pivot_7() => AssertInterval(1, true,
    [
        Kmm(13, 990),
        Kmm(13, 1010),
        Kmm(14, 10),
    ], [
        Kmm(13, 990),
        Kmm(13, 991),
        Kmm(13, 992),
        Kmm(13, 993),
        Kmm(13, 994),
        Kmm(13, 995),
        Kmm(13, 996),
        Kmm(13, 997),
        Kmm(13, 998),
        Kmm(13, 999),
        Kmm(13, 1000),
        Kmm(13, 1001),
        Kmm(13, 1002),
        Kmm(13, 1003),
        Kmm(13, 1004),
        Kmm(13, 1005),
        Kmm(13, 1006),
        Kmm(13, 1007),
        Kmm(13, 1008),
        Kmm(13, 1009),
        Kmm(13, 1010),
        Kmm(14, 0),
        Kmm(14, 1),
        Kmm(14, 2),
        Kmm(14, 3),
        Kmm(14, 4),
        Kmm(14, 5),
        Kmm(14, 6),
        Kmm(14, 7),
        Kmm(14, 8),
        Kmm(14, 9),
        Kmm(14, 10),

    ]);


    private void AssertInterval(int step, bool useZeroPivot,  Distance[] origin, Distance[] expected)
    {
        var result = UsrednatorLogic.CalculateIntervals(origin, step, useZeroPivot);
        
        /*
        var dataInput = origin.Select(x => new NumberDataItem(x, [0.0])).ToArray();
        var average = UsrednatorLogic.ApproximationFilter(
            dataInput, 
            step, 
            useZeroPivot);
        var result = average.Select(a => a.Distance).ToArray();
        */
        var expectedStr = expected.Select(a => new NumberDataItem(a, [0.0])).ToFormattedString();
        var actualStr = result.Select(a => new NumberDataItem(a, [0.0])).ToFormattedString();
        Assert.AreEqual(expectedStr,actualStr);
    }
    

    public Distance Kmm(int km, int m)
    {
        return new Distance(km, m);
    }
}