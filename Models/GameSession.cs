namespace CitiesRpcServer.Models
{
    public class GameSession
    {
        public Guid Id { get; set; }
            = Guid.NewGuid();

        public List<Player> Players
            = new();

        public string LastCity
            = "";
           
        public string LastEvent
            = "";

        public HashSet<string> UsedCities
            = new();

        public int CurrentPlayerIndex
            = 0;

        public bool Finished
            = false;

        public string Winner
            = "";

        public DateTime TurnStarted
            = DateTime.Now;
    }
}