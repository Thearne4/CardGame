using System;
using System.Collections.Generic;

namespace Shared.Cards
{
    public class Deck
    {
        private readonly Random _random = new Random();

        private List<Card> CardsInDeck { get; set; }
        public int RemainingCardsInDeck { get { return CardsInDeck.Count; } }

        public Deck()
        {
            ResetDeck();
        }

        public void ResetDeck()
        {
            CardsInDeck = new List<Card>();
            foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                foreach (Card.Value value in Enum.GetValues(typeof(Card.Value)))
                {
                    CardsInDeck.Add(new Card(suit, value));
                }
            }
        }
        public void ShuffleDeck()
        {
            CardsInDeck.Shuffle();
        }
        internal List<Card> GetDeck()
        {
            return CardsInDeck;
        }
        public Card GetRandomCard(bool removeFromDeck = true)
        {
            Card card = CardsInDeck[_random.Next(CardsInDeck.Count)];
            if (removeFromDeck) CardsInDeck.Remove(card);
            return card;
        }
        public IEnumerable<Card> GetRandomCards(int numberOfCards, bool removeFromDeck = true)
        {
            for (int iCards = 0; iCards < numberOfCards; iCards++)
                yield return GetRandomCard(removeFromDeck);
        }
        public Card GetTopCard(bool removeFromDeck = true)
        {
            Card card = CardsInDeck[0];
            if (removeFromDeck) CardsInDeck.Remove(card);
            return card;
        }
        public IEnumerable<Card> GetTopCards(int numberOfCards, bool removeFromDeck = true)
        {
            if (removeFromDeck)
                for (int iCards = 0; iCards < numberOfCards; iCards++)
                    yield return GetTopCard();
            else
                for (int iCards = 0; iCards < numberOfCards; iCards++)
                    yield return CardsInDeck[iCards];

        }
    }

    internal static class ListExtention
    {
        private static readonly Random Rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
