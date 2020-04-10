using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PokerClient.Models;
using PokerClient.Web.Commands;
using PokerClient.Web.Dtos;
using PokerClient.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using PokerClient.Interfaces;

namespace PokerClient.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        // move to in memory cache
        private readonly IGameRepository _gameRepository;

        private readonly IMapper _mapper;
        private readonly IHubContext<GameHub> _gameHub;
        private readonly IHubContext<LobbyHub> _lobbyHub;

        private string GameId => User.Claims.FirstOrDefault(c => c.Type == "game-id")?.Value;
        private string PlayerId => User.Claims.FirstOrDefault(c => c.Type == "player-id")?.Value;

        public GamesController(
            IMapper mapper,
            IHubContext<GameHub> gameHub,
            IHubContext<LobbyHub> lobbyHub,
            IGameRepository gameRepository)
        {
            _mapper = mapper;
            _gameHub = gameHub;
            _lobbyHub = lobbyHub;
            _gameRepository = gameRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Game>> Get()
        {
            var game = _mapper.Map<List<GameListItemDto>>(_gameRepository.GetGames());
            return Ok(game);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<GameTokenDto>> CreateGame([FromBody] CreateGameCommand command)
        {
            Game game = new Game(
                command.Name,
                command.Password,
                command.MaxPlayers.Value,
                command.AdminName,
                command.AdminBuyIn.Value,
                command.SmallBlind.Value,
                command.BigBlind.Value);

            _gameRepository.AddGame(game);

            await _lobbyHub.Clients.All.SendAsync("gamecreated", _mapper.Map<GameListItemDto>(game));

            return Ok(GenerateToken(game.Id, game.Players.First().Id));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{id}/players")]
        public async Task<ActionResult<GameTokenDto>> JoinGame([FromRoute] string id, [FromBody] JoinGameCommand command)
        {
            Game game = _gameRepository.GetGame(id);

            if (game == null)
            {
                return NotFound();
            }

            if (game.Password != command.Password)
            {
                return Forbid("Incorrect password");
            }

            string playerId = game.Join(
                command.Name,
                command.BuyIn.Value);

            await EmitGameStateAsync(game, playerId);

            return Ok(GenerateToken(game.Id, playerId));
        }

        [HttpPost]
        [Route("start")]
        public async Task<ActionResult> StartGame()
        {
            if (PlayerId == null)
            {
                return Forbid();
            }

            Game game = _gameRepository.GetGame(GameId, PlayerId);

            if (game == null)
            {
                return NotFound();
            }

            game.Start();

            await EmitGameStateAsync(game);

            return Ok();
        }

        [HttpPost]
        [Route("play")]
        public async Task<ActionResult> MakeMove([FromBody] MakeMoveCommand command)
        {
            if (PlayerId == null)
            {
                return Forbid();
            }

            Game game = _gameRepository.GetGame(GameId, PlayerId);

            if (game == null)
            {
                return NotFound();
            }

            game.MakeMove(PlayerId, command.Amount);

            await EmitGameStateAsync(game);

            return Ok();
        }

        private async Task EmitGameStateAsync(Game game, string newPlayerId = null)
        {
            foreach (var player in game.Players.Where(x => x.Id != newPlayerId))
            {
                var gameDto = _mapper.Map<GameDto>(game);
                gameDto.RemoveOppopentsCards(player.Id);
                await _gameHub.Clients.Groups(player.Id).SendAsync("gamestatechanged", gameDto);
            }
        }

        private GameTokenDto GenerateToken(string gameId, string playerId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("poker-secret-poker-secret-poker-secret-poker-secret-poker-secret");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("game-id", gameId),
                    new Claim("player-id", playerId),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new GameTokenDto(gameId, playerId, tokenHandler.WriteToken(token));
        }
    }
}