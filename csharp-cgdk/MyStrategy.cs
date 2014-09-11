using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Hockeyist self, World world, Game game, Move move)
        {
            Manager.SetRoles(world, game);

            StateMachine.Run(world, move, self, game);
        }

        private static Hockeyist GetNearestOpponent(double x, double y, World world)
        {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.Hockeyists)
            {
                if (hockeyist.IsTeammate || hockeyist.Type == HockeyistType.Goalie
                    || hockeyist.State == HockeyistState.KnockedDown
                    || hockeyist.State == HockeyistState.Resting)
                {
                    continue;
                }

                double opponentRange = GetDistance(x,y, hockeyist.X, hockeyist.Y);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange)
                {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent;
        }

        private static double GetDistance(double x, double y, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
        }

        public static Point GetBestPositionToAttack(Hockeyist self)
        {
            if(self.GetDistanceTo(Manager.BestTopStrikePosition.X, Manager.BestTopStrikePosition.Y) > self.GetDistanceTo(Manager.BestBottomStrikePosition.X, Manager.BestBottomStrikePosition.Y))
            {
                 return Manager.BestBottomStrikePosition;
            }

            return Manager.BestTopStrikePosition;
        }

        public static Point GetBestHitPosition(Hockeyist self)
        {
            return GetBestPositionToAttack(self) == Manager.BestTopStrikePosition ? Manager.BestBottomHitPosition : Manager.BestTopHitPosition;
        }
    }
}