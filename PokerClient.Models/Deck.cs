using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClient.Models
{
    public class Deck
    {
        private readonly IEnumerable<Card> _cards;
        private int _cardsTaken = 0;

        public Deck()
        {
            var newCards = new List<Card>();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)).Cast<Suit>())
            {
                for (int i = 2; i <= 14; i++)
                {
                    newCards.Add(new Card(i, suit));
                }
            }

            _cards = newCards.OrderBy(a => Guid.NewGuid()).ToList();
        }

        public IEnumerable<Card> GetNext(int count)
        {
            IEnumerable<Card> cards = _cards.Skip(_cardsTaken).Take(count);
            _cardsTaken += count;
            return cards;
        }

        public Card GetNext()
        {
            Card card = _cards.Skip(_cardsTaken).First();
            _cardsTaken++;
            return card;
        }
    }
}
