using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class HockeyistExtensions
    {
        public static Hockeyist NearestOpponent(this Hockeyist self, World world, Game game)
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
