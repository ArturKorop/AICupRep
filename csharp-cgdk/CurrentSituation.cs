using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class CurrentSituation
    {
        //public GameScores GameScore { get; set; }

        //public RinkPosition SelfPosition { get; set; }

        //public Dictionary<Hockeyist, RinkPosition> OpponentPositions;

        //public Dictionary<Hockeyist, RinkPosition> TeammatesPositions;

        public Point BestHitPosition { get; set; }

        private World world;
        private Game game;
        private Hockeyist self;

        public CurrentSituation(World world, Game game, Hockeyist self)
        {
            this.world = world;
            this.game = game;
            this.self = self;

            this.Init();
        }

        private void Init()
        {
            this.CalculateGameScore();
            this.CalculateRinkPositions();
            this.CalculateBestHitPosition();
        }

        private void CalculateGameScore()
        {
            //if (this.world.GetMyPlayer().GoalCount > this.world.GetOpponentPlayer().GoalCount)
            //{
            //    this.GameScore = GameScores.Win;
            //}
            //else if (this.world.GetMyPlayer().GoalCount < this.world.GetOpponentPlayer().GoalCount)
            //{
            //    this.GameScore = GameScores.Lose;
            //}
            //else
            //{
            //    this.GameScore = GameScores.Draw;
            //}
        }

        private void CalculateRinkPositions()
        {
            //this.SelfPosition = CalculateRinkPosition(self);
            //this.OpponentPositions = new Dictionary<Hockeyist, RinkPosition>();
            //this.TeammatesPositions = new Dictionary<Hockeyist, RinkPosition>();

            //foreach (var hockeyist in this.world.OpponentTeam())
            //{
            //    this.OpponentPositions.Add(hockeyist, CalculateRinkPosition(hockeyist));
            //}

            //foreach (var hockeyist in this.world.Teammates(this.self))
            //{
            //    this.TeammatesPositions.Add(hockeyist, CalculateRinkPosition(hockeyist));
            //}
        }

        private void CalculateBestHitPosition()
        {
            var opponent = world.GetOpponentPlayer();
            var opponentGoalie = this.world.OpponentGoalie();
            Point opponentGoaliePosition = null;
            if(opponentGoalie == null)
            {
                opponentGoaliePosition = Manager.FieldCenter;
            }
            else
            {
                opponentGoaliePosition = opponentGoalie.ToPoint();
            }

            var hitX = opponent.NetFront;
            if (opponentGoaliePosition.Y < Manager.FieldCenter.Y)
            {
                this.BestHitPosition = new Point(hitX, opponent.NetBottom - Constants.DistanceFromNetBorderToBestHitTarget);
            }
            else
            {
                this.BestHitPosition = new Point(hitX, opponent.NetTop + Constants.DistanceFromNetBorderToBestHitTarget);
            }
        }

        //private static RinkPosition CalculateRinkPosition(Hockeyist hockeyist)
        //{
        //    var x = hockeyist.X;
        //    var y = hockeyist.Y;

        //    RinkHorizontalPosition horizontalPosition;
        //    if (IsBetweenCoord(x, Manager.MyNetCenter.X, Manager.MyThirdX))
        //    {
        //        horizontalPosition = RinkHorizontalPosition.My;
        //    }
        //    else if (IsBetweenCoord(x, Manager.MyThirdX, Manager.OpponentThirdX))
        //    {
        //        horizontalPosition = RinkHorizontalPosition.Center;
        //    }
        //    else
        //    {
        //        horizontalPosition = RinkHorizontalPosition.Opponent;
        //    }

        //    RinkVerticalPosition verticalPosition;
        //    if (y < Manager.TopThirdY)
        //    {
        //        verticalPosition = RinkVerticalPosition.Top;
        //    }
        //    else if (y > Manager.BottomThirdY)
        //    {
        //        verticalPosition = RinkVerticalPosition.Bottom;
        //    }
        //    else
        //    {
        //        verticalPosition = RinkVerticalPosition.Center;
        //    }

        //    return new RinkPosition(horizontalPosition, verticalPosition);
        //}
    }
}
