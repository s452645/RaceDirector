using backend.Models;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;

namespace backend.Repositories
{
    public class SeasonEventRoundRaceHeatRepository : ISeasonEventRoundRaceHeatRepository
    {
        private readonly BackendContext _backendContext;

        public SeasonEventRoundRaceHeatRepository(BackendContext backendContext) 
        {
            _backendContext = backendContext;
        }

        public void UpdateHeat(SeasonEventRoundRaceHeatDto updatedHeat)
        {
            var entity = updatedHeat.ToEntity();
            _backendContext.SeasonEventRoundRaceHeats.Update(entity);
        }

        public async Task SaveAsync()
        {
            await _backendContext.SaveChangesAsync();
        }
    }
}
