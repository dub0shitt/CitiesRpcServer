namespace CitiesRpcServer.Models
{
    public class GameState
    {
        public string LastCity { get; set; } = "";

        public HashSet<string> UsedCities
            = new HashSet<string>();
    }
}