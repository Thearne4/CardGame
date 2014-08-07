using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Server.Annotations;
using Shared.Cards;

namespace Server.BasicCardGame
{
    public class Player : INotifyPropertyChanged
    {
        #region Fields & Properties
        private List<Card> _cards;
        public string Name { get; set; }

        private List<Card> Cards
        {
            get { return _cards; }
            set
            {
                var old = _cards;
                _cards = value;
                if (old != value) OnPropertyChanged();
            }
        }

        public int CardsRemaining { get { return Cards.Count; } }
        #endregion

        public Player(string name)
        {
            Name = name;
            Cards = new List<Card>();
        }

        #region Methods
        internal void ClearCards()
        {
            Cards.Clear();
        }

        internal void TakeCards(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);
        }

        internal bool HasCard(Card card)
        {
            return Cards.Contains(card);
        }

        private void CardPlayed(Card card)
        {
            if (!Cards.Contains(card)) throw new ArgumentException("Player doesn't have this card", "card");

            Cards.Remove(card);
        }

        internal void PlayCard(Card card)
        {
            //TODO: Dissallow playing hearts if not yet played
            //TODO: Dissallow playing off suit
            if (!Cards.Contains(card)) throw new ArgumentException("Player doesn't have this card", "card");

            //TODO: Make it so this method can only be called from tcp class (and test unit)

            if (PlayerWantsToPlayCard == null) return;
            
            if (PlayerWantsToPlayCard.Invoke(this, new PlayerWantsToPlayCardEventArgs(card)))
                CardPlayed(card);
        }
        #endregion

        #region Events
        public event PlayerWantsToPlayCardEventHandler PlayerWantsToPlayCard;
        public delegate bool PlayerWantsToPlayCardEventHandler(object sender, PlayerWantsToPlayCardEventArgs args);
        public class PlayerWantsToPlayCardEventArgs : EventArgs
        {
            public Card Card { get; private set; }

            public PlayerWantsToPlayCardEventArgs(Card card)
            {
                Card = card;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        // ReSharper disable once InconsistentNaming
        public void TESTPlayRandomCard(Random rnd)
        {
            PlayCard(Cards[rnd.Next(0, Cards.Count)]);
        }
    }
}
