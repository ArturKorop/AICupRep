using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public class AttackerActions : Actions
    {
        private bool IsInAttackRinkSide
        {
            get
            {
                return this.self.X.IsBetweenCoord(BestPositionToAttack.X, Manager.OpponentNetCenter.X);
            }
        }

        public AttackerActions(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
            : base(world, game, self, move, currentSituation)
        {
        }

        public override void FreePuck_SelfNearestToPuckAction()
        {
            this.SetTurnAndSpeed(this.AngleToPuck);
            this.move.Action = ActionType.TakePuck;
        }

        public override void FreePuck_TeammateNearestToPuck()
        {
            this.AttackerWaitPuck();
        }

        public override void OpponentHavePuck()
        {
            if (this.CanHitPuck && this.CanDoAction)
            {
                if (this.Puck.ToPoint().IsInOpponentSide())
                {
                    this.move.Action = ActionType.TakePuck;
                }
                else
                {
                    this.move.Action = ActionType.Strike;
                }
            }
            else
            {
                if(this.Puck.ToPoint().IsInOpponentSide())
                {
                    this.SetTurnAndSpeed(this.AngleToPuck);
                }
                else
                {
                    this.AttackerWaitPuck();
                }
            }
        }

        public override void MeHavePuck_TeammateHavePuck()
        {
            this.AttackerWaitPuck();
        }

        public override void MeHavePuck_SelfHavePuck()
        {
            if(this.IsInAttackRinkSide)
            {
                if (Math.Abs(this.self.X - Manager.OpponentNetCenter.X) <= 200)
                {
                    this.SetTurnAndSpeed(this.self.GetAngleTo(Manager.DefenderPosition));
                }
                else 
                {
                    var hitPosition = GetBestHitPosition(this.self, this.currentSituation);
                    var angleToHitPosition = this.self.GetAngleTo(hitPosition);

                    if (Math.Abs(angleToHitPosition) <= Constants.StrikeAngle)
                    {
                        if (this.CanDoAction)
                        {
                            if (this.NearestOpponent.CanHitPuck(this.world, this.game) && this.NearestOpponent.RemainingCooldownTicks < 10)
                            {
                                this.Strike();
                            }
                            else
                            {
                                this.Swing();
                            }
                        }
                        else
                        {
                            this.move.SpeedUp = 1;
                        }
                    }
                    else
                    {
                        if (Math.Abs(this.self.GetAngleTo(hitPosition)) >= Math.PI / 2) //&& this.self.GetDistanceTo(this.NearestOpponent) <= 200)
                        {
                            var anglesToTeammates = this.world.Teammates(this.self).Select(x => this.self.GetAngleTo(x));
                            var minAngleToTeammates = anglesToTeammates.Min(x => Math.Abs(x));
                            var angleToTeammate = anglesToTeammates.First(x => Math.Abs(x) == minAngleToTeammates);
                            if (minAngleToTeammates <= this.game.PassSector / 2 && this.CanDoAction)
                            {
                                this.move.PassAngle = angleToTeammate;
                                this.move.PassPower = 0.5;
                                this.move.Action = ActionType.Pass;
                            }
                            else
                            {
                                this.SetTurnAndSpeed(angleToHitPosition);
                            }
                        }
                        else
                        {
                            this.SetTurnAndSpeed(angleToHitPosition);
                        }
                    }
                }
            }
            else
            {
                var minDistToOpponents = this.world.OpponentTeam().Select(x => x.DistanceToSegment(this.self.ToPoint(), this.BestPositionToAttack)).Min();

                if (minDistToOpponents < 200)
                {
                    var anglesToTeammates = this.world.Teammates(this.self).Select(x => this.self.GetAngleTo(x));
                    var minAngleToTeammates = anglesToTeammates.Min(x => Math.Abs(x));
                    var angleToTeammate = anglesToTeammates.First(x => Math.Abs(x) == minAngleToTeammates);
                    if (minAngleToTeammates <= this.game.PassSector / 2 && this.CanDoAction)
                    {
                        this.move.PassAngle = angleToTeammate;
                        this.move.PassPower = 0.5;
                        this.move.Action = ActionType.Pass;
                    }
                    else
                    {
                        this.SetTurnAndSpeed(this.self.GetAngleTo(Manager.DefenderPosition));
                    }
                }
                else
                {
                    this.SetTurnAndSpeed(this.AngleToBestAttackPosition);
                }
            }
        }
    }
}
