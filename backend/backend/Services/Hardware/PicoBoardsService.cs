using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Hardware;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Hardware
{
    public class PicoBoardsService
    {
        private readonly BackendContext _context;

        public PicoBoardsService(BackendContext context)
        {
            _context = context;
        }

        public async Task<List<PicoBoardDto>> GetPicoBoards()
        {
            return await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .Select(pb => new PicoBoardDto(pb))
                .ToListAsync();
        }

        public async Task<PicoBoardDto> AddPicoBoard(PicoBoardDto picoBoardDto)
        {
            var picoBoard = picoBoardDto.ToEntity();
            _context.PicoBoards.Add(picoBoard);
            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }

        public async Task DeletePicoBoard(Guid picoBoardId)
        {
            var picoBoard = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .FirstOrDefaultAsync(pb => pb.Id == picoBoardId);

            if (picoBoard == null)
            {
                throw new NotFoundException($"Cannot delete Pico Board [{picoBoardId}]: Pico Board not found");
            }

            var breakBeamSensors = picoBoard.BreakBeamSensors;
            picoBoard.BreakBeamSensors.Clear();

            _context.BreakBeamSensors.RemoveRange(breakBeamSensors);
            _context.PicoBoards.Remove(picoBoard);

            await _context.SaveChangesAsync();
        }
    }
}
