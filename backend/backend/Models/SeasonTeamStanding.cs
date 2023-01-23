namespace backend.Models
{
    public class SeasonTeamStanding
    {
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }

        public double Points { get; set; }
        public int Place { get; set; }

        // TODO: consider each participant points?
    }
}
