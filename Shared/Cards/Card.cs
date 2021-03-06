﻿using System;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;

namespace Shared.Cards
{
    public class Card : IComparer
    {
        public Suit CardSuit { get; private set; }
        public Value CardValue { get; private set; }

        public Card(Suit suit, Value value)
        {
            this.CardSuit = suit;
            this.CardValue = value;
        }


        public enum Suit : byte
        {
            Hearts = 0x1,
            Diamonds = 0x2,
            Clubs = 0x3,
            Spades = 0x4
        }
        public enum Value : byte
        {
            Ace = 0x0,
            Two = 0x1,
            Three = 0x2,
            Four = 0x3,
            Five = 0x4,
            Six = 0x5,
            Seven = 0x6,
            Eight = 0x7,
            Nine = 0x8,
            Ten = 0x9,
            Jack = 0xA,
            Queen = 0xB,
            King = 0xC
        }

        public int Compare(object x, object y)
        {
            if (!(x is Card)) throw new ArgumentException("Not of type Card", "x");
            if (!(y is Card)) throw new ArgumentException("Not of type Card", "y");

            if ((byte)(x as Card).CardSuit < (byte)(y as Card).CardSuit) return 1;
            if ((byte)(x as Card).CardSuit > (byte)(y as Card).CardSuit) return -1;
            if ((byte)(x as Card).CardValue < (byte)(y as Card).CardValue) return 1;
            if ((byte)(x as Card).CardValue > (byte)(y as Card).CardValue) return -1;
            return 0;
        }
    }
}
