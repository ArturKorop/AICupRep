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
            var actions = self.IsAttacker() ? new AttackerActions(world, game, self, move, currentSituation) as Actions : new DefenderActions(world, game, self, move, currentSituation) as Actions;
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
                        actions.FreePuck_TeammateNearestToPuck();
                    }
                    break;
            }
        }

        private static void OpponentHavePuckState(World world, Game game, Hockeyist self, Actions actions)
        {
            actions.OpponentHavePuck();
        }

        private static void MeHavePuck(World world, Game game, Hockeyist self, Actions actions, CurrentSituation currentSituation)
        {
            var havePuckState = world.HavePuckState(self);
            switch (havePuckState)
            {
                case HavePuckStates.SelfHavePuck:
                    {
                        actions.MeHavePuck_SelfHavePuck();
                    }
                    break;
                case HavePuckStates.TeammateHavePuck:
                    {
                        actions.MeHavePuck_TeammateHavePuck();
                    }
                    break;
            }
        }
    }
}
