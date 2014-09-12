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

        public static Point BestBottomStrikePosition { get; set; }

        public static Point BestTopPrepareToStrikePosition { get; set; }

        public static Point BestBottomPrepareToStrikePosition { get; set; }

        public static Point RetreatPosition { get; set; }

        public static Point DefenderPosition { get; set; }

        public static Point FieldCenter { get; set; }

        public static Point MyNetCenter { get; set; }

        public static Point OpponentNetCenter { get; set; }

        public static double MyThirdX { get; set; }

        public static double OpponentThirdX { get; set; }

        public static double TopThirdY { get; set; }

        public static double BottomThirdY { get; set; }

        private static bool isInit;

        public static void Init(World world, Game game)
        {
            if (!isInit)
            {
                CalculateNetCentres(world, game);
                CalculateBestStrikePosition(world, game);

                isInit = true;
            }
        }

        private static void CalculateBestStrikePosition(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();

            var bestTopY = game.RinkTop + Constants.RinkHeightToBestStrikePosition;
            var bestBottomY = game.RinkBottom - Constants.RinkHeightToBestStrikePosition;

            var rinkWidth = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = rinkWidth / 2 * 0.8;
            var bestX = 0.0;
            var retreatX = 0.0;
            if(opponent.NetLeft > Manager.FieldCenter.X) 
            {
                bestX = game.RinkRight - bestAttackerRinkLength;
                retreatX = game.RinkLeft + Constants.BestRetreatDistance;
            }
            else
            {
                bestX = game.RinkLeft + bestAttackerRinkLength;
                retreatX = game.RinkRight - Constants.BestRetreatDistance;
            }
            
            BestTopStrikePosition = new Point(bestX, bestTopY);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);

            RetreatPosition = new Point(retreatX, Manager.FieldCenter.Y);

            DefenderPosition = new Point(world.GetMyPlayer().NetFront, Manager.MyNetCenter.Y);
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

            BestTopPrepareToStrikePosition = new Point(FieldCenter.X, TopThirdY);
            BestBottomPrepareToStrikePosition = new Point(FieldCenter.X, BottomThirdY);
        }
    }

    public static class WorldExtensions
    {
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

        public static Hockeyist OpponentGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => !x.IsTeammate && x.Type == HockeyistType.Goalie);
        }

        public static Hockeyist MyGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.IsTeammate && x.Type == HockeyistType.Goalie);
        }
    }

    public static class MathExtensions
    {
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

        public static Point ToPoint(this Unit hockeyist)
        {
            return new Point(hockeyist.X, hockeyist.Y);
        }

        public static double DistanceToSegment(this Hockeyist hockeist, Point a, Point b)
        {
            return hockeist.ToPoint().DistanceToSegment(a, b);
        }

        public static double GetDistanceTo(this Hockeyist hockeyist, Point point)
        {
            return hockeyist.GetDistanceTo(point.X, point.Y);
        }
    }

    public static class Extensions
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

        public static Hockeyist HockeyistWithPuck(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.Id == world.Puck.OwnerHockeyistId);
        }

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

        public static Hockeyist NearestTeammateToPuck(this World world)
        {
            Hockeyist nearestTeammateToPuck = null;
            var rangeToPuck = double.MaxValue;

            foreach (var hockeyist in world.MyTeam())
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
    }

    public static class HockeyistExtensions
    {
        public static double ApproximateTimeToGoToTarget(this Hockeyist self, Point target, Game game)
        {
            var angleToTarget = self.GetAngleTo(target.X, target.Y);
            var distanceToTarget = self.GetDistanceTo(target);

            var turnTicks = angleToTarget / game.HockeyistTurnAngleFactor;
            var distanceTicks = 0.0;
            var a = (game.HockeyistSpeedUpFactor - game.HockeyistSpeedDownFactor);
            if(angleToTarget < Math.PI/2)
            {
                var d = Math.Sqrt(Math.Pow(self.Speed(), 2) + 2 * a * distanceToTarget);
                distanceTicks = (d - self.Speed()) / a;
            }
            else
            {
                distanceTicks = Math.Sqrt(2 * distanceToTarget / a);
            }

            return distanceTicks/2 + turnTicks;
        }

        public static double Speed(this Unit self)
        {
            return Math.Sqrt(Math.Pow(self.SpeedX, 2) + Math.Pow(self.SpeedY, 2));
        }
    }
}
