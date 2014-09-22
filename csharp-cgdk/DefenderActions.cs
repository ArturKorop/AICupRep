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
                if(this.self.CanHitPuck(this.world, this.game))
                {
                    this.move.Action = ActionType.TakePuck;
                }
                else
                {
                    this.SetTurnAndSpeed(this.self.GetAngleTo(this.Puck));
                    this.move.Action = ActionType.TakePuck;
                }
            }
            else
            {
                this.GoToDefenderPosition();
            }
        }

        public override void FreePuck_TeammateNearestToPuck()
        {
            this.GoToDefenderPosition();
            this.move.Action = ActionType.TakePuck;
        }

        public override void MeHavePuck_SelfHavePuck()
        {
            var attacker = this.world.Teammates(this.self).First(x => x.IsAttacker());
            var angleToAttacker = this.self.GetAngleTo(attacker);

            if (this.world.OpponentTeam().Select(x => x.DistanceToSegment(this.self.ToPoint(), attacker.ToPoint())).Min() > 100)
            {

                if (Math.Abs(angleToAttacker) <= this.game.PassSector / 2)
                {
                    this.Pass(angleToAttacker, this.self.GetDistanceTo(attacker));
                }
                else
                {
                    this.move.Turn = angleToAttacker;
                    this.move.SpeedUp = 0;
                }
            }
            else
            {
                Manager.ChangeAttVsDef();
            }
        }

        public override void MeHavePuck_TeammateHavePuck()
        {
            this.GoToDefenderPosition();
        }

        public override void OpponentHavePuck()
        {
            if (this.Puck.ToPoint().IsInMyRinkSide())
            {
                if (this.self.CanHitPuck(world, game))
                {
                    this.move.Action = ActionType.TakePuck;
                }
                else
                {
                    this.GoToDefenderPosition();
                    this.move.Action = ActionType.TakePuck;
                }
            }
            else
            {
                this.GoToDefenderPosition();
            }
        }

        private void GoToDefenderPosition()
        {
            var defenderPosition = this.currentSituation.DefenderPosition;
            
            if (this.self.GetDistanceTo(defenderPosition) < 15)
            {
                this.move.Turn = this.self.GetAngleTo(this.Puck);
                this.move.SpeedUp = 0;
            }
            else
            {
                if (this.self.GetDistanceTo(defenderPosition) < 50)
                {
                    this.move.Turn = this.self.GetAngleTo(defenderPosition);
                    if(this.self.Speed() < 5)
                    {
                        this.move.SpeedUp = 0.1;
                    }
                    else
                    {
                        this.move.SpeedUp = 0;
                    }
                }
                else if (this.self.GetDistanceTo(defenderPosition) < 100)
                {
                    this.move.Turn = this.self.GetAngleTo(defenderPosition);
                    this.move.SpeedUp = 0.4;
                }
                else
                {
                    this.SetTurnAndSpeed(self.GetAngleTo(defenderPosition.X, defenderPosition.Y));
                }
            }
        }
    }
}
