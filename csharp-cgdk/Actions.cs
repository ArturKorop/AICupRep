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
            var nearestOpponentToTeammateWithPuck = teammateNearestToPuck.NearestOpponent(world, game);

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

        public void Strike()
        {
            move.SpeedUp = 1;
            move.Action = ActionType.Strike;
        }

        public void Swing()
        {
            move.SpeedUp = 1;
            move.Action = ActionType.Swing;
        }

        public virtual void HavePuck_SelfHavePuck_MoveToBestStrikePosition()
        {
            var bestStrikePosition = GetBestPositionToAttack(self, world);

            move.Turn = self.GetAngleTo(bestStrikePosition.X, bestStrikePosition.Y);
            move.SpeedUp = 1;//CalculateOptimalSpeed(1, move.Turn);
        }

        public virtual void HavePuck_SelfHavePuck_TurnToStrike()
        {
            var bestHitPosition = GetBestHitPosition(self, this.currentSituation);

            move.SpeedUp = 0;
            move.Turn = self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y);
        }

        public virtual void OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(move.Turn);
            move.Action = ActionType.Strike;
        }

        public virtual void HavePuck_GoToRetreatPosition()
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
