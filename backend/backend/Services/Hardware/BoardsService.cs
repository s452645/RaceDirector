using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Hardware
{
    enum PicoBoardInclusion
    {
        PICO_BOARD_BREAK_BEAM_SENSOR,
    }

    public class BoardsService
    {
        private readonly BackendContext _context;
        private readonly BoardSyncer _boardSyncer;
        private readonly BoardListener _boardListener;

        public BoardsService(BackendContext context, BoardSyncer boardSyncer, BoardListener boardListener)
        {
            _context = context;
            _boardSyncer = boardSyncer;
            _boardListener = boardListener;
        }

        public async Task<List<PicoBoardDto>> GetPicoBoards()
        {
            var boards = await _context.PicoBoards
                .Include(pb => pb.BreakBeamSensors)
                .ToListAsync();

            var boardsDtos = boards.Select(b => new PicoBoardDto(b)).ToList();
            return boardsDtos.Select(b => AssignLastSyncData(b)).ToList();
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

        public async Task<PicoBoardDto> AddPicoBoard(PicoBoardDto picoBoard)
        {
            var entity = picoBoard.ToEntity();
            _context.PicoBoards.Add(entity);
            await _context.SaveChangesAsync();

            return new PicoBoardDto(entity);
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

        public async Task DeletePicoBoard(Guid picoBoardId)
        {
            var inclusions = new List<PicoBoardInclusion> { PicoBoardInclusion.PICO_BOARD_BREAK_BEAM_SENSOR };
            var board = await FindPicoBoardOrThrow(picoBoardId, inclusions);

            var breakBeamSensors = board.BreakBeamSensors;

            _context.BreakBeamSensors.RemoveRange(breakBeamSensors);
            _context.PicoBoards.Remove(board);

            await _context.SaveChangesAsync();
        }

        public async Task<List<SyncBoardResult>> SyncAllBoards()
        {
            var boards = await _context.PicoBoards.Where(b => PicoBoardType.WiFi.Equals(b.Type)).ToListAsync();
            var syncTasks = new List<Task<SyncBoardResult>>();

            foreach (var board in boards)
            {
                var dto = new PicoBoardDto(board);
                var task = _boardSyncer.Sync(dto);
                syncTasks.Add(task);
            }

            IEnumerable<SyncBoardResult> syncResults = await Task.WhenAll(syncTasks);
            await _context.SyncBoardResults.AddRangeAsync(syncResults);
            await _context.SaveChangesAsync();

            return syncResults.ToList();
        }

        private async Task<PicoBoard> FindPicoBoardOrThrow(Guid picoBoardId, List<PicoBoardInclusion> inclusions)
        {
            var defaultErrorMsg = $"Pico Board [{picoBoardId}] not found";
            return await FindPicoBoardOrThrow(picoBoardId, inclusions, defaultErrorMsg);
        }

        private async Task<PicoBoard> FindPicoBoardOrThrow(Guid picoBoardId, List<PicoBoardInclusion> inclusions, string errorMsg)
        {
            var query = _context.PicoBoards.Where(picoBoard => picoBoard.Id == picoBoardId);
            
            var board = await AddInclusions(query, inclusions)
                .FirstOrDefaultAsync();

            if (board is null)
            {
                throw new NotFoundException(errorMsg);
            }

            return board;
        }

        private PicoBoardDto AssignLastSyncData(PicoBoardDto board)
        {
            var lastSync = _context.SyncBoardResults
                .Where(r => r.PicoBoardId == board.Id)
                .OrderByDescending(r => r.SyncFinishedTimestamp)
                .FirstOrDefault();

            if (lastSync == null)
            {
                return board;
            }

            board.LastSyncOffset = lastSync.CurrentSyncOffset;
            board.LastSyncResult = lastSync.SyncResult;
            board.LastSyncDateTime = lastSync.SyncFinishedTimestamp;

            return board;
        }

        private async Task<List<PicoBoard>> GetAllPicoBoards(List<PicoBoardInclusion> inclusions)
        {
            var query = _context.PicoBoards;
            return await AddInclusions(query, inclusions).ToListAsync();
        }

        private IQueryable<PicoBoard> AddInclusions(IQueryable<PicoBoard> query, List<PicoBoardInclusion> inclusions)
        {
            if (inclusions.Contains(PicoBoardInclusion.PICO_BOARD_BREAK_BEAM_SENSOR))
            {
                query.Include(picoBoard => picoBoard.BreakBeamSensors);
            }

            // add more if needed...

            return query;
        }
    }
}
