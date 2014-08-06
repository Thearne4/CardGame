using System;
using System.Collections.Generic;
using Shared.Cards;

namespace Server.BasicCardGame
{
    public abstract class GameManager
    {
        protected static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #region Fields

        protected Deck Deck;
        public Player[] Players;
        private int[] score;
        private List<Dictionary<Card, Player>> _history;

        private int _currentDealer;
        private int currentPlayer;
        protected Dictionary<Card, Player> CurrentHand;

        private Random random = new Random();
        #endregion

        #region Properties
        public bool DoneDealing { get { return Deck.RemainingCardsInDeck == 0; } }

        public int CurrentPlayer
        {
            get { return currentPlayer; }
        }

        public int CurrentDealer
        {
            get { return _currentDealer; }
        }

        public Dictionary<Player, int> Scores
        {
            get
            {
                if (Players.Length != score.Length) return null;
                var scores = new Dictionary<Player, int>();
                for (int iPlayer = 0; iPlayer < Players.Length; iPlayer++)
                {
                    scores.Add(Players[iPlayer], score[iPlayer]);
                }
                return scores;
            }

        }
        #endregion

        protected GameManager(Player[] players, int? firstDealer = null)
        {
            Players = players;
            score = new int[Players.Length];
            SetDealer(firstDealer);
            ResetGame();
        }

        #region Methods
        #region Setup
        internal void ResetGame()
        {
            Logger.Trace("Setting game to default values (clearing score, previous plays, ...)");
            ClearGame();
            SetDealer();
        }

        protected void ClearGame()
        {
            foreach (var gamer in Players) gamer.ClearCards();
            _history = null;
            Deck.ResetDeck();
        }
        private void SetDealer(int? idInGamers = null)
        {
            _currentDealer = idInGamers == null || idInGamers < 0 || idInGamers >= Players.Length ? random.Next(0, Players.Length) : (int)idInGamers;
        }

        internal void SetNextDealer()
        {
            var nextDealer = CurrentDealer + 1;
            if (nextDealer >= Players.Length) nextDealer = 0;
            _currentDealer = nextDealer;
        }

        public void DealEveryone(int? numCards = null, bool startAfterDealer = true)
        {
            var maxCards = (int)Math.Floor((double)Deck.RemainingCardsInDeck / Players.Length);
            if (numCards != null && numCards > maxCards) throw new ArgumentOutOfRangeException("numCards");

            if (startAfterDealer)
            {
                int iCurrentlyDealing = CurrentDealer + 1;
                int playersDealed = 0;
                do
                {
                    if (iCurrentlyDealing >= Players.Length) iCurrentlyDealing = 0;

                    Players[iCurrentlyDealing].TakeCards(Deck.GetRandomCards(numCards ?? maxCards));

                    iCurrentlyDealing++;
                    playersDealed++;
                } while (playersDealed < Players.Length);
            }
            else
                foreach (var gamer in Players)
                    gamer.TakeCards(Deck.GetRandomCards(numCards ?? maxCards));

            currentPlayer = CurrentDealer;
            SetNextPlayerAsActive();
        }

        private void SetNextPlayerAsActive()
        {
            int tmpCurrentPlayer = CurrentDealer + 1;
            if (tmpCurrentPlayer >= Players.Length) tmpCurrentPlayer = 0;
            currentPlayer = tmpCurrentPlayer;
        }
        #endregion


        public void PlayCard(Player player, Card card)
        {
            //Check if legal play
            if (Players[CurrentPlayer] != player) throw new ArgumentException("It is not this player's turn!", "player");
            if (!Players[CurrentPlayer].HasCard(card)) throw new ArgumentException("Player threw a card he's not supposed to have!", "card");

            //Add to public hand
            if(CurrentHand==null)CurrentHand=new Dictionary<Card, Player>();
            CurrentHand.Add(card, player);
            AdvertisePlay(card, player);

            //Checks and game progression
            CheckIfHandIsFinished();
            CheckIfPlayIsFinished();
            SetNextPlayerAsActive();
        }

        protected abstract void CheckIfHandIsFinished();
        protected abstract void CheckIfPlayIsFinished();

        private void AdvertisePlay(Card card, Player player)
        {
            try
            {
                if (CardPlayed != null) CardPlayed.Invoke(new CardPlayedEventArgs(card, player));
            }
            catch (Exception exception)
            {
                Logger.Error("Error letting players know what card was played", exception);
            }
        }
        protected void AdvertiseWinningHand(Player winningPlayer, bool clearCurrentHand = true, Dictionary<Card, Player> hand = null)
        {
            try
            {
                if (HandDone != null) HandDone.Invoke(new HandDoneEventArgs(hand ?? CurrentHand, winningPlayer));
                }
            catch (Exception exception)
            {
                Logger.Error("Error letting players know what card was played", exception);
            }
            if (clearCurrentHand) CurrentHand.Clear();
        }
        #endregion

        #region Events

        public event CardPlayedEventHandler CardPlayed;
        public delegate void CardPlayedEventHandler(CardPlayedEventArgs args);
        public class CardPlayedEventArgs : EventArgs
        {
            public Card Card { get; private set; }
            public Player Player { get; private set; }

            public CardPlayedEventArgs(Card card, Player player)
            {
                Card = card;
                Player = player;
            }
        }

        public event HandDoneEventHandler HandDone;
        public delegate void HandDoneEventHandler(HandDoneEventArgs args);
        public class HandDoneEventArgs : EventArgs
        {
            public Dictionary<Card, Player> Hand { get; private set; }
            public Player WinningPlayer { get; private set; }

            public HandDoneEventArgs(Dictionary<Card, Player> hand, Player winningPlayer)
            {
                Hand = hand;
                WinningPlayer = winningPlayer;
            }
        }
        #endregion
    }
}