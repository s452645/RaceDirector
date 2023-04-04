namespace backend.Models
{
    public class SeasonEventRoundRace
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int ParticipantsCount { get; set; }

        public List<SeasonEventRoundRaceResult> Results { get; set; }
        public List<SeasonEventRoundRaceHeat> Heats { get; set; }

        public int InstantAdvancements { get; set; }
        public int SecondChances { get; set; }

        public SeasonEventRound Round { get; set; }
    }
}
