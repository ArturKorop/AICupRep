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
                    if(this.Puck.Speed() <= 15)
                    {
                        this.move.Action = ActionType.TakePuck;
                    }
                    else
                    {
                        if (this.self.GetDistanceTo(this.NearestOpponent) > 240 && this.world.MyGoalie() != null && Math.Abs(this.world.MyGoalie().Y - this.self.Y) < 20)
                        {
                            this.move.Action = ActionType.TakePuck;
                        }
                        else
                        {
                            this.move.Action = ActionType.Strike;
                        }
                    }
                }
                else
                {
                    this.SetTurnAndSpeed(this.AngleToPuck);
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
        }

        public override void MeHavePuck_SelfHavePuck()
        {
            Manager.ChangeAttVsDef(this.world, this.game);
        }

        public override void MeHavePuck_TeammateHavePuck()
        {
            this.GoToDefenderPosition();
        }

        public override void OpponentHavePuck()
        {
            if (this.Puck.ToPoint().IsInMyRinkSide())
            {
                if (this.self.CanHitPuck(world, game) || this.self.CanHitOpponent(this.world, this.game, this.self.NearestOpponentDistance(this.world)))
                {
                    this.move.Action = ActionType.Strike;
                }
                else
                {
                    //var distanceToPuck = this.self.GetDistanceTo(this.Puck);
                    //if(distanceToPuck > 200)
                    //{
                    //    this.GoToDefenderPosition();
                    //}
                    //else
                    //{
                    //    this.SetTurnAndSpeed(this.self.GetAngleTo(this.Puck));
                    //}
                    this.SetTurnAndSpeed(this.self.GetAngleTo(this.Puck));
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
            var distanceToDefenderPosition = this.self.GetDistanceTo(defenderPosition);

            var multiplier = 1;
            var angleToDefenderPosition = this.self.GetAngleTo(defenderPosition);
            double angleToTurn = 0.0;
            if (Math.Abs(angleToDefenderPosition) >= Math.PI / 2)
            {
                angleToTurn = Math.PI - Math.Abs(angleToDefenderPosition);
                multiplier = -1;
            }
            else
            {
                angleToTurn = Math.Abs(angleToDefenderPosition);
            }

            angleToTurn *= angleToDefenderPosition > 0 ? -1 : 1;

            

            //var speed = this.self.Speed();


            //if(distanceToDefenderPosition < 20)
            //{
            //    this.move.Turn = this.AngleToPuck;
            //    if(this.self.Speed() > 1)
            //    {
            //        var speedUp = -multiplier;
            //        this.move.SpeedUp = speedUp;
            //    }
            //    else
            //    {
            //        this.move.SpeedUp = 0;
            //    }
            //}
            //else
            //{
            //    var dist = Math.Pow(speed, 2) / (2 * this.game.HockeyistSpeedUpFactor);
            //    this.move.Turn = angleToTurn;
            //    var speedUp = dist > distanceToDefenderPosition ? -multiplier : multiplier;
            //    this.move.SpeedUp = speedUp;
            //}

            if(distanceToDefenderPosition == 0)
            {
                this.move.Turn = this.AngleToPuck;
                this.move.SpeedUp = 0;
            }
            else if (distanceToDefenderPosition < 15)
            {
                this.move.Turn = this.AngleToPuck;
                this.move.SpeedUp = 0; 
                if (this.self.Speed() > 1)
                {
                    this.move.SpeedUp = -1 * multiplier;
                }
            }
            else if(distanceToDefenderPosition < 45)
            {
                this.move.Turn = this.AngleToPuck;
                if(this.self.Speed() < 0.1)
                {
                    this.move.SpeedUp = 1 * multiplier;
                }
            }
            else if(distanceToDefenderPosition < 60)
            {
                this.move.Turn = angleToTurn;
                this.move.SpeedUp = 0.2 * multiplier;
            }
            else
            {
                this.move.Turn = angleToTurn;
                this.move.SpeedUp = 1 * multiplier;
            }

            //if (this.self.GetDistanceTo(defenderPosition) < 15)
            //{
            //    this.move.Turn = this.self.GetAngleTo(this.Puck);
            //    this.move.SpeedUp = 0;
            //}
            //else
            //{
            //    if (this.self.GetDistanceTo(defenderPosition) < 50)
            //    {
            //        this.move.Turn = this.self.GetAngleTo(defenderPosition);
            //        if(this.self.Speed() < 5)
            //        {
            //            this.move.SpeedUp = 0.1;
            //        }
            //        else
            //        {
            //            this.move.SpeedUp = 0;
            //        }
            //    }
            //    else if (this.self.GetDistanceTo(defenderPosition) < 100)
            //    {
            //        this.move.Turn = this.self.GetAngleTo(defenderPosition);
            //        this.move.SpeedUp = 0.4;
            //    }
            //    else
            //    {
            //        this.SetTurnAndSpeed(self.GetAngleTo(defenderPosition.X, defenderPosition.Y));
            //    }
            //}
        }
    }
}
