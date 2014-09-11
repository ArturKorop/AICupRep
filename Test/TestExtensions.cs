using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class TestExtensions
    {
        [TestMethod]
        public void DistanceTo_1()
        {
            var a = new Point(100, 100);
            var b = new Point(200, 100);
            var from = new Point(150, 50);

            Assert.AreEqual(from.DistanceToSegment(a, b), 50);
        }

        [TestMethod]
        public void DistanceTo_2()
        {
            var a = new Point(100, 100);
            var b = new Point(200, 200);
            var from = new Point(150, 150);

            Assert.AreEqual(from.DistanceToSegment(a, b), 0);
        }

        [TestMethod]
        public void DistanceTo_3()
        {
            var a = new Point(100, 100);
            var b = new Point(200, 110);
            var from = new Point(150, 50);

            Assert.AreEqual(Math.Round(from.DistanceToSegment(a, b), 0), 55);
        }

        [TestMethod]
        public void DistanceTo_4()
        {
            var a = new Point(100, 100);
            var b = new Point(200, 100);
            var from = new Point(150, 150);

            Assert.AreEqual(from.DistanceToSegment(a, b), 50);
        }
    }
}
