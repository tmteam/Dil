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
}