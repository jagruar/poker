using PokerClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerClient.Models
{
    public class Game
    {
        // Game metadata
        public string Password { get; private set; }
        public string Id { get; private set; }
        public DateTime LastActivity { get; private set; }
        public int MaxPlayers { get; private set; }
        public string Name { get; private set; }

        // Turn Processing
        public int SmallBlind { get; private set; }
        public int BigBlind { get; private set; }
        public int MinimumMoveAmount { get; private set; }
        public int MinimumBetIncrement { get; private set; }
        public int DealerSeatNumber { get; set; }
        public int SmallBlindSeatNumber { get; set; }
        public int BigBlindSeatNumber { get; set; }
        public int NextSeatNumber { get; set; }
        public BettingRound BettingRound { get; set; }
        public int Pot { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public IEnumerable<Card> Flop { get; set; }
        public Card River { get; set; }
        public Card Turn { get; set; }

        public Game(string name, string password, int maxPlayers, string adminName, int adminBuyIn, int smallBlind, int bigBlind)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Password = password;
            MaxPlayers = maxPlayers;
            Players.Add(new Player(adminName, adminBuyIn, 0));
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
        }

        public string Join(string name, int buyIn)
        {
            ValidationException.ThrowIfTrue(Players.Any(x => x.Name == name), "Name taken");
            ValidationException.ThrowIfTrue(Players.Count >= MaxPlayers, "Game already full");

            int[] availableSeats = Enumerable.Range(0, MaxPlayers)
                .Where(x => !Players.Any(p => p.SeatNumber == x))
                .ToArray();
            int seatIndex = new Random().Next(availableSeats.Count());

            var player = new Player(name, buyIn, availableSeats[seatIndex]);
            Players.Add(player);
            Players = Players.OrderBy(x => x.SeatNumber).ToList();

            return player.Id;
        }

        public void Start()
        {

            DealerSeatNumber = Players.OrderBy(x => Guid.NewGuid()).First().SeatNumber;
            StartRound();
        }

        public void StartRound()
        {
            Players.ForEach(x => x.TotalAmountBet = 0);
            Players.ForEach(x => x.RoundStatus = PlayerRoundStatus.YetToBet);
            BettingRound = BettingRound.Opening;
            ProcessBlinds();
            MinimumBetIncrement = BigBlind;
            DealCards();
            NextSeatNumber = NextPlayer(BigBlindSeatNumber).SeatNumber;
            CalculateMinMoveAmount();
        }

        private void ProcessBlinds()
        {
            DealerSeatNumber = NextPlayer(DealerSeatNumber).SeatNumber;

            Player smallBlindPlayer = NextPlayer(DealerSeatNumber);
            SmallBlindSeatNumber = smallBlindPlayer.SeatNumber;
            smallBlindPlayer.Blind(SmallBlind, true);

            Player bigBlindPlayer = NextPlayer(SmallBlindSeatNumber);
            BigBlindSeatNumber = bigBlindPlayer.SeatNumber;
            bigBlindPlayer.Blind(BigBlind, false);
        }

        public void MakeMove(string playerId, int? amount)
        {
            Player player = Players.FirstOrDefault(x => x.Id == playerId);
            ValidationException.ThrowIfTrue(player == null, "Invalid player ID");
            ValidationException.ThrowIfTrue(player.SeatNumber != NextSeatNumber, "Not your move");

            ProcessBet(player, amount);

            int highestBet = Players.Max(x => x.AmountBet);

            // All players folded - award money
            if (Players.Count(x => x.RoundStatus != PlayerRoundStatus.Folded) == 1)
            {
                Players.ForEach(x => x.EndRound());
                Pot = Players.Sum(x => x.TotalAmountBet);
                var winner = Players.First(x => x.RoundStatus != PlayerRoundStatus.Folded);
                winner.AwardWinnings(Pot);
                StartRound();
            }
            // All players have matched the bet, folded or are all in and no players are big blind.
            else if (Players.All(x => 
                (x.RoundStatus == PlayerRoundStatus.Checked) ||
                ((x.AmountBet == highestBet
                || x.RoundStatus == PlayerRoundStatus.Folded
                || x.RoundStatus == PlayerRoundStatus.AllIn) && x.RoundStatus != PlayerRoundStatus.BigBlind && highestBet != 0)))
            {
                Players.ForEach(x => x.EndRound());
                Pot = Players.Sum(x => x.TotalAmountBet);
                NextSeatNumber = NextPlayer(DealerSeatNumber).SeatNumber;
                BettingRound = (BettingRound)((int)BettingRound + 1);
                MinimumMoveAmount = 0;
            }
            else
            {
                Player nextPlayer = NextActivePlayer(player.SeatNumber);
                NextSeatNumber = nextPlayer.SeatNumber;
                CalculateMinMoveAmount();                
                // stay in current betting round
            }
        }

        private Player NextPlayer(int currentSeatNumber)
        {
            int nextSeatNumber = (currentSeatNumber + 1) % MaxPlayers;
            Player nextPlayer = Players.FirstOrDefault(x => x.SeatNumber == nextSeatNumber);

            if (nextPlayer != null)
            {
                return nextPlayer;
            }
            else
            {
                return NextPlayer(nextSeatNumber);
            }
        }

        private Player NextActivePlayer(int currentSeatNumber)
        {
            int nextSeatNumber = (currentSeatNumber + 1) % MaxPlayers;
            Player nextPlayer = Players.FirstOrDefault(x => x.SeatNumber == nextSeatNumber);

            if (nextPlayer != null && (nextPlayer.RoundStatus != PlayerRoundStatus.AllIn || nextPlayer.RoundStatus != PlayerRoundStatus.Folded))
            {
                return nextPlayer;
            }
            else
            {
                return NextPlayer(nextSeatNumber);
            }
        }

        private void ProcessBet(Player player, int? amount)
        {
            if (amount == null)
            {
                player.Fold();
                return;
            }

            int highestBet = Players.Max(x => x.AmountBet);
            int betAmount = amount.Value;

            ValidationException.ThrowIfTrue(amount < MinimumMoveAmount, "Les than minimum move amount");

            if (betAmount == 0)
            {
                player.Check();
                return;
            }

            if (betAmount == MinimumMoveAmount)
            {
                player.Call(betAmount);
                return;
            }

            // less than minimum bet, not going all in
            ValidationException.ThrowIfTrue(
                betAmount + player.AmountBet < highestBet + MinimumBetIncrement && player.Balance > betAmount,
                "can not bet less than current bet");

            player.Bet(betAmount);

            // Bet has been made check increment increase
            int playersIncrement = player.AmountBet - highestBet;
            MinimumBetIncrement = Math.Max(playersIncrement, MinimumBetIncrement); 
        }

        private void CalculateMinMoveAmount()
        {
            Player player = Players.FirstOrDefault(x => x.SeatNumber == NextSeatNumber);
            int highestBet = Players.Max(x => x.AmountBet);

            if (player.AmountBet < highestBet)
            {
                MinimumMoveAmount = Math.Min(player.Balance, highestBet - player.AmountBet);
            }
            else
            {
                MinimumMoveAmount = 0;
            }
        }

        private void DealCards()
        {
            var deck = new Deck();
            Flop = deck.GetNext(3);
            River = deck.GetNext();
            Turn = deck.GetNext();
            Players.ForEach(x => x.Deal(deck.GetNext(2)));
        }

        private void EvaluateHands()
        {
            IEnumerable<Card> cards = Flop.Append(Turn).Append(River);

            foreach (var player in Players)
            {
                var combinedCards = player.Cards.Concat(cards);
                IEnumerable<Hand> hands = combinedCards.DifferentCombinations(5).Select(x => new Hand(x));
                player.HandValue = hands.Max(x => x.Value);
            }
        }
    }
}
