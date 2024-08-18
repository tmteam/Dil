using Dil.Core.Entities;
using Usrednator;

namespace UsrednatorTest;

public class InterpolateTest
{
    [Test]
    public void Test_1()
    {
        var interpolated = Helper.Interpolate(NumberDataItem.ParseOrThrow("13 590 1  3", " "),
            NumberDataItem.ParseOrThrow("13 600 2  4", " "), new Distance(13, 595));
        
        Assert.AreEqual(new Distance(13,595), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{1.5, 3.5}, interpolated.Data);        
    }
    
    [Test]
    public void Test_2()
    {
        var interpolated = Helper.Interpolate(
            NumberDataItem.ParseOrThrow("13 590 1  3", " "),
            NumberDataItem.ParseOrThrow("13 600 2  4", " "), 
            new Distance(13, 590));
        
        Assert.AreEqual(new Distance(13,590), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{1.0, 3.0}, interpolated.Data);        
    }
    
    [Test]
    public void Test_3()
    {
        var interpolated = Helper.Interpolate(
            NumberDataItem.ParseOrThrow("13 590 1  3 5", " "),
            NumberDataItem.ParseOrThrow("13 600 2  4 6", " "), 
            new Distance(13, 600));
        
        Assert.AreEqual(new Distance(13,600), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{2.0, 4.0, 6.0}, interpolated.Data);        
    }
    
    [Test]
    public void Test_4()
    {
        var interpolated = Helper.Interpolate(
            NumberDataItem.ParseOrThrow("13 600 1", " "),
            NumberDataItem.ParseOrThrow("14 400 3", " "), 
            new Distance(14, 0));
        
        Assert.AreEqual(new Distance(14,0), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{2.0}, interpolated.Data);        
    }
    
    [Test]
    public void Test_5()
    {
        var interpolated = Helper.Interpolate(
            NumberDataItem.ParseOrThrow("13 1200 1", " "),
            NumberDataItem.ParseOrThrow("14 400 3", " "), 
            new Distance(14, 0));
        
        Assert.AreEqual(new Distance(14,000), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{1.0}, interpolated.Data);        
    }
    
    [Test]
    public void Test_6()
    {
        var interpolated = Helper.Interpolate(
            NumberDataItem.ParseOrThrow("13 1000 1", " "),
            NumberDataItem.ParseOrThrow("14 200 3", " "), 
            new Distance(14, 0), 1200);
        
        Assert.AreEqual(new Distance(14,000), interpolated.Distance);
        CollectionAssert.AreEqual(new[]{2.0}, interpolated.Data);        
    }

}