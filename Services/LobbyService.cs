using CitiesRpcServer.Models;
using System.Collections.Generic;
using System.Text;

namespace CitiesRpcServer.Services
{
    public class LobbyService
    {
        private readonly List<string> cities;

        public List<GameSession> Sessions
            = new();

        private List<Player> waitingPlayers
            = new();

        private Dictionary<string, Guid>
            playerSessions = new();

        public LobbyService()
        {
            Console.OutputEncoding =
                Encoding.UTF8;

            cities = LoadCities();

            Console.WriteLine(
                $"Загружено городов: {cities.Count}");
        }

        public string Join(string playerName)
        {
            if (playerSessions.ContainsKey(playerName))
                return "Уже подключен";

            Player player =
                new Player
                {
                    Name = playerName
                };

            waitingPlayers.Add(player);

            Console.WriteLine(
                $"Игрок {playerName} вошел в лобби");

            if (waitingPlayers.Count >= 3)
            {
                GameSession session =
                    new GameSession();

                session.Players.AddRange(
                    waitingPlayers.Take(3));

                foreach (var p in session.Players)
                {
                    playerSessions[p.Name] =
                        session.Id;
                }

                waitingPlayers.RemoveRange(
                    0,
                    3);

                Sessions.Add(session);

                session.TurnStarted =
                    DateTime.Now;

                Console.WriteLine(
                    $"Создана сессия {session.Id}");

                return
                    $"Создана игра {session.Id}";
            }

            return "Ожидание игроков...";
        }

        public string GetPlayerStatus(
            string playerName)
        {
            if (!playerSessions
                .ContainsKey(playerName))
            {
                return "WAIT";
            }

            Guid sessionId =
                playerSessions[playerName];

            return $"START:{sessionId}";
        }

        public GameSession? GetSession(Guid id)
        {
            return Sessions
                .FirstOrDefault(
                    x => x.Id == id);
        }

        public string PlayMove(
            Guid sessionId,
            string playerName,
            string city)
        {
            var session =
                GetSession(sessionId);

            if (session == null)
                return "Сессия не найдена";

            if (session.Finished)
                return
                    $"Игра завершена. Победитель: {session.Winner}";

            if ((DateTime.Now - session.TurnStarted)
                .TotalSeconds > 30)
            {
                var timedOutPlayer =
                    session.Players[
                        session.CurrentPlayerIndex];

                timedOutPlayer.IsActive = false;

                int activePlayers =
                    session.Players.Count(
                        x => x.IsActive);

                if (activePlayers == 1)
                {
                    session.Finished = true;

                    session.Winner =
                        session.Players
                            .First(x => x.IsActive)
                            .Name;

                    return
                        $"Игрок {timedOutPlayer.Name} выбыл по таймауту. Победитель: {session.Winner}";
                }

                NextPlayer(session);

                session.TurnStarted =
                    DateTime.Now;

                return
                    $"Игрок {timedOutPlayer.Name} выбыл по таймауту";
            }

            var currentPlayer =
                session.Players[
                    session.CurrentPlayerIndex];

            if (!currentPlayer.IsActive)
            {
                NextPlayer(session);

                currentPlayer =
                    session.Players[
                        session.CurrentPlayerIndex];
            }

            if (currentPlayer.Name != playerName)
            {
                return
                    $"Сейчас ход игрока {currentPlayer.Name}";
            }

            city = city
                .Trim()
                .ToLower();

            if (string.IsNullOrWhiteSpace(city))
            {
                return "Город не введён";
            }

            if (!cities.Contains(city))
            {
                return "Такого города нет в базе";
            }

            if (session.UsedCities.Contains(city))
            {
                return "Город уже использовался";
            }

            if (!string.IsNullOrEmpty(
                session.LastCity))
            {
                char lastLetter =
                    GetLastValidLetter(
                        session.LastCity);

                if (city[0] != lastLetter)
                {
                    return
                        $"Город должен начинаться на '{lastLetter}'";
                }
            }

            session.LastCity = city;

            session.UsedCities.Add(city);

            NextPlayer(session);

            session.TurnStarted =
                DateTime.Now;

            return
                $"Город принят. Следующий игрок: " +
                $"{session.Players[session.CurrentPlayerIndex].Name}";
        }

        public string EliminatePlayer(
    Guid sessionId,
    string playerName)
        {
            var session =
                GetSession(sessionId);

            if (session == null)
                return "Сессия не найдена";

            var player =
                session.Players
                    .FirstOrDefault(
                        x => x.Name == playerName);

            if (player == null)
                return "Игрок не найден";

            player.IsActive = false;

            session.LastEvent =
                $"Игрок {playerName} выбыл";

            if (session.Players[
                session.CurrentPlayerIndex]
                .Name == playerName)
            {
                NextPlayer(session);
            }

            int activePlayers =
                session.Players
                    .Count(
                        x => x.IsActive);

            if (activePlayers == 1)
            {
                session.Finished = true;

                session.Winner =
                    session.Players
                        .First(
                            x => x.IsActive)
                        .Name;


                return
                    $"Победитель: {session.Winner}";
            }

            session.TurnStarted =
                DateTime.Now;

            return
                $"Игрок {playerName} выбыл";
        }

        public string CheckTimeout(
            Guid sessionId)
        {
            var session =
                GetSession(sessionId);

            if (session == null)
                return "";

            if (session.Finished)
                return "";

            if ((DateTime.Now -
                session.TurnStarted)
                .TotalSeconds <= 30)
            {
                return "";
            }

            var player =
                session.Players[
                    session.CurrentPlayerIndex];

            return EliminatePlayer(
                sessionId,
                player.Name);
        }

        private void NextPlayer(
            GameSession session)
        {
            do
            {
                session.CurrentPlayerIndex++;

                if (session.CurrentPlayerIndex >=
                    session.Players.Count)
                {
                    session.CurrentPlayerIndex = 0;
                }
            }
            while (!session.Players[
                session.CurrentPlayerIndex]
                .IsActive);
        }

        private char GetLastValidLetter(
            string city)
        {
            city = city.ToLower();

            int index =
                city.Length - 1;

            while (index >= 0)
            {
                char c = city[index];

                if (c != 'ь' &&
                    c != 'ъ' &&
                    c != 'ы')
                {
                    return c;
                }

                index--;
            }

            return city[0];
        }

        private List<string> LoadCities()
        {
            HashSet<string> result =
                new HashSet<string>();

            foreach (string line in
                File.ReadLines("city.csv")
                .Skip(1))
            {
                try
                {
                    int cityIndex =
                        line.LastIndexOf("г ");

                    if (cityIndex == -1)
                        continue;

                    string city =
                        line.Substring(
                            cityIndex + 2);

                    int commaIndex =
                        city.IndexOf(',');

                    if (commaIndex > 0)
                    {
                        city =
                            city.Substring(
                                0,
                                commaIndex);
                    }

                    city = city
                        .Replace("\"", "")
                        .Trim()
                        .ToLower();

                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        result.Add(city);
                    }
                }
                catch
                {
                }
            }

            return result.ToList();
        }
    }
}