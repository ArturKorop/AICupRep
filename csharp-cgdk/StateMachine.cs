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
            Strategy strategy = null;
            var strategyType = CalculateStrategyType(currentSituation);
            strategy = CreateStrategy(strategyType, world, game, self, move, currentSituation);

            var puckState = world.PuckState(self);
            switch(puckState)
            {
                case PuckStates.FreePuck:
                    {
                        strategy.FreePuck();
                    }
                    break;
                case PuckStates.HavePuck:
                    {
                        var havePuckState = world.HavePuckState(self);
                        switch (havePuckState)
                        {
                            case HavePuckStates.SelfHavePuck:
                                {
                                    strategy.HavePuck_SelfHavePuck();
                                }
                                break;
                            case HavePuckStates.TeammateHavePuck:
                                {
                                    strategy.HavePuck_TeammateHavePuck();
                                }
                                break;
                        }
                    }
                    break;
                case PuckStates.OpponentHavePuck:
                    {
                        strategy.OpponentHavePuck();
                    }
                    break;
                case PuckStates.Swing:
                    {
                        strategy.Strike();
                    }
                    break;
            }
        }

        private static StrategyTypes CalculateStrategyType(CurrentSituation currentSituation)
        {
            return currentSituation.GameScore == GameScores.Win
                ? StrategyTypes.Defence
                : StrategyTypes.Attack;
        }

        private static Strategy CreateStrategy(StrategyTypes strategyType, World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
        {
            Strategy strategy = null;
            switch (strategyType)
            {
                case StrategyTypes.Attack:
                    {
                        strategy = new AttackStrategy(world, game, self, move, currentSituation);
                    }
                    break;
                case StrategyTypes.Defence:
                    {
                        strategy = new AttackStrategy(world, game, self, move, currentSituation);
                    }
                    break;
            }

            return strategy;
        }
    }

    
}
