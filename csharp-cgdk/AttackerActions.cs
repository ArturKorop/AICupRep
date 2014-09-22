using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class AttackerActions : Actions
    {
        public AttackerActions(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
            : base(world, game, self, move, currentSituation)
        {
        }

        public override void FreePuck_SelfNearestToPuckAction()
        {
            this.SetTurnAndSpeed(this.self.GetAngleTo(this.Puck));
            this.move.Action = ActionType.TakePuck;
        }

        public override void FreePuck_TeammateNearestToPuck()
        {
            this.AttackerWaitPuck();
        }

        public override void OpponentHavePuck()
        {
            if(this.self.CanHitPuck(this.world, this.game))
            {
                this.move.Action = ActionType.Strike;
            }
            else
            {
                this.AttackerWaitPuck();
            }
        }

        public override void MeHavePuck_TeammateHavePuck()
        {
            this.AttackerWaitPuck();
        }

        public override void MeHavePuck_SelfHavePuck()
        {
            var bestAttackerPosition = GetBestPositionToAttack(this.self, this.world);

            if(this.self.GetDistanceTo(bestAttackerPosition) < 100)
            {
                var hitPosition = GetBestHitPosition(this.self, this.currentSituation);
                var angleToHitPosition = this.self.GetAngleTo(hitPosition.X, hitPosition.Y);

                if(Math.Abs(angleToHitPosition) <= Constants.StrikeAngle)
                {
                    var nearestOpponent = this.self.NearestOpponentTime(this.world, this.game);

                    if(nearestOpponent.CanHitPuck(this.world,this.game))
                    {
                        this.Strike();
                    }
                    else
                    {
                        if(this.self.Speed() > 2)
                        {
                            this.Strike();
                        }
                        else
                        {
                            this.Swing();
                        }
                    }
                }
                else
                {
                    this.move.Turn = angleToHitPosition;
                    this.move.SpeedUp = 0;
                }
            }
            else
            {
                this.SetTurnAndSpeed(this.self.GetAngleTo(bestAttackerPosition.X, bestAttackerPosition.Y));
            }
        }
    }
}
