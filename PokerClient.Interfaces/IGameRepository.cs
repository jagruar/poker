using PokerClient.Models;
using System.Collections.Generic;

namespace PokerClient.Interfaces
{
    public interface IGameRepository
    {
        IEnumerable<Game> GetGames();
        Game GetGame(string gameId, string playerId = null);
        void AddGame(Game game);
    }
}
