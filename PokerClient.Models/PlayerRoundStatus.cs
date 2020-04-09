namespace PokerClient.Models
{
    public enum PlayerRoundStatus
    {
        YetToBet = 1,
        HasBet = 2,
        Called = 3,
        Checked = 4,
        Folded = 5,
        AllIn = 6,
        Deciding = 7,
        BigBlind = 8,
    }
}
