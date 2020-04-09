namespace PokerClient.Web.Commands
{
    public class CreateGameCommand
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int? MaxPlayers { get; set; }
        public string AdminName { get; set; }
        public int? AdminBuyIn { get; set; }
        public int? SmallBlind { get; set; }
        public int? BigBlind { get; set; }
    }
}
