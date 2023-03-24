namespace backend.Models.Dtos
{
    public class SeasonEventDto
    {

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Circuit? Circuit { get; set; }

        public SeasonEventDto()
        {
            Id = Guid.NewGuid();
        }

        public SeasonEventDto(Guid id, string? name, DateTime startDate, DateTime endDate, Circuit circuit)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Circuit = circuit;
        }

        public SeasonEventDto(SeasonEvent seasonEvent)
        {
            Id = seasonEvent.Id;
            Name = seasonEvent.Name;
            StartDate = seasonEvent.StartDate;
            EndDate = seasonEvent.EndDate;
            Circuit = seasonEvent.Circuit;
        }

        public SeasonEvent ToEntity()
        {
            var season = new SeasonEvent();
            season.Id = Id;
            season.Name = Name ?? string.Empty;
            season.StartDate = StartDate;
            season.EndDate = EndDate;
            season.Circuit = Circuit;

            return season;
        }
    }
}

