//using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
//{
//    public class DefenceStrategy : Strategy
//    {
//        public DefenceStrategy(World world, Game game, Hockeyist self, Move move, CurrentSituation currentSituation)
//            : base(world, game, self, move, currentSituation)
//        {
//        }

//        public override void HavePuck_TeammateHavePuck()
//        {
//#warning Calculate best def position based on goalie position
//            var targetPosition = Manager.DefenderPosition;

//            if (self.GetDistanceTo(targetPosition) > 50)
//            {
//                this.move.Turn = this.self.GetAngleTo(targetPosition.X, targetPosition.Y);
//                this.move.SpeedUp = Strategy.CalculateOptimalSpeed(1, this.move.Turn);
//            }
//            else
//            {
//                this.move.Turn = this.self.GetAngleTo(Manager.OpponentNetCenter.X, Manager.OpponentNetCenter.Y);
//                this.move.SpeedUp = 0;
//            }
//        }

//        public override void FreePuck_TeammateNearestToPuck()
//        {
//            var targetPosition = new Point(Manager.MyNetCenter.X, this.world.Puck.Y);

//            if (self.GetDistanceTo(targetPosition) > 50)
//            {
//                this.move.Turn = this.self.GetAngleTo(targetPosition.X, targetPosition.Y);
//                this.move.SpeedUp = Strategy.CalculateOptimalSpeed(1, this.move.Turn);
//            }
//            else
//            {
//                this.move.Turn = this.self.GetAngleTo(Manager.OpponentNetCenter.X, Manager.OpponentNetCenter.Y);
//                this.move.SpeedUp = 0;
//            }

//            this.move.Action = ActionType.TakePuck;
//        }

//        public override void OpponentHavePuck_TeammateNearestToOpponentWithPuck()
//        {
//            this.HavePuck_TeammateHavePuck();
//        }
//    }
//}
