using System.Diagnostics.CodeAnalysis;
using Dil.Core.Entities;

namespace UsrednatorTest;

[SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class InputParseTest
{
    [Test]
    public void Input_1()
    {
        var result = NumberDataItem.ParseOrThrow("42\t300\t100500");
        Assert.AreEqual(42, result.Distance.Km);
        Assert.AreEqual(300, result.Distance.M);
        Assert.AreEqual(100500.0, result.Data[0]);
    }
    
    [Test]
    public void Input_2()
    {
        var result = NumberDataItem.ParseOrThrow("42\t300\t100500\t1");
        Assert.AreEqual(42, result.Distance.Km);
        Assert.AreEqual(300, result.Distance.M);
        Assert.AreEqual(100500.0, result.Data[0]);
        Assert.AreEqual(1.0, result.Data[1]);
    }
    
    [Test]
    public void Input_3()
    {
       var result =  NumberDataItem.ParseOrThrow(
"""
6  0  5301.7  7.3  562.7  784.6  3040.2  4.7  781.7  599.3
"""," ");
      Console.WriteLine(result);  
    }
}