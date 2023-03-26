namespace backend.Models
{
    public class Season
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SeasonEvent> Events { get; set; }
        public List<SeasonCarStanding> Standings { get; set; }
        public List<SeasonTeamStanding> TeamStandings { get; set; }   
    }
}
