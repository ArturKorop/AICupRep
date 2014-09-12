using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class Constants
    {
        public const double StrikeAngle = 1.0D * Math.PI / 180.0D;

        public const double DangerDistanceToOpponent = 70;

        public const double DistanceToStrike = 280;

        public const double RinkHeightToBestStrikePosition = 50;

        public const double BestRetreatDistance = 200;

        public const double DistanceFromNetBorderToBestHitTarget = 20;
    }

    public enum GameScores
    {
        Win,
        Lose,
        Draw
    }

    public enum RinkHorizontalPosition
    {
        My,
        Center,
        Opponent
    }

    public enum RinkVerticalPosition
    {
        Top,
        Center,
        Bottom
    }

    public enum StrategyTypes
    {
        Attack,
        Defence
    }

    public class RinkPosition
    {
        public RinkHorizontalPosition Horizontal { get; private set; }
        public RinkVerticalPosition Vertical { get; private set; }

        public RinkPosition(RinkHorizontalPosition horizontal, RinkVerticalPosition vertical)
        {
            this.Horizontal = horizontal;
            this.Vertical = vertical;
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

    public class Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
