using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Seasons.Events;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Seasons.Events.Circuits
{
    public class CircuitService
    {
        private readonly BackendContext _context;

        public CircuitService(BackendContext context)
        {
            _context = context;
        }

        public async Task<CircuitDto> GetCircuitById(Guid seasonEventId, Guid circuitId)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);
            var circuit = seasonEvent.CircuitId == circuitId ? seasonEvent.Circuit : null;

            if (circuit == null)
            {
                throw new NotFoundException($"Circuit [{circuitId}] not found in Season Event [{seasonEventId}]");
            }

            return new CircuitDto(circuit);
        }

        public async Task<CircuitDto> AddCircuit(Guid seasonEventId, CircuitDto circuitDto)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);

            if (seasonEvent.Circuit != null)
            {
                throw new BadRequestException($"Cannot add a new Circuit to Season Event [{seasonEvent.Name}]: Circuit [{seasonEvent.Circuit.Name}] is already assigned");
            }

            var circuit = circuitDto.ToEntity();
            circuit.SeasonEvent = seasonEvent;

            _context.Circuits.Add(circuit);
            seasonEvent.Circuit = circuit;

            await _context.SaveChangesAsync();
            return new CircuitDto(circuit);
        }

        public async Task<CircuitDto> UpdateCircuit(Guid seasonEventId, CircuitDto updatedCircuit)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);

            if (updatedCircuit.Id != seasonEvent.Circuit?.Id)
            {
                throw new NotFoundException($"Cannot update Circuit [{updatedCircuit.Name}] in SeasonEvent [{seasonEvent.Name}]: Circuit not found in Season Event");
            }

            var existingCircuit = seasonEvent.Circuit;
            updatedCircuit.ToEntity(existingCircuit);

            await _context.Checkpoints.AddRangeAsync(existingCircuit.Checkpoints);
            await _context.SaveChangesAsync();

            return new CircuitDto(existingCircuit);
        }

        public async Task DeleteCircuit(Guid seasonEventId, Guid circuitId)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);

            var circuit = seasonEvent.CircuitId == circuitId ? seasonEvent.Circuit : null;

            if (circuit == null)
            {
                throw new NotFoundException($"Circuit [{circuitId}] not found in Season Event [{seasonEventId}]");
            }

            seasonEvent.Circuit = null;
            _context.Circuits.Remove(circuit);

            await _context.SaveChangesAsync();
        }

        private async Task<SeasonEvent> GetSeasonEventOrThrow(Guid seasonEventId)
        {
            var seasonEvent = await _context.SeasonEvents
                .Include(se => se.Circuit).ThenInclude(c => c.Checkpoints)
                .FirstOrDefaultAsync(se => se.Id == seasonEventId);

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Season Event [{seasonEventId}] not found.");
            }

            return seasonEvent;
        }
    }
}
