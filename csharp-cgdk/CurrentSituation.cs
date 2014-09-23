using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class CurrentSituation
    {
        public Point DefenderPosition { get; set; }

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
            this.CalculateBestHitPosition();
            this.CalculateBestDefenderPositon();
        }

        private void CalculateBestHitPosition()
        {
            var opponent = world.GetOpponentPlayer();
            var opponentGoalie = this.world.OpponentGoalie();
            Point opponentGoaliePosition = null;
            if (opponentGoalie == null)
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

        private void CalculateBestDefenderPositon()
        {
            var me = this.world.GetMyPlayer();
            var goalie = this.world.MyGoalie();
            if (goalie != null)
            {
                var defX = goalie.X > Manager.FieldCenter.X ? goalie.X - Constants.RangeFromGoalieX : goalie.X + Constants.RangeFromGoalieX;
                var defY = goalie.Y >= Manager.FieldCenter.Y ? me.NetTop + Constants.RangeFromGoalieY : me.NetBottom - Constants.RangeFromGoalieY;

                this.DefenderPosition = new Point(defX, defY);
            }
            else
            {
                this.DefenderPosition = Manager.DefenderPosition;
            }

        }
    }
}
