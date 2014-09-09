using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        private const double StrikeAngle = 1.0D * Math.PI / 180.0D;

        public void Move(Hockeyist self, World world, Game game, Move move)
        {
            Manager.SetRoles(world, game);

             if(self.IsForward())
             {
                 if(world.Puck.OwnerPlayerId == world.GetMyPlayer().Id)
                 {
                     if(world.Puck.OwnerHockeyistId == self.Id)
                     {
                         if(self.State == HockeyistState.Swinging)
                         {
                             move.Action = ActionType.Strike;

                             return;
                         }

                         var bestPosition = GetBestPositionToAttack(self);
                         if(self.GetDistanceTo(bestPosition.X, bestPosition.Y) > 50)
                         {
                             move.SpeedUp = 1;
                             move.Turn = self.GetAngleTo(bestPosition.X, bestPosition.Y);
                         }
                         else
                         {
                             var bestHitPosition = GetBestHitPosition(self);
                             if (Math.Abs(self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y)) < StrikeAngle)
                             {
                                 move.Action = ActionType.Swing;
                             }
                             else
                             {
                                 move.SpeedUp = -1;
                                 move.Turn = self.GetAngleTo(bestHitPosition.X, bestHitPosition.Y);
                             }
                         }
                     }
                     else
                     {
                         move.SpeedUp = 1;
                         move.Turn = self.GetAngleTo(world.Puck);
                         move.Action = ActionType.TakePuck;
                     }
                 }
                 else
                 {
                     move.SpeedUp = 1;
                     move.Turn = self.GetAngleTo(world.Puck);
                     move.Action = ActionType.TakePuck;
                 }
             }
             else if(self.IsDeffender())
             {
                 if (world.Puck.OwnerPlayerId == world.GetMyPlayer().Id)
                 {
                     if (world.Puck.OwnerHockeyistId == self.Id)
                     {
                         move.SpeedUp = 1;
                         move.Turn = self.GetAngleTo(world.Hockeyists.Single(x => x.IsTeammate && x.IsForward()));
                     }
                 }
                 else
                 {
                     move.SpeedUp = 1;
                     move.Turn = self.GetAngleTo(world.Puck);
                     move.Action = ActionType.TakePuck;
                 }
             }

            Console.WriteLine(string.Format("[{0}]: [{1},{2}], Speed: {3}, Angle: {4}, Role: {5}", self.Id, self.X, self.Y, self.AngularSpeed, self.Angle, self.IsForward() ? "Attacker" : "Defender"));
            //if (self.State == HockeyistState.Swinging)
            //{
            //    move.Action = ActionType.Strike;

            //    return;
            //}

            //if (world.Puck.OwnerPlayerId == self.PlayerId)
            //{
            //    if (world.Puck.OwnerPlayerId == self.Id)
            //    {
            //        Player opponentPlayer = world.GetOpponentPlayer();

            //        double netX = 0.5D*(opponentPlayer.NetBack + opponentPlayer.NetFront);
            //        double netY = 0.5D*(opponentPlayer.NetBottom + opponentPlayer.NetTop);
            //        netY += (self.Y < netY ? 0.5D : -0.5D)*game.GoalNetHeight;

            //        double angleToNet = self.GetAngleTo(netX, netY);
            //        move.Turn = angleToNet;

            //        if (Math.Abs(angleToNet) < StrikeAngle)
            //        {
            //            move.Action = ActionType.Swing;
            //        }
            //    }
            //    else
            //    {
            //        Hockeyist nearestOpponent = GetNearestOpponent(self.X, self.Y, world);
            //        if (nearestOpponent != null)
            //        {
            //            if (self.GetDistanceTo(nearestOpponent) > game.StickLength)
            //            {
            //                move.SpeedUp = 1;
            //            }
            //            else if (Math.Abs(self.GetAngleTo(nearestOpponent)) < 0.5D*game.StickSector)
            //            {
            //                move.Action = ActionType.Strike;
            //            }
            //            move.Turn = self.GetAngleTo(nearestOpponent);
            //        }
            //    }
            //}
            //else
            //{
            //    move.SpeedUp = 1;
            //    move.Turn = self.GetAngleTo(world.Puck);
            //    move.Action = ActionType.TakePuck;
            //}
        }

        private static Hockeyist GetNearestOpponent(double x, double y, World world)
        {
            Hockeyist nearestOpponent = null;
            double nearestOpponentRange = 0.0D;

            foreach (Hockeyist hockeyist in world.Hockeyists)
            {
                if (hockeyist.IsTeammate || hockeyist.Type == HockeyistType.Goalie
                    || hockeyist.State == HockeyistState.KnockedDown
                    || hockeyist.State == HockeyistState.Resting)
                {
                    continue;
                }

                double opponentRange = GetDistance(x,y, hockeyist.X, hockeyist.Y);

                if (nearestOpponent == null || opponentRange < nearestOpponentRange)
                {
                    nearestOpponent = hockeyist;
                    nearestOpponentRange = opponentRange;
                }
            }

            return nearestOpponent;
        }

        private static double GetDistance(double x, double y, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2));
        }

        public static Point GetBestPositionToAttack(Hockeyist self)
        {
            if(self.GetDistanceTo(Manager.BestTopStrikePosition.X, Manager.BestTopStrikePosition.Y) > self.GetDistanceTo(Manager.BestBottomStrikePosition.X, Manager.BestBottomStrikePosition.Y))
            {
                 return Manager.BestBottomStrikePosition;
            }

            return Manager.BestTopStrikePosition;
        }

        public static Point GetBestHitPosition(Hockeyist self)
        {
            return GetBestPositionToAttack(self) == Manager.BestTopStrikePosition ? Manager.BestTopHitPosition : Manager.BestBottomHitPosition;
        }
    }
}