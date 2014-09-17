using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class Manager
    {
        public static Point BestTopStrikePosition { get; set; }

        public static Point BestTopHitPosition { get; set; }

        public static Point BestBottomStrikePosition { get; set; }

        public static Point BestBottomHitPosition { get; set; }

        public static Point RetreatPosition { get; set; }

        public static Point FieldCenter { get; set; }

        public static Point MyNetCenter { get; set; }

        public static Point OpponentNetCenter { get; set; }

        public static double MyThirdX { get; set; }

        public static double OpponentThirdX { get; set; }

        public static double TopThirdY { get; set; }

        public static double BottomThirdY { get; set; }

        public static int AttackerId { get; set; }

        public static int DefenderId { get; set; }

        public static Dictionary<long, SelfHavePuckStates?> RetreatList;

        private static bool isInit;

        public static void Init(World world, Game game)
        {
            if (!isInit)
            {
                FieldCenter = world.Puck.ToPoint();
                CalculateBestStrikePosition(world, game);
                CalculateNetCentres(world, game);

                RetreatList = new Dictionary<long, SelfHavePuckStates?>();
                foreach (var hockeyiest in world.MyTeam())
                {
                    RetreatList.Add(hockeyiest.Id, null);
                }

                isInit = true;
            }
        }

        private static void CalculateBestStrikePosition(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();
            var rinkBorderLength = ((game.RinkBottom - game.RinkTop) - game.GoalNetHeight) / 2;
            var bestTopY = game.RinkTop + rinkBorderLength * 0.5;
            var bestBottomY = game.RinkBottom - rinkBorderLength * 0.5;

            var rinkLength = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = rinkLength / 2 * 0.7;
            var bestX = 0.0;
            var bestHitX = 0.0;
            var retreatX = 0.0;
            if(opponent.NetLeft > 500) 
            {
                bestX = game.RinkRight - bestAttackerRinkLength;
                bestHitX = game.RinkRight;
                retreatX = game.RinkLeft + Constants.RetreatX;
            }
            else
            {
                bestX = game.RinkLeft + bestAttackerRinkLength;
                bestHitX = game.RinkLeft;
                retreatX = game.RinkRight - Constants.RetreatX;
            }
            
            BestTopStrikePosition = new Point(bestX, bestTopY);
            BestTopHitPosition = new Point(bestHitX, opponent.NetTop + 20);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);
            BestBottomHitPosition = new Point(bestHitX, opponent.NetBottom - 20);

            RetreatPosition = new Point(retreatX, FieldCenter.Y);

            Hockeyist nearestToPuck = null;
            Hockeyist farestToPuck = null;
            double nearestDist = double.MaxValue;
            double farestDist = double.MinValue;

            foreach (var hockeyiest in world.MyTeam())
            {
                var dist = hockeyiest.GetDistanceTo(world.Puck);
                if(dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestToPuck = hockeyiest;
                }
                
                if(dist > farestDist)
                {
                    farestDist = dist;
                    farestToPuck = hockeyiest;
                }
            }

            AttackerId = nearestToPuck.Id;
            DefenderId = farestToPuck.Id;
        }

        private static void CalculateNetCentres(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();
            var me = world.GetMyPlayer();

            OpponentNetCenter = new Point(opponent.NetFront, (opponent.NetTop + opponent.NetBottom) / 2);
            MyNetCenter = new Point(me.NetFront, (me.NetTop + me.NetBottom) / 2);
            FieldCenter = new Point(world.Puck.X, world.Puck.Y);

            var fieldWidth = Math.Abs(me.NetFront - opponent.NetFront);
            var fieldHeight = game.RinkBottom - game.RinkTop;

            var thirdFieldWidth = fieldWidth / 3;
            var thirdFieldHeight = fieldHeight / 3;

            MyThirdX = MyNetCenter.X > FieldCenter.X
                ? MyNetCenter.X - thirdFieldWidth
                : MyNetCenter.X + thirdFieldWidth;

            OpponentThirdX = OpponentNetCenter.X > FieldCenter.X
                ? OpponentNetCenter.X - thirdFieldWidth
                : OpponentNetCenter.X + thirdFieldWidth;

            TopThirdY = game.RinkTop + thirdFieldHeight;
            BottomThirdY = game.RinkBottom - thirdFieldHeight;

            //BestTopPrepareToStrikePosition = new Point(FieldCenter.X, TopThirdY);
            //BestBottomPrepareToStrikePosition = new Point(FieldCenter.X, BottomThirdY);
        }
    }

    public static class StateExtensions
    {
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
            Manager.RetreatList[self.Id] = null;
            var selfDistanceToPuck = self.GetDistanceTo(world.Puck);

            return world.Teammates(self).Max(x => x.GetDistanceTo(world.Puck)) > selfDistanceToPuck
                ? FreePuckStates.TeammateNearestToPuck
                : FreePuckStates.SelfNearestToPuck;
        }

        public static OpponentHavePuckStates OpponentHavePuckState(this World world, Hockeyist self)
        {
            Manager.RetreatList[self.Id] = null;

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

            if (distanceToPuck <= game.StickLength && Math.Abs(angleToPuck) <= game.StickSector / 2)
            {
                return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
            }

            if (distanceToOpponent <= game.StickLength && Math.Abs(angleToOpponent) <= game.StickSector / 2)
            {
                if(distanceToPuck <= game.StickLength && Math.Abs(angleToPuck) <= game.StickSector / 2)
                {
                    return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
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

        public static SelfHavePuckStates SelfHavePuckState(this World world, Hockeyist self, CurrentSituation currentSituation)
        {
            if(self.State == HockeyistState.Swinging)
            {
                return SelfHavePuckStates.Strike;
            }

            var isRetreat = Manager.RetreatList[self.Id];
            if(isRetreat != null)
            {
                var basePoint = Manager.RetreatPosition;
                if (self.GetDistanceTo(basePoint.X, basePoint.Y) > 150)
                {
                    return SelfHavePuckStates.MoveToRetreatPosition;
                }
                else
                {
                    Manager.RetreatList[self.Id] = null;
                }
            }

            var distanceToNet = self.GetDistanceTo(world.OpponentNetCenter().X, world.OpponentNetCenter().Y);
            var bestPosToAttack = Actions.GetBestPositionToAttack(self, world);
            var distanceToBestStrikePosition = self.GetDistanceTo(bestPosToAttack.X, bestPosToAttack.Y);
            var distanceBetweenNetAndBestStrikePosition = bestPosToAttack.DistanceTo(world.OpponentNetCenter());

            if (distanceToNet < distanceBetweenNetAndBestStrikePosition || distanceToBestStrikePosition < 50 || distanceToNet < distanceToBestStrikePosition)
            {
                var bestHitPosition = Actions.GetBestHitPosition(self, currentSituation);
                var nearestOpponentDistanceToStrikeVector = world.Hockeyists.Where(x => !x.IsTeammate).Select(x => x.DistanceToSegment(self.ToPoint(), bestHitPosition)).Min();
                if (nearestOpponentDistanceToStrikeVector < world.Puck.Radius / 2)
                {
                    Manager.RetreatList[self.Id] = SelfHavePuckStates.MoveToRetreatPosition;

                    return SelfHavePuckStates.MoveToRetreatPosition;
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
    }

    public static class HockeyistExtensions
    {
        public static Hockeyist NearestOpponent(this Hockeyist self, World world)
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

        public static bool IsAttacker(this Hockeyist self)
        {
            return self.Id == Manager.AttackerId;
        }

        public static Hockeyist IsDefender(this Hockeyist self)
        {
            return self.Id == Manager.DefenderId;
        }
    }

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

    public static class WorldExtensions
    {
        #region Reliable func

        /// <summary>
        /// Return opponent goalie, if it possible.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <returns>The opponent goalie.</returns>
        public static Hockeyist OpponentGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => !x.IsTeammate && x.Type == HockeyistType.Goalie);
        }

        /// <summary>
        /// Return my goalie, if it possible.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <returns>My goalie.</returns>
        public static Hockeyist MyGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.IsTeammate && x.Type == HockeyistType.Goalie);
        }

        /// <summary>
        /// Return all other teammates without goalie, resting teammates and self.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="self">The self.</param>
        /// <returns>The teammates</returns>
        public static IEnumerable<Hockeyist> Teammates(this World world, Hockeyist self)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Id != self.Id && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        /// <summary>
        /// Return all my team in the rink without goalie and resting teammates
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The teammates.</returns>
        public static IEnumerable<Hockeyist> MyTeam(this World world)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        /// <summary>
        /// Return all opponent team in the rink without goalie and resting teammates
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The teammates.</returns>
        public static IEnumerable<Hockeyist> OpponentTeam(this World world)
        {
            return world.Hockeyists.Where(x => !x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
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

        #endregion

        public static Hockeyist NearestTeammateToPuck(this World world)
        {
            Hockeyist nearestTeammateToPuck = null;
            var rangeToPuck = double.MaxValue;

            foreach (var hockeyist in world.MyTeam())
            {
                var distance = hockeyist.GetDistanceTo(world.Puck);
                if (nearestTeammateToPuck == null || distance < rangeToPuck)
                {
                    nearestTeammateToPuck = hockeyist;
                    rangeToPuck = distance;
                }
            }

            return nearestTeammateToPuck;
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
