using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos;
using backenend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
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
                .Include(se => se.Circuit)
                .FirstOrDefaultAsync(se => se.Id == seasonEventId);

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Season Event [{seasonEventId}] not found.");
            }

            return seasonEvent;
        }
    }
}
