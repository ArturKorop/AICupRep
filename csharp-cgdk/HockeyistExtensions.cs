using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class HockeyistExtensions
    {
        public static Hockeyist NearestOpponentTime(this Hockeyist self, World world, Game game)
        {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.OpponentTeam())
            {
                double opponentRange = self.ApproximateTimeToGoToTarget(hockeyist.ToPoint(), game);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange)
                {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent ?? world.OpponentTeam().First();
        }

        public static Hockeyist NearestOpponentDistance(this Hockeyist self, World world)
        {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.OpponentTeam())
            {
                double opponentRange = self.GetDistanceTo(hockeyist);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange)
                {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent ?? world.OpponentTeam().First();
        }

        public static bool CanHitPuck(this Hockeyist self, World world, Game game)
        {
            var puck = world.Puck;

            return self.GetDistanceTo(puck) <= game.StickLength &&
                    Math.Abs(self.GetAngleTo(puck)) <= game.StickSector / 2;
        }

        public static bool IsNearPoint(this Hockeyist self, Point point, double nearDistance)
        {
            return self.GetDistanceTo(point) <= nearDistance;
        }

        public static bool IsAttacker(this Hockeyist self)
        {
            return self.Id == Manager.AttackerId;
        }

        public static bool IsDefender(this Hockeyist self)
        {
            return self.Id == Manager.DefenderId;
        }
    }
}
