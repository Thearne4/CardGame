using System;
using System.IO.Ports;
using System.Linq;
using Server;
using Server.BasicCardGame;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class HeartsManagerTest
    {
        [Test]
        public void DealPlayers()
        {
            var players = new[] { new Player("Arne"), new Player("Tim"), new Player("Stijn"), new Player("Yorick") };
            var manager = new HeartsManager(players);

            Assert.That(manager.Players == players);

            manager.Deal();

            Assert.That(manager.DoneDealing);
        }

        private int _cardsPlayed;
        private int _handsPlayed;
        private int _roundsPlayed;

        [Test]
        public void PlayRound()
        {
            _roundsPlayed = 0;
            var players = new[] { new Player("Arne"), new Player("Tim"), new Player("Stijn"), new Player("Yorick") };
            var manager = new HeartsManager(players);

            manager.CardPlayed += manager_CardPlayed;
            manager.HandDone += manager_HandDone;
            manager.RoundDone += manager_RoundDone;

            PlayRound(manager);

            Assert.That(_roundsPlayed == 1);
        }
        public void PlayRound(HeartsManager manager)
        {
            _handsPlayed = 0;
            _cardsPlayed = 0;

            manager.Deal();
            Assert.That(manager.DoneDealing);
            foreach (var player in manager.Players) Assert.That(player.CardsRemaining == 13);

            var rnd = new Random();
            for (int iCardInDeck = 0; iCardInDeck < 52; iCardInDeck++)
            {
                manager.CurrentPlayer.TESTPlayRandomCard(rnd);
            }

            Assert.That(_handsPlayed == 13);
            Assert.That(_cardsPlayed == 52);
        }

        void manager_CardPlayed(GameManager.CardPlayedEventArgs args)
        {
            _cardsPlayed++;
        }
        void manager_HandDone(GameManager.HandDoneEventArgs args)
        {
            _handsPlayed++;
        }
        void manager_RoundDone(GameManager.RoundDoneEventArgs args)
        {
            _roundsPlayed++;
        }

        [Test]
        public void PlayMatch()
        {
            _roundsPlayed = 0;
            var players = new[] { new Player("Arne"), new Player("Tim"), new Player("Stijn"), new Player("Yorick") };
            var manager = new HeartsManager(players);

            manager.CardPlayed += manager_CardPlayed;
            manager.HandDone += manager_HandDone;
            manager.RoundDone += manager_RoundDone;

            do
            {
                PlayRound(manager);

            } while (!manager.Scores.Any(o => o.Value > 100));


            Assert.That(manager.Scores.Any(o => o.Value > 100));
        }

    }
}
