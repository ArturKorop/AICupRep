using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
    public static class WorldExtensions
    {
        #region Reliable func

        /// <summary>
        /// Return opponent goalie, if it possible.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <returns>The opponent goalie.</returns>
        public static Hockeyist OpponentGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => !x.IsTeammate && x.Type == HockeyistType.Goalie);
        }

        /// <summary>
        /// Return my goalie, if it possible.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <returns>My goalie.</returns>
        public static Hockeyist MyGoalie(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.IsTeammate && x.Type == HockeyistType.Goalie);
        }

        /// <summary>
        /// Return all other teammates without goalie, resting teammates and self.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="self">The self.</param>
        /// <returns>The teammates</returns>
        public static IEnumerable<Hockeyist> Teammates(this World world, Hockeyist self)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Id != self.Id && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        /// <summary>
        /// Return all my team in the rink without goalie and resting teammates
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The teammates.</returns>
        public static IEnumerable<Hockeyist> MyTeam(this World world)
        {
            return world.Hockeyists.Where(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        /// <summary>
        /// Return all opponent team in the rink without goalie and resting teammates
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The teammates.</returns>
        public static IEnumerable<Hockeyist> OpponentTeam(this World world)
        {
            return world.Hockeyists.Where(x => !x.IsTeammate && x.Type != HockeyistType.Goalie && x.State != HockeyistState.Resting);
        }

        public static Hockeyist HockeyistWithPuck(this World world)
        {
            return world.Hockeyists.SingleOrDefault(x => x.Id == world.Puck.OwnerHockeyistId);
        }

        public static Point OpponentNetCenter(this World world)
        {
            var opponent = world.GetOpponentPlayer();

            return new Point((opponent.NetLeft + opponent.NetRight) / 2, (opponent.NetTop + opponent.NetBottom) / 2);
        }

        #endregion

        public static Hockeyist NearestTeammateToPuck(this World world)
        {
            Hockeyist nearestTeammateToPuck = null;
            var rangeToPuck = double.MaxValue;

            foreach (var hockeyist in world.MyTeam())
            {
                var distance = hockeyist.GetDistanceTo(world.Puck);
                if (nearestTeammateToPuck == null || distance < rangeToPuck)
                {
                    nearestTeammateToPuck = hockeyist;
                    rangeToPuck = distance;
                }
            }

            return nearestTeammateToPuck;
        }
    }
}
