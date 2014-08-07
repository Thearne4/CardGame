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
        protected Dictionary<Dictionary<Card, Player>, Player> _history; // Played hand as dictionary + winner of hand

        private int _currentDealer;
        private int currentPlayer;
        protected Dictionary<Card, Player> CurrentHand;

        private Random random = new Random();
        #endregion

        #region Properties
        public bool DoneDealing { get { return Deck.RemainingCardsInDeck == 0; } }

        public Player CurrentPlayer
        {
            get { return Players[currentPlayer]; }
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
            foreach (var player in Players)
            {
                player.PropertyChanged += player_PropertyChanged;
                player.PlayerWantsToPlayCard += player_PlayerWantsToPlayCard;
            }
            score = new int[Players.Length];
            Deck = new Deck();
            SetDealer(firstDealer);
            ResetGame();
        }

        #region Methods
        private void player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private bool player_PlayerWantsToPlayCard(object sender, Player.PlayerWantsToPlayCardEventArgs args)
        {
            try
            {
                PlayCard(sender as Player, args.Card);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error("Failed to play card!", exception);
            }
            return false;
        }

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
            _history = new Dictionary<Dictionary<Card, Player>, Player>();
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

        public virtual void Deal()
        {
            DealEveryone();
        }
        protected void DealEveryone(int? numCards = null, bool startAfterDealer = true)
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
            int tmpCurrentPlayer = currentPlayer + 1;
            if (tmpCurrentPlayer >= Players.Length) tmpCurrentPlayer = 0;
            currentPlayer = tmpCurrentPlayer;
        }
        #endregion


        public void PlayCard(Player player, Card card)
        {
            //Check if legal play
            if (CurrentPlayer != player) throw new ArgumentException("It is not this player's turn!", "player");
            if (!CurrentPlayer.HasCard(card)) throw new ArgumentException("Player threw a card he's not supposed to have!", "card");

            //Add to public hand and history
            if (CurrentHand == null) CurrentHand = new Dictionary<Card, Player>();
            CurrentHand.Add(card, player);
            AdvertisePlay(card, player);

            //Checks and game progression
            CheckIfHandIsFinished();
            CheckIfRoundIsFinished();
            SetNextPlayerAsActive();
        }

        protected abstract void CheckIfHandIsFinished();
        protected abstract void CheckIfRoundIsFinished();

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

        protected void AdvertiseRoundOver(int[] scoresToAdd)
        {
            if (scoresToAdd.Length != Players.Length) throw new ArgumentOutOfRangeException("scoresToAdd", "Score Array isn't the same size as player array");

            if (score == null) score = new int[Players.Length];

            for (int iPlayer = 0; iPlayer < score.Length; iPlayer++)
                score[iPlayer] += scoresToAdd[iPlayer];

            try
            {
                if (RoundDone != null) RoundDone.Invoke(new RoundDoneEventArgs(Players, score, scoresToAdd));
            }
            catch (Exception exception)
            {
                Logger.Error("Problem reporting round over", exception);
            }

            SetupNextRound();
        }

        protected virtual void SetupNextRound()
        {
            SetNextDealer();
            Deck.ResetDeck();
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

        public event RoundDoneEventHandler RoundDone;
        public delegate void RoundDoneEventHandler(RoundDoneEventArgs args);
        public class RoundDoneEventArgs : EventArgs
        {
            public RoundDoneEventArgs(Player[] players, int[] scores, int[] pointTakenThisRound)
            {
                Players = players;
                Scores = scores;
                PointTakenThisRound = pointTakenThisRound;
            }

            public Player[] Players { get; private set; }
            public int[] Scores { get; private set; }
            public int[] PointTakenThisRound { get; private set; }
        }


        #endregion
    }
}