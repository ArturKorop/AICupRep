using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class AttackStrategy : Strategy
    {
        public AttackStrategy(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
            : base(world, game, self, move, currentSituation)
        {
        }

        public override void HavePuck_TeammateHavePuck()
        {
            var teammateWithPuck = world.HockeyistWithPuck();
            var nearestOpponentToTeammateWithPuck = teammateWithPuck.NearestOpponent(world);
            var nearestOpponentToNet = world.OpponentTeam().First(x => x.GetDistanceTo(Manager.OpponentNetCenter) == world.OpponentTeam().Select(y => y.GetDistanceTo(Manager.OpponentNetCenter)).Min());

            Hockeyist targetOpponent;
            if(self.GetDistanceTo(nearestOpponentToTeammateWithPuck) > self.GetDistanceTo(nearestOpponentToNet))
            {
                targetOpponent = nearestOpponentToNet;
            }
            else
            {
                targetOpponent = nearestOpponentToTeammateWithPuck;
            }

            move.Turn = self.GetAngleTo(targetOpponent);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);

            if (self.GetDistanceTo(targetOpponent) < game.StickLength && Math.Abs(self.GetAngleTo(targetOpponent)) < game.StickSector / 2 &&
                !(self.GetDistanceTo(teammateWithPuck) < game.StickLength && Math.Abs(self.GetAngleTo(teammateWithPuck)) < game.StickSector / 2))
            {
                move.Action = ActionType.Strike;
            }
        }

        public override void FreePuck()
        {
            var puck = world.Puck;
            var distanceToPuck = this.self.ApproximateTimeToGoToTarget(puck.ToPoint(), this.game);
            if (distanceToPuck <= this.world.Teammates(self).Select(x => x.ApproximateTimeToGoToTarget(puck.ToPoint(), this.game)).Min())
            {
                this.move.Turn = this.self.GetAngleTo(puck);
                this.move.SpeedUp = CalculateOptimalSpeed(1, this.move.Turn);
                this.move.Action = ActionType.TakePuck;
            }
            else
            {
                this.move.Turn = this.self.GetAngleTo(puck);
                this.move.SpeedUp = CalculateOptimalSpeed(1, this.move.Turn);
                this.move.Action = ActionType.TakePuck;
            }
        }

        public override void HavePuck_SelfHavePuck()
        {
            
        }

        public override void OpponentHavePuck()
        {
            
        }
    }

    public abstract class Strategy
    {
        protected World world;
        protected Game game;
        protected Hockeyist self;
        protected Move move;
        protected CurrentSituation currentSituation;

        public Strategy(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
        {
            this.world = world;
            this.game = game;
            this.self = self;
            this.move = move;
            this.currentSituation = currentSituation;
        }

        public abstract void FreePuck();

        public abstract void HavePuck_SelfHavePuck();

        public abstract void HavePuck_TeammateHavePuck();

        public abstract void OpponentHavePuck();

        public void Strike()
        {
            move.SpeedUp = 1;
            move.Action = ActionType.Strike;
        }

        

        public void HavePuck_SelfHavePuck_MoveToBestStrikePosition()
        {
            var bestStrikePosition = GetBestPositionToAttack(self, world);
            var bestPrepareToStrikePosition = GetBestPreparePositionToStrikePosition(self, world);

            Point targetPosition = null;
            if(CurrentSituation.IsBetweenCoord(self.X, Manager.MyNetCenter.X, bestPrepareToStrikePosition.X))
            {
                targetPosition = bestPrepareToStrikePosition;
            }
            else
            {
                targetPosition = bestStrikePosition;
            }

            move.Turn = self.GetAngleTo(targetPosition.X, targetPosition.Y);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
        }

        public void HavePuck_SelfHavePuck_TurnToStrike()
        {
            var bestHitPosition = this.currentSituation.BestHitPosition;

            move.SpeedUp = 0;
            move.Turn = self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y);
        }

        public void OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
            move.Action = ActionType.Strike;
        }

        public void HavePuck_GoToBase()
        {
            var basePoint = Manager.RetreatPosition;

            move.Turn = self.GetAngleTo(basePoint.X, basePoint.Y);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
        }




        public static double CalculateOptimalSpeed(double speed, double turn)
        {
            var modTurn = Math.Abs(turn);
            if (modTurn > Math.PI)
            {
                throw new NotSupportedException();
            }

            if (modTurn > Math.PI / 2)
            {
                return -1;
            }

            if (modTurn == 0 || modTurn < Math.PI / 8)
            {
                return speed;
            }

            return speed * (1 - modTurn / Math.PI);
        }

        public static Point GetBestPositionToAttack(Hockeyist self, World world)
        {
            var minDistOpponentToTop = world.OpponentTeam().Select(x => x.GetDistanceTo(Manager.BestTopStrikePosition)).Min();
            var minDistOpponentToBottom = world.OpponentTeam().Select(x => x.GetDistanceTo(Manager.BestBottomStrikePosition)).Min();
            var selfDistToTop = self.GetDistanceTo(Manager.BestTopStrikePosition);
            var selfDistToBottom = self.GetDistanceTo(Manager.BestBottomStrikePosition);
            if (Math.Min(selfDistToTop, selfDistToBottom) > 300)
            {
                return minDistOpponentToBottom > minDistOpponentToTop
                    ? Manager.BestBottomStrikePosition
                    : Manager.BestTopStrikePosition;
            }
            else
            {
                return selfDistToBottom > selfDistToTop
                    ? Manager.BestTopStrikePosition
                    : Manager.BestBottomStrikePosition;
            }
        }

        public static Point GetBestPreparePositionToStrikePosition(Hockeyist self, World world)
        {
            var minDistOpponentToTop = world.OpponentTeam().Select(x => x.GetDistanceTo(Manager.BestTopPrepareToStrikePosition)).Min();
            var minDistOpponentToBottom = world.OpponentTeam().Select(x => x.GetDistanceTo(Manager.BestBottomPrepareToStrikePosition)).Min();
            var selfDistToTop = self.GetDistanceTo(Manager.BestTopPrepareToStrikePosition);
            var selfDistToBottom = self.GetDistanceTo(Manager.BestBottomPrepareToStrikePosition);
            if (Math.Min(selfDistToTop, selfDistToBottom) > 200)
            {
                return minDistOpponentToBottom > minDistOpponentToTop
                    ? Manager.BestBottomPrepareToStrikePosition
                    : Manager.BestTopPrepareToStrikePosition;
            }
            else
            {
                return selfDistToBottom > selfDistToTop
                    ? Manager.BestTopPrepareToStrikePosition
                    : Manager.BestBottomPrepareToStrikePosition;
            }
        }
    }
}
