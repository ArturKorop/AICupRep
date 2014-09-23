using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class Actions
    {
        protected World world;
        protected Game game;
        protected Hockeyist self;
        protected Move move;
        protected CurrentSituation currentSituation;

        protected double DistanceToBestAttackPosition { get { return this.self.GetDistanceTo(this.BestPositionToAttack); } }
        protected double AngleToBestAttackPosition { get { return this.self.GetAngleTo(this.BestPositionToAttack); } }
        protected double DistanceToPuck { get { return this.self.GetDistanceTo(this.Puck); } }
        protected double AngleToPuck { get { return this.self.GetAngleTo(this.Puck); } }
        protected Hockeyist NearestOpponent { get { return this.self.NearestOpponentDistance(this.world); } }
        protected bool CanHitPuck { get { return this.self.CanHitPuck(this.world, this.game); } }
        protected Point BestPositionToAttack { get { return GetBestPositionToAttack(this.self, this.world, this.game); } }
        protected bool CanDoAction { get { return this.self.RemainingCooldownTicks == 0; } }

        protected Unit Puck
        {
            get { return this.world.Puck; }
        }

        public Actions(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
        {
            this.world = world;
            this.game = game;
            this.self = self;
            this.move = move;
            this.currentSituation = currentSituation;
        }

        public virtual void FreePuck_SelfNearestToPuckAction()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(move.Turn);
            move.Action = ActionType.TakePuck;
        }

        public virtual void FreePuck_TeammateNearestToPuck()
        {
            var teammateNearestToPuck = world.NearestTeammateToPuck();
            var nearestOpponentToTeammateWithPuck = teammateNearestToPuck.NearestOpponentTime(world, game);

            if (self.GetDistanceTo(nearestOpponentToTeammateWithPuck) < game.StickLength && Math.Abs(self.GetAngleTo(nearestOpponentToTeammateWithPuck)) < game.StickSector / 2)
            {
                move.Action = ActionType.Strike;
            }

            var targetPosition = Manager.RetreatPosition;

            if (self.GetDistanceTo(targetPosition) > 100)
            {
                this.move.Turn = this.self.GetAngleTo(targetPosition.X, targetPosition.Y);
                this.move.SpeedUp = CalculateOptimalSpeed(this.move.Turn);
            }
            else
            {
                this.move.Turn = this.self.GetAngleTo(this.world.Puck);
                this.move.SpeedUp = 0;
            }
        }

        public virtual void MeHavePuck_SelfHavePuck()
        {

        }

        public virtual void MeHavePuck_TeammateHavePuck()
        {

        }

        public virtual void OpponentHavePuck()
        {
        }

        public void Strike()
        {
            move.SpeedUp = 0;
            move.Action = ActionType.Strike;
        }

        public void Swing()
        {
            move.SpeedUp = 0;
            move.Action = ActionType.Swing;
        }

        public void Pass(double angle, double distance)
        {
            this.move.Action = ActionType.Pass;
            this.move.PassAngle = angle;
            this.move.PassAngle = angle;
            this.move.PassPower = distance / 100;
        }

        public void AttackerWaitPuck()
        {
            var bestAttackPosition = BestPositionToAttack;

            if (this.self.IsNearPoint(bestAttackPosition, Constants.NearPointDistance))
            {
                this.move.SpeedUp = 0;
                this.move.Turn = this.self.GetAngleTo(this.Puck);
            }
            else
            {
                this.SetTurnAndSpeed(this.self.GetAngleTo(bestAttackPosition));
            }
        }

        public void HavePuck_SelfHavePuck_MoveToBestStrikePosition()
        {
            move.Turn = self.GetAngleTo(BestPositionToAttack);
            move.SpeedUp = 1;
        }

        public void HavePuck_SelfHavePuck_TurnToStrike()
        {
            var bestHitPosition = GetBestHitPosition(self, this.currentSituation);

            move.SpeedUp = 0;
            move.Turn = self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y);
        }

        public void OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(move.Turn);
            move.Action = ActionType.Strike;
        }

        public void HavePuck_GoToRetreatPosition()
        {
            var basePoint = Manager.RetreatPosition;

            move.Turn = self.GetAngleTo(basePoint.X, basePoint.Y);
            move.SpeedUp = CalculateOptimalSpeed(move.Turn);
        }

        public static double CalculateOptimalSpeed(double turn, double speed = 1)
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

        public static Point GetBestPositionToAttack(Hockeyist self, World world, Game game)
        {
            var nearestOpponent = self.NearestOpponentDistance(world);
            

            var selfDistToTop = self.ApproximateTimeToGoToTarget(Manager.BestTopStrikePosition, game);
            var selfDistToBottom = self.ApproximateTimeToGoToTarget(Manager.BestBottomStrikePosition, game);
            //if (Math.Min(selfDistToTop, selfDistToBottom) > 200)
            //{
            //    return minDistOpponentToBottom > minDistOpponentToTop
            //        ? Manager.BestBottomStrikePosition
            //        : Manager.BestTopStrikePosition;
            //}
            //else
            //{
            //    return selfDistToBottom > selfDistToTop
            //        ? Manager.BestTopStrikePosition
            //        : Manager.BestBottomStrikePosition;
            //}
            //return selfDistToBottom < selfDistToTop ? Manager.BestBottomStrikePosition : Manager.BestTopStrikePosition;

            //return nearestOpponent.Y > Manager.FieldCenter.Y && self.Y > nearestOpponent.Y
            //    ? Manager.BestTopStrikePosition
            //    : nearestOpponent.Y < Manager.FieldCenter.Y && self.Y < nearestOpponent.Y
            //        ? Manager.BestBottomStrikePosition
            //        : selfDistToBottom < selfDistToTop
            //            ? Manager.BestBottomStrikePosition
            //            : Manager.BestTopStrikePosition;
            return nearestOpponent.Y > self.Y
                ? Manager.BestTopStrikePosition
                : Manager.BestBottomStrikePosition;

        }

        public static Point GetBestHitPosition(Hockeyist self, CurrentSituation currentSituation)
        {
            return currentSituation.BestHitPosition;
        }

        protected void SetTurnAndSpeed(double turn)
        {
            this.move.Turn = turn;
            this.move.SpeedUp = CalculateOptimalSpeed(turn);
        }
    }
}
