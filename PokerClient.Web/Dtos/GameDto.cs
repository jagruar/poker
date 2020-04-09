using PokerClient.Models;
using System.Collections.Generic;

namespace PokerClient.Web.Dtos
{
    public class GameDto
    {
        public string Name { get; private set; }
        public int MaxPlayers { get; set; }
        public int MyProperty { get; set; }
        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
        public int MinimumBetIncrement { get; set; }
        public int DealerSeatNumber { get; set; }
        public int SmallBlindSeatNumber { get; set; }
        public int BigBlindSeatNumber { get; set; }
        public int NextSeatNumber { get; set; }
        public BettingRound BettingRound { get; set; }
        public int Pot { get; set; }
        public List<PlayerDto> Players { get; set; }
        public IEnumerable<CardDto> Flop { get; set; }
        public CardDto River { get; set; }
        public CardDto Turn { get; set; }

        public void RemoveOppopentsCards(string playerId)
        {
            Players.ForEach(x => 
            {
                x.Cards = x.Id == playerId ? x.Cards : null;
            });
        }
    }
}
