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

        public static Dictionary<long, SelfHavePuckStates?> RetreatList;

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

                RetreatList = new Dictionary<long, SelfHavePuckStates?>();
                foreach (var hockeyiest in world.Team())
                {
                    RetreatList.Add(hockeyiest.Id, null);
                }

                isSetRoles = true;
            }
        }

        private static void CalculateBestStrikePosition(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();
            var rinkBorderLength = ((game.RinkBottom - game.RinkTop) - game.GoalNetHeight) / 2;
            var bestTopY = game.RinkTop + rinkBorderLength * 2 / 3;
            var bestBottomY = game.RinkBottom - rinkBorderLength * 2 / 3;

            var rinkLength = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = rinkLength / 2 * 0.8;
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
            BestTopHitPosition = new Point(bestHitX, opponent.NetTop + 20);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);
            BestBottomHitPosition = new Point(bestHitX, opponent.NetBottom - 20);

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

        public static PuckStates PuckState(this World world, Hockeyist self)
        {
            return 
                self.State == HockeyistState.Swinging ? PuckStates.Swing :
                world.Puck.OwnerPlayerId == world.GetMyPlayer().Id ? PuckStates.HavePuck :
                world.Puck.OwnerPlayerId == world.GetOpponentPlayer().Id ? PuckStates.OpponentHavePuck :
                PuckStates.FreePuck;
        }

        public static HavePuckStates HavePuckState(this World world, Hockeyist self)
        {
            return world.Puck.OwnerHockeyistId == self.Id ? HavePuckStates.SelfHavePuck : HavePuckStates.TeammateHavePuck;
        }

        public static FreePuckStates FreePuckState(this World world, Hockeyist self)
        {
            var selfDistanceToPuck = self.GetDistanceTo(world.Puck);

            return world.Teammates(self).Max(x => x.GetDistanceTo(world.Puck)) > selfDistanceToPuck
                ? FreePuckStates.TeammateNearestToPuck
                : FreePuckStates.SelfNearestToPuck;
        }

        public static OpponentHavePuckStates OpponentHavePuckState(this World world, Hockeyist self)
        {
            var opponentWithPuck = world.Hockeyists.Single(x => x.Id == world.Puck.OwnerHockeyistId);
            var selfDistanceToOpponentWithPuck = self.GetDistanceTo(opponentWithPuck);

            return world.Teammates(self).Max(x => x.GetDistanceTo(opponentWithPuck)) > selfDistanceToOpponentWithPuck
                ? OpponentHavePuckStates.TeammatesNearestToOpponentWithPuck
                : OpponentHavePuckStates.SelfNearestToOpponentWithPuck;
        }

        public static SelfNearestToOpponentWithPuckStates SelfNearestToOpponentWithPuckState(this World world, Hockeyist self, Game game)
        {
            var opponentWithPuck = world.HockeyistWithPuck();
            var distanceToOpponent = self.GetDistanceTo(opponentWithPuck);
            var distanceToPuck = self.GetDistanceTo(world.Puck);
            var angleToOpponent = self.GetAngleTo(opponentWithPuck);
            var angleToPuck = self.GetAngleTo(world.Puck);

            if (distanceToOpponent <= game.StickLength && Math.Abs(angleToOpponent) <= game.StickSector / 2)
            {
                if(distanceToPuck <= game.StickLength && Math.Abs(angleToPuck) <= game.StickSector / 2)
                {
                    return SelfNearestToOpponentWithPuckStates.CannotStrikeOpponent;
                }
                else
                {
                    return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
                }
            }
            else
            {
                return SelfNearestToOpponentWithPuckStates.CannotStrikeOpponent;
            }

        }

        public static SelfHavePuckStates SelfHavePuckState(this World world, Hockeyist self)
        {
            if(self.State == HockeyistState.Swinging)
            {
                return SelfHavePuckStates.Strike;
            }

            var isRetreat = Manager.RetreatList[self.Id];
            if(isRetreat != null)
            {
                var basePoint = Manager.DefenderPosition;
                if (self.GetDistanceTo(basePoint.X, basePoint.Y) > 150)
                {
                    return SelfHavePuckStates.MoveToBase;
                }
                else
                {
                    Manager.RetreatList[self.Id] = null;
                }
            }

            var distanceToNet = self.GetDistanceTo(world.OpponentNetCenter().X, world.OpponentNetCenter().Y);
            var bestPosToAttack = Actions.GetBestPositionToAttack(self);
            var distanceToBestStrikePosition = self.GetDistanceTo(bestPosToAttack.X, bestPosToAttack.Y);
            var distanceBetweenNetAndBestStrikePosition = bestPosToAttack.DistanceTo(world.OpponentNetCenter());

            if (distanceToNet < distanceBetweenNetAndBestStrikePosition || distanceToBestStrikePosition < 50 || distanceToNet < distanceToBestStrikePosition)
            {
                var bestHitPosition = Actions.GetBestHitPosition(self);
                var nearestOpponentDistanceToStrikeVector = world.Hockeyists.Where(x => !x.IsTeammate).Select(x => x.DistanceToSegment(self.ToPoint(), bestHitPosition)).Min();
                if (nearestOpponentDistanceToStrikeVector < world.Puck.Radius)
                {
                    Manager.RetreatList[self.Id] = SelfHavePuckStates.MoveToBase;

                    return SelfHavePuckStates.MoveToBase;
                }
                else
                {
                    if (Math.Abs(self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y)) < Constants.StrikeAngle)
                    {
                        if (self.GetDistanceTo(self.NearestOpponent(world)) > Constants.DangerDistanceToOpponent && self.GetDistanceTo(bestHitPosition.X, bestHitPosition.Y) > Constants.DistanceToStrike)
                        {
                            return SelfHavePuckStates.Swing;
                        }
                        else
                        {
                            return SelfHavePuckStates.Strike;
                        }
                    }
                    else
                    {
                        return SelfHavePuckStates.TurnToStrike;
                    }
                }
            }
            else
            {
                return SelfHavePuckStates.MoveToBestStrikePosition;
            }
        }

        public static Hockeyist HockeyistWithPuck(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.Id == world.Puck.OwnerHockeyistId);
        }

        public static Point OpponentNetCenter(this World world)
        {
            var opponent = world.GetOpponentPlayer();

            return new Point((opponent.NetLeft + opponent.NetRight) / 2, (opponent.NetTop + opponent.NetBottom) / 2);
        }

        public static IEnumerable<Hockeyist> Teammates(this World world, Hockeyist self)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Id != self.Id && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        /// <summary>
        /// Return all team in the rink without goalie and resting teammates
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The teammates.</returns>
        public static IEnumerable<Hockeyist> Team(this World world)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        public static IEnumerable<Hockeyist> Opponents(this World world)
        {
            return world.Hockeyists.Where(x => !x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        public static Hockeyist NearestOpponent(this Hockeyist self, World world)
        {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.Opponents())
            {
                double opponentRange = self.GetDistanceTo(hockeyist);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange)
                {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent ?? world.Opponents().First();
        }

        public static Hockeyist NearestTeammateToPuck(this World world)
        {
            Hockeyist nearestTeammateToPuck = null;
            var rangeToPuck = double.MaxValue;

            foreach (var hockeyist in world.Team())
            {
                var distance = hockeyist.GetDistanceTo(world.Puck);
                if(nearestTeammateToPuck == null || distance < rangeToPuck)
                {
                    nearestTeammateToPuck = hockeyist;
                    rangeToPuck = distance;
                }
            }

            return nearestTeammateToPuck;
        }

        public static double DistanceTo(this Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        public static double DistanceToSegment(this Point from, Point a, Point b)
        {
            var distance = Math.Abs((b.X - a.X) * (a.Y - from.Y) - (a.X - from.X) * (b.Y - a.Y)) / b.DistanceTo(a);

            return distance;
        }

        public static Point ToPoint(this Hockeyist hockeyist)
        {
            return new Point(hockeyist.X, hockeyist.Y);
        }

        public static double DistanceToSegment(this Hockeyist hockeist, Point a, Point b)
        {
            return hockeist.ToPoint().DistanceToSegment(a, b);
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
