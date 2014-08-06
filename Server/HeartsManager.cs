using System;
using System.Linq;
using Server.BasicCardGame;

namespace Server
{
    internal class HeartsManager : GameManager
    {
        public HeartsManager(Player[] players)
            : base(players)
        {
            if (players == null)
                throw new ArgumentNullException("players", "Players cant be null");
            if (players.Length != 4)
                throw new ArgumentOutOfRangeException("players", "Number of players HAS to be 4");

            Logger.Trace("Added players to game");
        }

        protected override void CheckIfHandIsFinished()
        {
            if (CurrentHand == null || CurrentHand.Count < 4) return;
            if (CurrentHand.Count > 4) throw new Exception("Too many cards on the table");//too many dicks on the dancefloor!


            //winning hand (or losing depends on what's thrown :D)
            var firstcard = CurrentHand.First().Key;
            var possibleWinners = CurrentHand.Where(o => o.Key.CardSuit == firstcard.CardSuit);
            AdvertiseWinningHand(possibleWinners.OrderBy(o => o.Key.CardValue).First().Value);
        }

        protected override void CheckIfPlayIsFinished()
        {
            if (Players.Any(player => player.CardsRemaining > 0)) return;
            if (CurrentHand != null && CurrentHand.Count > 0) return;


        }
    }
}
