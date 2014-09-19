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
                        actions.Strike();
                    }
                    break;
            }
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
                                    actions.Strike();
                                }
                                break;
                            case SelfHavePuckStates.Swing:
                                {
                                    actions.Swing();
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
}
