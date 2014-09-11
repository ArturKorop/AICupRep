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
            var actions = new Actions(world, game, self, move);
            var puckState = world.PuckState(self);

            switch(puckState)
            {
                case PuckStates.FreePuck:
                    {
                        var freePuckState = world.FreePuckState(self);
                        switch(freePuckState)
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
                    break;
                case PuckStates.HavePuck:
                    {
                        var havePuckState = world.HavePuckState(self);
                        switch (havePuckState)
                        {
                            case HavePuckStates.SelfHavePuck:
                                {
                                    var selfHavePuckState = world.SelfHavePuckState(self);
                                    switch(selfHavePuckState)
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
                    break;
                case PuckStates.OpponentHavePuck:
                    {
                        var opponentHavePuckState = world.OpponentHavePuckState(self);
                        switch(opponentHavePuckState)
                        {
                            case OpponentHavePuckStates.SelfNearestToOpponentWithPuck:
                                {
                                    var selfNearestToOpponentWithPuckState = world.SelfNearestToOpponentWithPuckState(self, game);
                                    switch(selfNearestToOpponentWithPuckState)
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
        Swing
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

        public const double DistanceToStrike = 280;
    }

    public class Actions
    {
        private World world;
        private Game game;
        private Hockeyist self;
        private Move move;

        public Actions(World world, Game game, Hockeyist self, Move move)
        {
            this.world = world;
            this.game = game;
            this.self = self;
            this.move = move;
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
            var nearestOpponentToTeammateWithPuck = teammateNearestToPuck.NearestOpponent(world);

            move.Turn = self.GetAngleTo(nearestOpponentToTeammateWithPuck);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
            
            if(self.GetDistanceTo(nearestOpponentToTeammateWithPuck) < game.StickLength && Math.Abs(self.GetAngleTo(nearestOpponentToTeammateWithPuck)) < game.StickSector / 2)
            {
                move.Action = ActionType.Strike;
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
            var bestStrikePosition = GetBestPositionToAttack(self);

            move.Turn = self.GetAngleTo(bestStrikePosition.X, bestStrikePosition.Y);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
        }

        public void HavePuck_SelfHavePuck_TurnToStrike()
        {
            var bestHitPosition = GetBestHitPosition(self);

            move.SpeedUp = 0;
            move.Turn = self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y);
        }

        public void OpponentHavePuck_SelfNearestToOpponentWithPuck_CanStrikeOpponent()
        {
            move.Turn = self.GetAngleTo(world.Puck);
            move.SpeedUp = CalculateOptimalSpeed(1, move.Turn);
            move.Action = ActionType.Strike;
        }

        public static Point GetBestPositionToAttack(Hockeyist self)
        {
            if (self.GetDistanceTo(Manager.BestTopStrikePosition.X, Manager.BestTopStrikePosition.Y) > self.GetDistanceTo(Manager.BestBottomStrikePosition.X, Manager.BestBottomStrikePosition.Y))
            {
                return Manager.BestBottomStrikePosition;
            }

            return Manager.BestTopStrikePosition;
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

        public static Point GetBestHitPosition(Hockeyist self)
        {
            return GetBestPositionToAttack(self) == Manager.BestTopStrikePosition ? Manager.BestBottomHitPosition : Manager.BestTopHitPosition;
        }
    }
}
