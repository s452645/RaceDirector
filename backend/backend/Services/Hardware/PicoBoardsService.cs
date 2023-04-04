using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
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

        public async Task<List<BreakBeamSensorDto>> GetBoardBreakBeamSensors(Guid id)
        {
            var picoBoard = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .FirstOrDefaultAsync(pb => pb.Id == id);

            if (picoBoard == null)
            {
                throw new NotFoundException($"Board [{id}] not found");
            }

            return picoBoard.BreakBeamSensors.Select(sensor => new BreakBeamSensorDto(sensor)).ToList();
        }

        public async Task<PicoBoardDto> AddBreakBeamSensor(Guid picoBoardId, BreakBeamSensorDto breakBeamSensorDto)
        {
            var picoBoard = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .FirstOrDefaultAsync(pb => pb.Id == picoBoardId);

            if (picoBoard == null)
            {
                throw new NotFoundException($"Cannot add Break Beam Sensor to Board [{picoBoardId}]: Board not found");
            }

            var sensor = breakBeamSensorDto.ToEntity();
            sensor.Board = picoBoard;
            _context.BreakBeamSensors.Add(sensor);

            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }

        public async Task<PicoBoardDto> RemoveBreakBeamSensor(Guid picoBoardId, Guid breakBeamSensorId)
        {
            var picoBoard = await _context.PicoBoards
              .Include(pb => pb.BreakBeamSensors)
              .FirstOrDefaultAsync(pb => pb.Id == picoBoardId);

            if (picoBoard == null)
            {
                throw new NotFoundException($"Cannot delete Break Beam Sensor from Board [{picoBoardId}]: Board not found");
            }

            var sensor = picoBoard.BreakBeamSensors.FirstOrDefault(bbs => bbs.Id == breakBeamSensorId);

            if (sensor == null)
            {
                throw new NotFoundException($"Cannot delete Break Beam Sensor from Board [{picoBoard.Name}]: Sensor [{breakBeamSensorId}] is not assigned to the Board");
            }

            picoBoard.BreakBeamSensors.Remove(sensor);
            _context.BreakBeamSensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }
    }
}
