using System.Runtime.Serialization;

namespace PokerClient.Web.Commands
{
    public class JoinGameCommand
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public int? BuyIn { get; set; }
    }
}
