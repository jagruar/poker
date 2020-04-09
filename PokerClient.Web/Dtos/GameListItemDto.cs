namespace PokerClient.Web.Dtos
{
    public class GameListItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
    }
}
