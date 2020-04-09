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

namespace PokerClient.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        // move to in memory cache
        private static readonly List<Game> _games = new List<Game>();

        private readonly IMapper _mapper;
        private readonly IHubContext<GameHub> _gameHub;

        private string GameId => User.Claims.FirstOrDefault(c => c.Type == "game-id")?.Value;
        private string PlayerId => User.Claims.FirstOrDefault(c => c.Type == "player-id")?.Value;

        public GamesController(
            IMapper mapper,
            IHubContext<GameHub> gameHub)
        {
            _mapper = mapper;
            _gameHub = gameHub;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Game>> Get()
        {
            var game = _mapper.Map<List<GameListItemDto>>(_games);
            return Ok(game);
        }

        [HttpGet("mine")]
        public ActionResult<List<Game>> GetForPlayer()
        {
            var game = _mapper.Map<List<GameListItemDto>>(_games);
            return Ok(game);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<GameTokenDto> CreateGame([FromBody] CreateGameCommand command)
        {
            if (_games.Any(x => x.Name == command.Name))
            {
                return BadRequest("There is already a game with this name");
            }

            Game game = new Game(
                command.Name,
                command.Password,
                command.MaxPlayers.Value,
                command.AdminName,
                command.AdminBuyIn.Value,
                command.SmallBlind.Value,
                command.BigBlind.Value);

            _games.Add(game);

            return Ok(GenerateToken(game.Id, game.Players.First().Id));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{id}/players")]
        public async Task<ActionResult<GameTokenDto>> JoinGame([FromRoute] string id, [FromBody] JoinGameCommand command)
        {
            Game game = _games.FirstOrDefault(x => x.Id == id);

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
        public async Task<ActionResult> StartGame() //[FromBody] StartGameCommand command)
        {
            Game game = GetGame();

            if (game == null)
            {
                return NotFound();
            }

            game.Start();

            await EmitGameStateAsync(game);

            return Ok(game);
        }

        [HttpPost]
        [Route("play")]
        public ActionResult MakeMove([FromBody] MakeMoveCommand command)
        {
            Game game = GetGame();

            if (game == null)
            {
                return NotFound();
            }

            //game.MakeMove(command.PlayerId, command.Amount);

            return Ok(game);
        }
        private Game GetGame()
        {
            return _games.FirstOrDefault(x => x.Id == GameId);         
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