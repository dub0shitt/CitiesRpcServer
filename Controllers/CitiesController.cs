using Microsoft.AspNetCore.Mvc;
using CitiesRpcServer.Services;

namespace CitiesRpcServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly GameService game;

        public CitiesController(
            GameService game)
        {
            this.game = game;
        }

        [HttpGet("start")]
        public string Start()
        {
            return game.StartGame();
        }

        [HttpGet("play/{city}")]
        public string Play(string city)
        {
            return game.PlayCity(city);
        }

        [HttpGet("last")]
        public string Last()
        {
            return game.GetLastCity();
        }
    }
}