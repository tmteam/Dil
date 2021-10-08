using System.Collections.Generic;
using NUnit.Framework;

namespace Pereverter.Tests
{
    public class Tests
    {
        [Test]
        public void ReversedLenghts_GetReversed_zeroMeter()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{4, 1000}, {3, 900},{2, 800} });

            var (km,m) = lengths.GetReversed(3, 0);
            Assert.AreEqual(0,m);
            Assert.AreEqual(3,km);
        }
        
        [Test]
        public void ReversedLenghts_GetReversed_KmBeforeData()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{4, 1000}, {3, 900},{2, 800} });

            var (km,m) = lengths.GetReversed(1, 100);
            //Нет информации о километре 0-1. Считается 1000м. 
            Assert.AreEqual(900,m);
            Assert.AreEqual(0,km);
        }
        
        [Test]
        public void GetReversed_zeroMeter()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1000}});

            var (km,m) = lengths.GetReversed(3, 0);
            Assert.AreEqual(0,m);
            Assert.AreEqual(3,km);
        }
        
        [Test]
        public void GetReversed_KmBeforeData()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1000}});

            var (km,m) = lengths.GetReversed(1, 100);
            //Нет информации о километре 0-1. Считается 1000м. 
            Assert.AreEqual(900,m);
            Assert.AreEqual(0,km);
        }
        [Test]
        public void GetReversed_MiddleKm()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1100}});

            var (km,m) = lengths.GetReversed(4, 300);
            //3-4: 900
            Assert.AreEqual(600,m);
            Assert.AreEqual(3,km);
        }
        [Test]
        public void GetReversed_LastKm()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1100}});

            var (km,m) = lengths.GetReversed(5, 300);
            //4-5: 1100
            Assert.AreEqual(800,m);
            Assert.AreEqual(4,km);
        }
        [Test]
        public void GetReversed_NextKmAfterData()
        {
            var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1100}});

            var (km,m) = lengths.GetReversed(6, 300);
            //Нет информации о 5-6 участке. Считаем как 1000
            Assert.AreEqual(700,m);
            Assert.AreEqual(5,km);
        }
        //[TestCase(54,468,53,531)]
        [TestCase(58,1,    57,1008)]
        [TestCase(58,2,    57,1007)]
        [TestCase(58,0,    58,0)]
        [TestCase(58,1008, 57,1)]
        [TestCase(57,0,    57,0)]
        [TestCase(58,448,  57,561)]
        public void ReversedLengths_GetReversed_RealLengths(int kmOrigin, int mOrigin, int kmReversed, int mReversed)
        {
            var (km,m) = _realLengths.GetReversed(kmOrigin,mOrigin);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(kmReversed, km,"km");
                Assert.AreEqual(mReversed, m  ,"m");
            });
        }

        private Lengths _realLengths= Lengths.Create(new Dictionary<int, int> {
            { 72,	935},
            { 71,	975},
            { 70,	1075},
            { 69,	1026},
            { 68,	973},
            { 67,	995},
            { 66,	976},
            { 65,	1000},
            { 64,	1009},
            { 63,	985},
            { 62,	840},
            { 61,	1122},
            { 60,	1099},
            { 59,	964},
            { 58,	950},
            { 57,	1009},
            { 56,	998},
            { 55,	998},
            { 54,	999},
            { 53,	1000}
        });
    }
}