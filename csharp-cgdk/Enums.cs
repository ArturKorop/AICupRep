using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
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
}
