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
            return player.Id;
        }

        public void Start()
        {
            Players = Players.OrderBy(x => x.SeatNumber).ToList();
            DealerSeatNumber = new Random().Next(1, Players.Count);
            StartRound();

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
            Player nextPlayer = Players.First(x => x.SeatNumber == NextSeatNumber);

            // All players folded - award money
            if (Players.Count(x => x.RoundStatus != PlayerRoundStatus.Folded) == 1)
            {
                // end round, next player won
            }
            // All players have matched the bet, folded or are all in and no players are big blind.
            else if (Players.All(x => 
                (x.AmountBet == highestBet
                || x.RoundStatus == PlayerRoundStatus.Folded
                || x.RoundStatus == PlayerRoundStatus.AllIn)
                && x.RoundStatus != PlayerRoundStatus.BigBlind))
            {
                // move to next betting round
            }
            else
            {
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

        private void ProcessBet(Player player, int? amount)
        {
            int highestBet = Players.Max(x => x.AmountBet);

            if (amount == null)
            {
                player.Fold();
                return;
            }

            int betAmount = amount.Value;

            ValidationException.ThrowIfTrue(amount == 0 && player.AmountBet != highestBet, "Can not check, call or bet");

            if (betAmount == 0)
            {
                player.Check();
                return;
            }

            // less than call, not going all in
            ValidationException.ThrowIfTrue(
                betAmount + player.AmountBet < highestBet && player.Balance < betAmount,
                "can not bet less than current bet");

            if (betAmount + player.AmountBet <= highestBet)
            {
                player.Call(betAmount);
                return;
            }

            // less than minimum bet, not going all in
            ValidationException.ThrowIfTrue(
                betAmount + player.AmountBet < highestBet + MinimumBetIncrement && player.Balance < betAmount,
                "can not bet less than current bet");

            player.Bet(betAmount);

            // Bet has been made check increment increase
            int playersIncrement = player.AmountBet - highestBet;
            MinimumBetIncrement = Math.Max(playersIncrement, MinimumBetIncrement); 
        }

        public void StartRound()
        {
            BettingRound = BettingRound.Opening;
            ProcessBlinds();
            DealCards();
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
