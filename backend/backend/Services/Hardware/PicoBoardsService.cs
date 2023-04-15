using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using backend.Services.Hardware.Comms;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace backend.Services.Hardware
{
    public class PicoBoardsService
    {
        private readonly BackendContext _context;
        private readonly BoardsManager _boardsManager;

        public PicoBoardsService(BackendContext context, BoardsManager boardsManager, HardwareCommunicationService commsService)
        {
            _context = context;
            _boardsManager = boardsManager;
        }

        public async Task<List<PicoBoardDto>> GetPicoBoards()
        {
            var boards = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .Include(pb => pb.SyncBoardResults)
                .ToListAsync();

            return boards.Select(board =>
            {
                var lastSync = board.SyncBoardResults.MaxBy(board => board.SyncFinishedTimestamp);
                var boardDto = new PicoBoardDto(board);

                if (lastSync != null)
                {
                    boardDto.LastSyncOffset = lastSync.CurrentSyncOffset;
                    boardDto.LastSyncResult = lastSync.SyncResult;
                    boardDto.LastSyncDateTime = lastSync.SyncFinishedTimestamp;
                }

                return boardDto;
            }).ToList();
        }

        public List<PicoBoardDto> GetActiveWiFiPicoBoards()
        {
            return _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .Where(pb => pb.Active && pb.Type == PicoBoardType.WiFi)
                .Select(pb => new PicoBoardDto(pb))
                .ToList();
        }

        public async Task<PicoBoardDto> AddPicoBoard(PicoBoardDto picoBoardDto)
        {
            var picoBoard = picoBoardDto.ToEntity();
            _context.PicoBoards.Add(picoBoard);
            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }

        public async Task<PicoBoardDto> ConnectPicoBoard(Guid id)
        {
            var picoBoard = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .FirstOrDefaultAsync(pb => pb.Id == id);

            if (picoBoard == null)
            {
                throw new NotFoundException($"Cannot connect Pico Board [{id}]: Pico Board not found");
            }

            picoBoard.Active = true;

            // TODO: It's blocking until it connects!
            var picoWBoard = new PicoWBoard(new PicoBoardDto(picoBoard));
            var result = await _boardsManager.AddPicoWBoard(picoWBoard);

            picoBoard.Connected = result;
            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }

        public void RunOnePicoBoardSync(Guid id)
        {
            _boardsManager.RunPicoBoardSyncOnce(id);
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

            var relatedCheckpoints = _context.Checkpoints
                .Where(c => c.BreakBeamSensorId == breakBeamSensorId)
                .ToList();

            relatedCheckpoints.ForEach(c => c.BreakBeamSensorId = null);

            picoBoard.BreakBeamSensors.Remove(sensor);
            _context.BreakBeamSensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return new PicoBoardDto(picoBoard);
        }
    }
}
