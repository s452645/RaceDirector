using backend.Models.Seasons;

namespace backend.Models.Dtos.Seasons
{
    public class SeasonDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public SeasonDto()
        {
            Id = Guid.NewGuid();
        }

        public SeasonDto(Guid id, string? name, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public SeasonDto(Season season)
        {
            Id = season.Id;
            Name = season.Name;
            StartDate = season.StartDate;
            EndDate = season.EndDate;
        }

        public Season ToEntity()
        {
            var season = new Season();
            season.Id = Id;
            season.Name = Name ?? string.Empty;
            season.StartDate = StartDate;
            season.EndDate = EndDate;

            return season;
        }
    }
}
