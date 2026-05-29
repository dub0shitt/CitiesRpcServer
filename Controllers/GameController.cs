using Microsoft.AspNetCore.Mvc;
using CitiesRpcServer.Services;

namespace CitiesRpcServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly LobbyService lobby;

        public GameController(
            LobbyService lobby)
        {
            this.lobby = lobby;
        }

        [HttpGet("{sessionId}")]
        public string GetInfo(
            Guid sessionId)
        {
            var session =
                lobby.GetSession(sessionId);

            if (session == null)
                return "Сессия не найдена";

            return
                $"Игроков: {session.Players.Count}, " +
                $"Последний город: {session.LastCity}";
        }

        [HttpGet(
            "{sessionId}/play/{player}/{city}")]
        public string Play(
            Guid sessionId,
            string player,
            string city)
        {
            return lobby.PlayMove(
                sessionId,
                player,
                city);
        }

        [HttpGet(
            "{sessionId}/eliminate/{player}")]
        public string Eliminate(
            Guid sessionId,
            string player)
        {
            return lobby.EliminatePlayer(
                sessionId,
                player);
        }

        [HttpGet("{sessionId}/last")]
        public string LastCity(
            Guid sessionId)
        {
            var session =
                lobby.GetSession(sessionId);

            if (session == null)
                return "";

            return session.LastCity;
        }

        [HttpGet("{sessionId}/current")]
        public string CurrentPlayer(
    Guid sessionId)
        {
            var session =
                lobby.GetSession(sessionId);

            if (session == null)
                return "";

            if (session.Finished)
            {
                return
                    $"WINNER:{session.Winner}";
            }

            while (!session.Players[
                session.CurrentPlayerIndex]
                .IsActive)
            {
                session.CurrentPlayerIndex++;

                if (session.CurrentPlayerIndex >=
                    session.Players.Count)
                {
                    session.CurrentPlayerIndex = 0;
                }
            }

            return session.Players[
                session.CurrentPlayerIndex]
                .Name;
        }

        [HttpGet("{sessionId}/active/{player}")]
        public string IsPlayerActive(
    Guid sessionId,
    string player)
        {
            var session =
                lobby.GetSession(sessionId);

            if (session == null)
                return "NOTFOUND";

            var p =
                session.Players
                    .FirstOrDefault(
                        x => x.Name == player);

            if (p == null)
                return "NOTFOUND";

            return p.IsActive
                ? "ACTIVE"
                : "ELIMINATED";
        }

        [HttpGet("{sessionId}/timeout")]
        public string Timeout(
            Guid sessionId)
        {
            return lobby.CheckTimeout(
                sessionId);
        }
    }
}