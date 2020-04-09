using AutoMapper;
using PokerClient.Models;
using PokerClient.Web.Dtos;
using System;

namespace PokerClient.Web.Profiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameListItemDto>()
                .ForMember(x => x.CurrentPlayers, y => y.MapFrom(x => x.Players.Count));

            CreateMap<Game, GameDto>()
                .ForMember(x => x.Flop, y => y.MapFrom(x => ShouldShow(BettingRound.Flop, x.BettingRound) ? x.Flop : null))
                .ForMember(x => x.Turn, y => y.MapFrom(x => ShouldShow(BettingRound.Turn, x.BettingRound) ? x.Turn : null))
                .ForMember(x => x.River, y => y.MapFrom(x => ShouldShow(BettingRound.River, x.BettingRound) ? x.River : null));

            CreateMap<Player, PlayerDto>();

            CreateMap<Card, CardDto>()
                .ForMember(x => x.Value, y => y.MapFrom(x => GetCardValue(x.Value, x.Suit)));            
        }

        private bool ShouldShow(BettingRound roundToShow, BettingRound currentRound)
        {
            return (int)currentRound >= (int)roundToShow;
        }

        private string GetCardValue(int value, Suit suit)
        {
            string valuePart = value switch
            {
                14 => "A",
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => value.ToString(),
            };

            string suitPart = suit switch
            {
                Suit.Hearts => "H",
                Suit.Diamonds => "D",
                Suit.Clubs => "C",
                Suit.Spades => "S",
                _ => throw new Exception("Invalid suit"),
            };

            return valuePart + suitPart;
        }
    }
}
