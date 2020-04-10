using PokerClient.Common;
using System;
using System.Collections.Generic;

namespace PokerClient.Models
{
    public class Player
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int SeatNumber { get; set; }

        public int Balance { get; set; }

        public int AmountBet { get; set; }

        public int TotalAmountBet { get; set; }

        public string HandValue { get; set; }

        public PlayerStatus Status { get; set; }

        public PlayerRoundStatus RoundStatus { get; set; }

        public IEnumerable<Card> Cards { get; set; }

        public Player(string name, int buyIn, int seatNumber)
        {
            Name = name;
            Balance = buyIn;
            SeatNumber = seatNumber;
            Id = Guid.NewGuid().ToString();
        }

        public void Deal(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public void Blind(int amount, bool isSmall)
        {
            if (amount >= Balance)
            {
                AmountBet = Balance;
                Balance = 0;
                RoundStatus = PlayerRoundStatus.AllIn;
            }
            else
            {
                AmountBet += amount;
                Balance -= amount;
                RoundStatus = isSmall ? PlayerRoundStatus.HasBet : PlayerRoundStatus.BigBlind;
            }
        }

        public void Bet(int amount)
        {
            ValidationException.ThrowIfTrue(amount > Balance, "insufficient funds");
            Balance -= amount;
            AmountBet += amount;
            RoundStatus = Balance == 0 ? PlayerRoundStatus.AllIn : PlayerRoundStatus.HasBet;
        }

        public void Fold()
        {
            this.RoundStatus = PlayerRoundStatus.Folded;
        }

        public void Check()
        {
            this.RoundStatus = PlayerRoundStatus.Checked;
        }

        public void EndRound()
        {
            TotalAmountBet += AmountBet;
            AmountBet = 0;
            if (RoundStatus != PlayerRoundStatus.AllIn && RoundStatus != PlayerRoundStatus.Folded)
            {
                RoundStatus = PlayerRoundStatus.YetToBet;
            }
        }

        public void AwardWinnings(int amount)
        {
            Balance += amount;
        }

        public void Call(int amount)
        {
            ValidationException.ThrowIfTrue(amount > Balance, "insufficient funds");
            AmountBet += amount;
            Balance -= amount;
            RoundStatus = Balance == 0 ? PlayerRoundStatus.AllIn : PlayerRoundStatus.Called; 
        }
    }
}
