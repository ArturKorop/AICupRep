using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class StateMachine
    {
        public static void Run(World world, Move move, Hockeyist self, Game game)
        {
            var currentSituation = new CurrentSituation(world, game, self);
            var actions = new Actions(world, game, self, move, currentSituation);
            var puckState = world.PuckState(self);

            switch(puckState)
            {
                case PuckStates.FreePuck:
                    {
                        FreePuckState(world, game, self, actions);
                    }
                    break;
                case PuckStates.HavePuck:
                    {
                        MeHavePuck(world, game, self, actions, currentSituation);
                    }
                    break;
                case PuckStates.OpponentHavePuck:
                    {
                        OpponentHavePuckState(world, game, self, actions);
                    }
                    break;
                case PuckStates.Swing:
                    {
                        actions.HavePuck_SelfHavePuck_Strike();
                    }
                    break;
            }
#if Debug
            

            if(world.Tick == world.TickCount)
            {
                Console.WriteLine("{0}:{1}", world.GetMyPlayer().GoalCount, world.GetOpponentPlayer().GoalCount);
                Console.ReadKey();
            }
#endif
        }

        private static void FreePuckState(World world, Game game, Hockeyist self, Actions actions)
        {
            var freePuckState = world.FreePuckState(self);
            switch (freePuckState)
            {
                case FreePuckStates.SelfNearestToPuck:
                    {
                        actions.FreePuck_SelfNearestToPuckAction();
                    }
                    break;
                case FreePuckStates.TeammateNearestToPuck:
                    {
                        actions.FreePuck_SelfNearestToPuckAction();
                    }
                    break;
            }
        }

        private static void OpponentHavePuckState(World world, Game game, Hockeyist self, Actions actions)
        {
            var opponentHavePuckState = world.OpponentHavePuckState(self);
            switch (opponentHavePuckState)
            {
                case OpponentHavePuckStates.SelfNearestToOpponentWithPuck:
                    {
                        var selfNearestToOpponentWithPuckState = world.SelfNearestToOpponentWithPuckState(self, game);
                        switch (selfNearestToOpponentWithPuckState)
                        {
                            case SelfNearestToOpponentWithPuckStates.CanStrikeOpponent:
                                {
                                    actions.OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent();
                                }
                                break;
                            case SelfNearestToOpponentWithPuckStates.CannotStrikeOpponent:
                                {
                                    actions.FreePuck_SelfNearestToPuckAction();
                                }
                                break;
                        }
                    }
                    break;
                case OpponentHavePuckStates.TeammatesNearestToOpponentWithPuck:
                    {
                        var selfNearestToOpponentWithPuckState = world.SelfNearestToOpponentWithPuckState(self, game);
                        switch (selfNearestToOpponentWithPuckState)
                        {
                            case SelfNearestToOpponentWithPuckStates.CanStrikeOpponent:
                                {
                                    actions.OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent();
                                }
                                break;
                            case SelfNearestToOpponentWithPuckStates.CannotStrikeOpponent:
                                {
                                    actions.FreePuck_SelfNearestToPuckAction();
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private static void MeHavePuck(World world, Game game, Hockeyist self, Actions actions, CurrentSituation currentSituation)
        {
            var havePuckState = world.HavePuckState(self);
            switch (havePuckState)
            {
                case HavePuckStates.SelfHavePuck:
                    {
                        var selfHavePuckState = world.SelfHavePuckState(self, currentSituation, game);
                        switch (selfHavePuckState)
                        {
                            case SelfHavePuckStates.MoveToBestStrikePosition:
                                {
                                    actions.HavePuck_SelfHavePuck_MoveToBestStrikePosition();
                                }
                                break;
                            case SelfHavePuckStates.Strike:
                                {
                                    actions.HavePuck_SelfHavePuck_Strike();
                                }
                                break;
                            case SelfHavePuckStates.Swing:
                                {
                                    actions.HavePuck_SelfHavePuck_Swing();
                                }
                                break;
                            case SelfHavePuckStates.TurnToStrike:
                                {
                                    actions.HavePuck_SelfHavePuck_TurnToStrike();
                                }
                                break;
                            case SelfHavePuckStates.MoveToRetreatPosition:
                                {
                                    actions.HavePuck_GoToRetreatPosition();
                                }
                                break;
                        }
                    }
                    break;
                case HavePuckStates.TeammateHavePuck:
                    {
                        actions.FreePuck_TeammateNearestToPuck();
                    }
                    break;
            }
        }

    }

    public enum PuckStates
    {
        HavePuck,
        FreePuck,
        OpponentHavePuck,
        Swing
    }

    public enum HavePuckStates
    {
        SelfHavePuck,
        TeammateHavePuck
    }

    public enum FreePuckStates
    {
        SelfNearestToPuck,
        TeammateNearestToPuck
    }

    public enum OpponentHavePuckStates
    {
        SelfNearestToOpponentWithPuck,
        TeammatesNearestToOpponentWithPuck
    }

    public enum SelfHavePuckStates
    {
        MoveToBestStrikePosition,
        TurnToStrike,
        Strike,
        Swing,
        MoveToRetreatPosition
    }

    public enum SelfNearestToOpponentWithPuckStates
    {
        CanStrikeOpponent,
        CannotStrikeOpponent
    }

    public static class Constants
    {
        public const double StrikeAngle = 1.0D * Math.PI / 180.0D;

        public const double DangerDistanceToOpponent = 70;

        public const double DistanceToStrike = 250;

        public const double DistanceFromNetBorderToBestHitTarget = 20;

        public const double RetreatX = 300;
    }

    public class Actions
    {
        private World world;
        private Game game;
        private Hockeyist self;
        private Move move;
        private CurrentSituation currentSituation;

        public Actions(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
        {
            this.world = world;
            this.game = game;
            this.self = self;
            this.move = move;
            this.currentSituation = currentSituation;
        }

        public void FreePuck_SelfNearestToPuckAction()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
            move.Action = ActionType.TakePuck;
        }

        public void FreePuck_TeammateNearestToPuck()
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
                this.move.SpeedUp = CalculateOptimalSpeed(1, this.move.Turn);
            }
            else
            {
                this.move.Turn = this.self.GetAngleTo(this.world.Puck);
                this.move.SpeedUp = 0;
            }
        }

        public void HavePuck_SelfHavePuck_Strike()
        {
            move.SpeedUp = 1;
            move.Action = ActionType.Strike;
        }

        public void HavePuck_SelfHavePuck_Swing()
        {
            move.SpeedUp = 1;
            move.Action = ActionType.Swing;
        }

        public void HavePuck_SelfHavePuck_MoveToBestStrikePosition()
        {
            var bestStrikePosition = GetBestPositionToAttack(self, world);

            move.Turn = self.GetAngleTo(bestStrikePosition.X, bestStrikePosition.Y);
            move.SpeedUp = 1;//CalculateOptimalSpeed(1, move.Turn);
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
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
            move.Action = ActionType.Strike;
        }

        public void HavePuck_GoToRetreatPosition()
        {
            var basePoint = Manager.RetreatPosition;

            move.Turn = self.GetAngleTo(basePoint.X, basePoint.Y);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
        }

        public static double CalculateOptimalSpeed(double speed, double turn)
        {
            var modTurn = Math.Abs(turn);
            if(modTurn > Math.PI)
            {
                throw new NotSupportedException();
            }

            if(modTurn > Math.PI / 2)
            {
                return -1;
            }

            if(modTurn == 0 || modTurn < Math.PI / 8)
            {
                return speed;
            }

            return speed * (1 - modTurn / Math.PI);
        }

        public static Point GetBestPositionToAttack(Hockeyist self, World world)
        {
            var minDistOpponentToTop = world.OpponentTeam().Select(x=>x.GetDistanceTo(Manager.BestTopStrikePosition)).Min();
            var minDistOpponentToBottom = world.OpponentTeam().Select(x => x.GetDistanceTo(Manager.BestBottomStrikePosition)).Min();
            var selfDistToTop = self.GetDistanceTo(Manager.BestTopStrikePosition);
            var selfDistToBottom = self.GetDistanceTo(Manager.BestBottomStrikePosition);
            if(Math.Min(selfDistToTop, selfDistToBottom) > 300)
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
            //return GetBestPositionToAttack(self) == Manager.BestTopStrikePosition ? Manager.BestBottomHitPosition : Manager.BestTopHitPosition;
            //return self.GetDistanceTo(Manager.BestTopHitPosition.X, Manager.BestTopHitPosition.Y) >
            //    self.GetDistanceTo(Manager.BestBottomHitPosition.X, Manager.BestBottomHitPosition.Y)
            //    ? Manager.BestTopHitPosition
            //    : Manager.BestBottomHitPosition;
            return currentSituation.BestHitPosition;
        }
    }
}
