using PokerClient.Interfaces;
using PokerClient.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokerClient.Implementation
{
    public class GameRepository : IGameRepository
    {
        private readonly List<Game> _games = new List<Game>();

        public void AddGame(Game game)
        {
            _games.Add(game);
        }

        public Game GetGame(string gameId, string playerId = null)
        {
            return _games.FirstOrDefault(x => x.Id == gameId && (x.Players.Any(x => x.Id == playerId) || playerId == null));
        }

        public IEnumerable<Game> GetGames()
        {
            return _games;
        }
    }
}
