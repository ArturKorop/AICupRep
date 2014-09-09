using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class Manager
    {
        public static long DefenderId { get; set; }

        public static long ForwardId { get; set; }

        public static Point BestTopStrikePosition { get; set; }

        public static Point BestTopHitPosition { get; set; }

        public static Point BestBottomStrikePosition { get; set; }

        public static Point BestBottomHitPosition { get; set; }

        public static Point DefenderPosition { get; set; }

        private static bool isSetRoles;

        public static void SetRoles(World world, Game game)
        {
            if (!isSetRoles)
            {
                var teammates = world.Hockeyists.Where(x => x.IsTeammate && x.Type != HockeyistType.Goalie);
                var orderedTeammates = teammates.OrderByDescending(x => x.GetDistanceTo(world.Puck)).ToArray();
                DefenderId = orderedTeammates[0].Id;
                ForwardId = orderedTeammates[1].Id;

                CalculateBestStrikePosition(world, game);

                isSetRoles = true;
            }
        }

        private static void CalculateBestStrikePosition(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();
            var rinkBorderLength = ((game.RinkBottom - game.RinkTop) - game.GoalNetHeight) / 2;
            var bestTopY = game.RinkTop + rinkBorderLength / 2;
            var bestBottomY = game.RinkBottom - rinkBorderLength / 2;

            var rinkLength = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = rinkLength / 2 * 0.4;
            var bestX = 0.0;
            var bestHitX = 0.0;
            var defenderX = 0.0;
            if(opponent.NetLeft > 500) 
            {
                bestX = game.RinkRight - bestAttackerRinkLength;
                bestHitX = game.RinkRight;
                defenderX = game.RinkLeft + 100;
            }
            else
            {
                bestX = game.RinkLeft + bestAttackerRinkLength;
                bestHitX = game.RinkLeft;
                defenderX = game.RinkRight - 100;
            }
            
            BestTopStrikePosition = new Point(bestX, bestTopY);
            BestTopHitPosition = new Point(bestHitX, opponent.NetTop + 10);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);
            BestBottomHitPosition = new Point(bestHitX, opponent.NetBottom - 10);

            DefenderPosition = new Point(defenderX, (game.RinkBottom - game.RinkTop) / 2 + game.RinkTop);

        }
    }

    public static class Extensions
    {
        public static bool IsDeffender(this Hockeyist hockeyist)
        {
            return hockeyist.Id == Manager.DefenderId;
        }

        public static bool IsForward(this Hockeyist hockeyist)
        {
            return hockeyist.Id == Manager.ForwardId;
        }
    }

    public class Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
