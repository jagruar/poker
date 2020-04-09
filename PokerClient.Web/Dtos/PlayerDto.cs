using PokerClient.Models;
using System.Collections.Generic;

namespace PokerClient.Web.Dtos
{
    public class PlayerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int SeatNumber { get; set; }
        public int Balance { get; set; }
        public int AmountBet { get; set; }
        public int TotalAmountBet { get; set; }
        public PlayerStatus Status { get; set; }
        public PlayerRoundStatus RoundStatus { get; set; }
        public IEnumerable<CardDto> Cards { get; set; }
    }
}
