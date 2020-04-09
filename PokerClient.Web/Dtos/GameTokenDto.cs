namespace PokerClient.Web.Dtos
{
    public class GameTokenDto
    {
        public string GameId { get; set; }
        public string PlayerId { get; set; }
        public string Token { get; set; }

        public GameTokenDto(string gameId, string playerId, string token)
        {
            GameId = gameId;
            PlayerId = playerId;
            Token = token;
        }
    }
}
