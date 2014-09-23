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

        public static Point RetreatPosition { get; set; }

        public static Point DefenderPosition { get; set; }

        public static Point FieldCenter { get; set; }

        public static Point MyNetCenter { get; set; }

        public static Point OpponentNetCenter { get; set; }

        public static double MyThirdX { get; set; }

        public static double OpponentThirdX { get; set; }

        public static double TopThirdY { get; set; }

        public static double BottomThirdY { get; set; }

        public static long AttackerId { get; set; }

        public static long DefenderId { get; set; }

        public static Dictionary<long, SelfHavePuckStates?> RetreatList;

        private static bool isInit;

        public static void Init(World world, Game game)
        {
            if (!isInit)
            {
                FieldCenter = world.Puck.ToPoint();
                CalculateNetCentres(world, game);
                CalculateBestStrikePosition(world, game);
                CalculateRoles(world);
                InitRetreateList(world);

                isInit = true;
            }

            if(world.GetMyPlayer().IsJustMissedGoal || world.GetMyPlayer().IsJustScoredGoal)
            {
                isInit = false;
            }
        }

        public static void ChangeAttVsDef(World world, Game game)
        {
            var tempId = Manager.AttackerId;
            Manager.AttackerId = Manager.DefenderId;
            Manager.DefenderId = tempId;

            var opponent = world.GetOpponentPlayer();
            var rinkBorderLength = ((game.RinkBottom - game.RinkTop) - game.GoalNetHeight) / 2;
            var bestTopY = game.RinkTop + rinkBorderLength * 0.4;
            var bestBottomY = game.RinkBottom - rinkBorderLength * 0.4;

            var rinkLength = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = (rinkLength / 2) * 1.1;
            var bestX = 0.0;
            var bestHitX = 0.0;
            var retreatX = 0.0;
            var defenderX = 0.0;
            if (opponent.NetLeft > 500)
            {
                bestX = game.RinkRight - bestAttackerRinkLength;
                bestHitX = game.RinkRight;
                retreatX = game.RinkLeft + Constants.RetreatX;
                defenderX = MyNetCenter.X + Constants.DefenderRangeFromNet;
            }
            else
            {
                bestX = game.RinkLeft + bestAttackerRinkLength;
                bestHitX = game.RinkLeft;
                retreatX = game.RinkRight - Constants.RetreatX;
                defenderX = MyNetCenter.X - Constants.DefenderRangeFromNet;
            }

            BestTopStrikePosition = new Point(bestX, bestTopY);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);
        }

        private static void CalculateBestStrikePosition(World world, Game game)
        {
            var opponent = world.GetOpponentPlayer();
            var rinkBorderLength = ((game.RinkBottom - game.RinkTop) - game.GoalNetHeight) / 2;
            var bestTopY = game.RinkTop + rinkBorderLength * 0.4;
            var bestBottomY = game.RinkBottom - rinkBorderLength * 0.4;

            var rinkLength = game.RinkRight - game.RinkLeft;
            var bestAttackerRinkLength = (rinkLength / 2) * 0.9;
            var bestX = 0.0;
            var bestHitX = 0.0;
            var retreatX = 0.0;
            var defenderX = 0.0;
            if(opponent.NetLeft > 500) 
            {
                bestX = game.RinkRight - bestAttackerRinkLength;
                bestHitX = game.RinkRight;
                retreatX = game.RinkLeft + Constants.RetreatX;
                defenderX = MyNetCenter.X + Constants.DefenderRangeFromNet;
            }
            else
            {
                bestX = game.RinkLeft + bestAttackerRinkLength;
                bestHitX = game.RinkLeft;
                retreatX = game.RinkRight - Constants.RetreatX;
                defenderX = MyNetCenter.X - Constants.DefenderRangeFromNet;
            }
            
            BestTopStrikePosition = new Point(bestX, bestTopY);

            BestBottomStrikePosition = new Point(bestX, bestBottomY);

            RetreatPosition = new Point(retreatX, FieldCenter.Y);

            DefenderPosition = new Point(defenderX, MyNetCenter.Y);

        }

        private static void CalculateRoles(World world)
        {
            Hockeyist nearestToPuck = null;
            Hockeyist farestToPuck = null;
            double nearestDist = double.MaxValue;
            double farestDist = double.MinValue;

            foreach (var hockeyiest in world.MyTeam())
            {
                var dist = hockeyiest.GetDistanceTo(world.Puck);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestToPuck = hockeyiest;
                }

                if (dist > farestDist)
                {
                    farestDist = dist;
                    farestToPuck = hockeyiest;
                }
            }

            AttackerId = nearestToPuck.Id;
            DefenderId = farestToPuck.Id;
        }

        private static void InitRetreateList(World world)
        {
            RetreatList = new Dictionary<long, SelfHavePuckStates?>();
            foreach (var hockeyiest in world.MyTeam())
            {
                RetreatList.Add(hockeyiest.Id, null);
            }
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
}
