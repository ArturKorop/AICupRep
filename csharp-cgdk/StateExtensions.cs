using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class StateExtensions
    {
        public static PuckStates PuckState(this World world, Hockeyist self)
        {
            return
                self.State == HockeyistState.Swinging ? PuckStates.Swing :
                world.Puck.OwnerPlayerId == world.GetMyPlayer().Id ? PuckStates.HavePuck :
                world.Puck.OwnerPlayerId == world.GetOpponentPlayer().Id ? PuckStates.OpponentHavePuck :
                PuckStates.FreePuck;
        }

        public static HavePuckStates HavePuckState(this World world, Hockeyist self)
        {
            return world.Puck.OwnerHockeyistId == self.Id ? HavePuckStates.SelfHavePuck : HavePuckStates.TeammateHavePuck;
        }

        public static FreePuckStates FreePuckState(this World world, Hockeyist self)
        {
            Manager.RetreatList[self.Id] = null;
            var selfDistanceToPuck = self.GetDistanceTo(world.Puck);

            return world.Teammates(self).Max(x => x.GetDistanceTo(world.Puck)) < selfDistanceToPuck
                ? FreePuckStates.TeammateNearestToPuck
                : FreePuckStates.SelfNearestToPuck;
        }

        public static OpponentHavePuckStates OpponentHavePuckState(this World world, Hockeyist self)
        {
            Manager.RetreatList[self.Id] = null;

            var opponentWithPuck = world.Hockeyists.Single(x => x.Id == world.Puck.OwnerHockeyistId);
            var selfDistanceToOpponentWithPuck = self.GetDistanceTo(opponentWithPuck);

            return world.Teammates(self).Max(x => x.GetDistanceTo(opponentWithPuck)) > selfDistanceToOpponentWithPuck
                ? OpponentHavePuckStates.TeammatesNearestToOpponentWithPuck
                : OpponentHavePuckStates.SelfNearestToOpponentWithPuck;
        }

        public static SelfNearestToOpponentWithPuckStates SelfNearestToOpponentWithPuckState(this World world, Hockeyist self, Game game)
        {
            var opponentWithPuck = world.HockeyistWithPuck();
            var distanceToOpponent = self.GetDistanceTo(opponentWithPuck);
            var distanceToPuck = self.GetDistanceTo(world.Puck);
            var angleToOpponent = self.GetAngleTo(opponentWithPuck);
            var angleToPuck = self.GetAngleTo(world.Puck);

            if (distanceToPuck <= game.StickLength && Math.Abs(angleToPuck) <= game.StickSector / 2)
            {
                return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
            }

            if (distanceToOpponent <= game.StickLength && Math.Abs(angleToOpponent) <= game.StickSector / 2)
            {
                if (distanceToPuck <= game.StickLength && Math.Abs(angleToPuck) <= game.StickSector / 2)
                {
                    return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
                }
                else
                {
                    return SelfNearestToOpponentWithPuckStates.CanStrikeOpponent;
                }
            }
            else
            {
                return SelfNearestToOpponentWithPuckStates.CannotStrikeOpponent;
            }

        }

        public static SelfHavePuckStates SelfHavePuckState(this World world, Hockeyist self, CurrentSituation currentSituation, Game game)
        {
            if (self.State == HockeyistState.Swinging)
            {
                return SelfHavePuckStates.Strike;
            }

            var isRetreat = Manager.RetreatList[self.Id];
            if (isRetreat != null)
            {
                var basePoint = Manager.RetreatPosition;
                if (self.GetDistanceTo(basePoint.X, basePoint.Y) > 150)
                {
                    return SelfHavePuckStates.MoveToRetreatPosition;
                }
                else
                {
                    Manager.RetreatList[self.Id] = null;
                }
            }

            var distanceToNet = self.GetDistanceTo(Manager.OpponentNetCenter.X, Manager.OpponentNetCenter.Y);
            var bestPosToAttack = Actions.GetBestPositionToAttack(self, world);
            var distanceToBestStrikePosition = self.GetDistanceTo(bestPosToAttack.X, bestPosToAttack.Y);
            var distanceBetweenNetAndBestStrikePosition = bestPosToAttack.DistanceTo(Manager.OpponentNetCenter);

            if (distanceToNet < distanceBetweenNetAndBestStrikePosition || distanceToBestStrikePosition < 50 || distanceToNet < distanceToBestStrikePosition)
            {
                var bestHitPosition = Actions.GetBestHitPosition(self, currentSituation);
                var nearestOpponentDistanceToStrikeVector = world.Hockeyists.Where(x => !x.IsTeammate).Select(x => x.DistanceToSegment(self.ToPoint(), bestHitPosition)).Min();
                if (nearestOpponentDistanceToStrikeVector < world.Puck.Radius / 2)
                {
                    Manager.RetreatList[self.Id] = SelfHavePuckStates.MoveToRetreatPosition;

                    return SelfHavePuckStates.MoveToRetreatPosition;
                }
                else
                {
                    if (Math.Abs(self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y)) < Constants.StrikeAngle)
                    {
                        if (self.GetDistanceTo(self.NearestOpponentTime(world, game)) > Constants.DangerDistanceToOpponent && self.GetDistanceTo(bestHitPosition.X, bestHitPosition.Y) > Constants.DistanceToStrike)
                        {
                            return SelfHavePuckStates.Swing;
                        }
                        else
                        {
                            return SelfHavePuckStates.Strike;
                        }
                    }
                    else
                    {
                        return SelfHavePuckStates.TurnToStrike;
                    }
                }
            }
            else
            {
                return SelfHavePuckStates.MoveToBestStrikePosition;
            }
        }
    }
}
