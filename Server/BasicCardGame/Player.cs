using System.Collections.Generic;
using Shared.Cards;

namespace Server.BasicCardGame
{
    public class Player
    {
        public string Name { get; set; }
        private List<Card> Cards { get; set; }
        public int CardsRemaining { get { return Cards.Count; } }

        public Player(string name)
        {
            Name = name;
        }

        internal void ClearCards()
        {
            Cards.Clear();
        }

        public void TakeCards(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);
        }

        public bool HasCard(Card card)
        {
            return Cards.Contains(card);
        }
    }
}
