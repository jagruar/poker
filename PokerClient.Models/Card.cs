namespace PokerClient.Models
{
    public class Card
    {
        public int Value { get; set; }
        public Suit Suit { get; set; }

        public Card(int value, Suit suit)
        {
            Value = value;
            Suit = suit;
        }        
    }
}
