using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class UnitExtensions
    {
        public static double ApproximateTimeToGoToTarget(this Hockeyist self, Point target, Game game)
        {
            var angleToTarget = self.GetAngleTo(target.X, target.Y);
            var distanceToTarget = self.GetDistanceTo(target);

            var turnTicks = angleToTarget / game.HockeyistTurnAngleFactor;
            var distanceTicks = 0.0;
            var a = (game.HockeyistSpeedUpFactor - game.HockeyistSpeedDownFactor);
            if (angleToTarget < Math.PI / 2)
            {
                var d = Math.Sqrt(Math.Pow(self.Speed(), 2) + 2 * a * distanceToTarget);
                distanceTicks = (d - self.Speed()) / a;
            }
            else
            {
                distanceTicks = Math.Sqrt(2 * distanceToTarget / a);
            }

            return distanceTicks / 2 + turnTicks;
        }

        public static double Speed(this Unit self)
        {
            return Math.Sqrt(Math.Pow(self.SpeedX, 2) + Math.Pow(self.SpeedY, 2));
        }

        public static double DistanceTo(this Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        public static double DistanceToSegment(this Point from, Point a, Point b)
        {
            if (from.X > Math.Max(a.X, b.X) || from.X < Math.Min(a.X, b.X) || from.Y > Math.Max(a.Y, b.Y) || from.Y < Math.Min(a.Y, b.Y))
            {
                return double.MaxValue;
            }

            var distance = Math.Abs((b.X - a.X) * (a.Y - from.Y) - (a.X - from.X) * (b.Y - a.Y)) / b.DistanceTo(a);

            return distance;
        }

        public static Point ToPoint(this Unit unit)
        {
            return new Point(unit.X, unit.Y);
        }

        public static double GetDistanceTo(this Unit unit, Point point)
        {
            return unit.GetDistanceTo(point.X, point.Y);
        }

        public static double DistanceToSegment(this Unit unit, Point a, Point b)
        {
            return unit.ToPoint().DistanceToSegment(a, b);
        }
    }
}
