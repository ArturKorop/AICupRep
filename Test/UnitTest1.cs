using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(0, 100);
            Assert.AreEqual(angle, Math.PI);
        }

        [TestMethod]
        public void Test2()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(100, 200);
            Assert.AreEqual(angle, Math.PI/2);
        }

        [TestMethod]
        public void Test3()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(100, 50);
            Assert.AreEqual(angle, -Math.PI/2);
        }

        [TestMethod]
        public void Test4()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(200, 100);
            Assert.AreEqual(angle, 0);
        }

        [TestMethod]
        public void Test5()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(150, 50);
            Assert.AreEqual(angle, -Math.PI/4);
        }

        [TestMethod]
        public void Test6()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(50, 50);
            Assert.AreEqual(angle, -Math.PI * 3/4);
        }

    }
}
