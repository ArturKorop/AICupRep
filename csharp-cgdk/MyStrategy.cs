using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Hockeyist self, World world, Game game, Move move)
        {
            Manager.SetRoles(world, game);

            StateMachine.Run(world, move, self, game);
        }
    }  
}