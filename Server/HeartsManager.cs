using System;
using System.Collections.Generic;
using System.Linq;
using Server.BasicCardGame;
using Shared.Cards;

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

            //winning hand (or losing depends on what's played :D)
            var firstcard = CurrentHand.First().Key;
            var possibleWinners = CurrentHand.Where(o => o.Key.CardSuit == firstcard.CardSuit);
            var winningPlayer = possibleWinners.OrderBy(o => o.Key.CardValue).First().Value;

            var hand = CurrentHand.ToDictionary(player => player.Key, player => player.Value);
            _history.Add(hand, winningPlayer);

            AdvertiseWinningHand(winningPlayer);
        }

        protected override void CheckIfPlayIsFinished()
        {
            if (Players.Any(player => player.CardsRemaining > 0)) return;
            if (CurrentHand != null && CurrentHand.Count > 0) return;

            //round finished
            var scoresToAdd = new int[Players.Length];
            foreach (var round in _history)
            {
                var pointsinhand = 0;
                foreach (var card in round.Key.Keys)//all cards in round
                {
                    if (card.CardSuit == Card.Suit.Hearts) pointsinhand++;
                    else if (card.CardSuit == Card.Suit.Spades && card.CardValue == Card.Value.Queen) pointsinhand += 13;
                }
                scoresToAdd[Array.IndexOf(Players, round.Value)] += pointsinhand;
            }

            AdvertiseRoundOver(scoresToAdd);
        }
    }
}
