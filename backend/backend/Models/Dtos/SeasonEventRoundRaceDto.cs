namespace backend.Models.Dtos
{
    public class SeasonEventRoundRaceDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int ParticipantsCount { get; set; }

        /*public List<SeasonEventRoundRaceResult> Results { get; set; }
        public List<SeasonEventRoundRaceHeat> Heats { get; set; }
*/
        public int InstantAdvancements { get; set; }
        public int SecondChances { get; set; }
        public Guid RoundId { get; set; }

        public SeasonEventRoundRaceDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            ParticipantsCount = 0;
            InstantAdvancements = 0;
            SecondChances = 0;
        }

        public SeasonEventRoundRaceDto(Guid id, int order, int participantsCount, int instantAdvancements, int secondChances, Guid roundId)
        {
            Id = id;
            Order = order;
            ParticipantsCount = participantsCount;
            InstantAdvancements = instantAdvancements;
            SecondChances = secondChances;
            RoundId = roundId;
        }

        public SeasonEventRoundRaceDto(SeasonEventRoundRace race)
        {
            Id = race.Id;
            Order = race.Order;
            ParticipantsCount = race.ParticipantsCount;
            InstantAdvancements = race.InstantAdvancements;
            SecondChances = race.SecondChances;
            RoundId = race.Round.Id;
        }

        public SeasonEventRoundRace ToEntity()
        {
            var race = new SeasonEventRoundRace();
            race.Id = Id;
            race.Order = Order;
            race.ParticipantsCount = ParticipantsCount;
            race.InstantAdvancements = InstantAdvancements;
            race.SecondChances = SecondChances;

            return race;
        }
    }
}
