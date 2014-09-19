using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class DefenderActions : Actions
    {
        public DefenderActions(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation) : base(world, game, self, move, currentSituation)
        {
        }

        public override void FreePuck_SelfNearestToPuckAction()
        {
            if(this.Puck.ToPoint().IsInMyRinkSide())
            {
                this.SetTurnAndSpeed(self.GetAngleTo(this.Puck));
                this.move.Action = ActionType.TakePuck;
            }
            else
            {
                this.GoToDefenderPosition();
            }
        }

        public override void FreePuck_TeammateNearestToPuck()
        {
            this.GoToDefenderPosition();
        }

        private void GoToDefenderPosition()
        {
            var defencePoint = Manager.DefenderPosition;
            this.SetTurnAndSpeed(self.GetAngleTo(Manager.DefenderPosition.X, Manager.DefenderPosition.Y));
        }
    }
}
