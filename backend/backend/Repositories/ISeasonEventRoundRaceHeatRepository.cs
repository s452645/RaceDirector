using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;

namespace backend.Repositories
{
    public interface ISeasonEventRoundRaceHeatRepository
    {
        public void UpdateHeat(SeasonEventRoundRaceHeatDto updatedHeat);
        public Task SaveAsync();
    }
}