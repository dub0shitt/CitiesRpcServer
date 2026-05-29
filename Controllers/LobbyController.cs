using Microsoft.AspNetCore.Mvc;
using CitiesRpcServer.Services;

namespace CitiesRpcServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbyController : ControllerBase
    {
        private readonly LobbyService lobby;

        public LobbyController(
            LobbyService lobby)
        {
            this.lobby = lobby;
        }

        [HttpGet("join/{name}")]
        public string Join(
            string name)
        {
            return lobby.Join(name);
        }

        [HttpGet("status/{name}")]
        public string Status(
            string name)
        {
            return lobby.GetPlayerStatus(name);
        }
    }
}