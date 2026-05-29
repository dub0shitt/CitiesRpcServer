using CitiesRpcServer.Models;
using System.Text;

namespace CitiesRpcServer.Services
{
    public class GameService
    {
        private readonly GameState gameState = new();

        private readonly List<string> cities;

        public GameService()
        {
            Console.OutputEncoding = Encoding.UTF8;

            cities = LoadCities();

            Console.WriteLine(
                $"Загружено городов: {cities.Count}");
        }

        public string StartGame()
        {
            gameState.LastCity = "";
            gameState.UsedCities.Clear();

            return "Игра началась";
        }

        public string PlayCity(string city)
        {
            city = city
                .Trim()
                .ToLower();

            if (!cities.Contains(city))
            {
                return "Такого города нет в базе";
            }

            if (gameState.UsedCities.Contains(city))
            {
                return "Город уже использовался";
            }

            if (!string.IsNullOrEmpty(gameState.LastCity))
            {
                char lastLetter =
                    GetLastValidLetter(
                        gameState.LastCity);

                if (city[0] != lastLetter)
                {
                    return $"Город должен начинаться на '{lastLetter}'";
                }
            }

            gameState.UsedCities.Add(city);
            gameState.LastCity = city;

            return "Город принят";
        }

        public string GetLastCity()
        {
            return gameState.LastCity;
        }

        private char GetLastValidLetter(
            string city)
        {
            city = city.ToLower();

            int index = city.Length - 1;

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
                        line.Substring(cityIndex + 2);

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