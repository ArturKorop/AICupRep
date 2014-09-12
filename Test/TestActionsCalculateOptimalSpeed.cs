using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk;

namespace Test
{
    [TestClass]
    public class TestCalculateOptimalSpeed
    {
        [TestMethod]
        public void Turn_180()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(0, 100);
            Assert.AreEqual(angle, Math.PI);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, 0), -1);
        }

        [TestMethod]
        public void Turn_90()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(100, 200);
            Assert.AreEqual(angle, Math.PI / 2);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, angle), 0.5);
        }

        [TestMethod]
        public void Turn_M90()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(100, 50);
            Assert.AreEqual(angle, -Math.PI / 2);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, angle), 0.5);
        }

        [TestMethod]
        public void Turn_0()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(200, 100);
            Assert.AreEqual(angle, 0);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, angle), 1);
        }

        [TestMethod]
        public void Turn_M45()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(150, 50);
            Assert.AreEqual(angle, -Math.PI / 4);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, angle), 0.75);
        }

        [TestMethod]
        public void Turn_M135()
        {
            var self = new Hockeyist(1, 1, 1, 10, 30, 100, 100, 0, 0, 0, 0, true, HockeyistType.Versatile, 100, 100, 100, 100, 100, HockeyistState.Active, 100, 30, 30, 10, null, null);
            var angle = self.GetAngleTo(50, 50);
            Assert.AreEqual(angle, -Math.PI * 3 / 4);
            Assert.AreEqual(Strategy.CalculateOptimalSpeed(1, angle), -1);
        }

    }
}
