using backend.Exceptions;
using backend.Models;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Hardware;
using backend.Services.Hardware;
using backend.Services.Hardware.Comms;
using Microsoft.EntityFrameworkCore;
using backend.Models.Seasons.Events.Rounds;
using backend.Models.Seasons.Events.Circuits;
using backend.Controllers.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Hardware;

namespace backend.Services.Seasons.Events.Rounds.Races
{
    public class RaceManager : IBoardListenerObserver
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly BoardListener _boardListener;
        private RaceDirectorWebSocket<SeasonEventRoundRaceDto>? _webSocket;

        private bool _eventsListenerStarted = false;
        private SeasonEventRound? _currentRound;

        public RaceManager(IServiceScopeFactory scopeFactory, BoardListener boardListener)
        {
            _scopeFactory = scopeFactory;
            _boardListener = boardListener;
            _currentRound = null;
            _webSocket = null;
        }

        public void RegisterWebsocket(RaceDirectorWebSocket<SeasonEventRoundRaceDto> websocket)
        {
            _webSocket = websocket;
        }

        public void UnregisterWebsocket()
        {
            _webSocket = null;
        }

        public async Task InitRace(
            Guid raceId, 
            Guid raceResultId
        )
        {
            if (!_eventsListenerStarted)
            {
                await StartListeningToEvents();
                _eventsListenerStarted = true;
                _boardListener.RegisterObserver(this);
            }

            await validateRoundChangeOrThrow(raceId);

            var raceResult = await GetRaceResult(raceId, raceResultId);
            raceResult.RaceState = RaceState.Pending;
            raceResult.SectorTimes = new List<float>().ToArray();
            raceResult.SectorTimestamps = new List<long>().ToArray();

            await UpdatePointsSummedAndPositions(raceResult);

            var raceDto = new SeasonEventRoundRaceDto(raceResult.SeasonEventRoundRace);
            if (_webSocket != null)
            {
                await _webSocket.Send(raceDto);
            }
        }

        public async Task setRaceDNF(Guid raceId, Guid raceResultId)
        {
            await validateRoundChangeOrThrow(raceId);

            var raceResult = await GetRaceResult(raceId, raceResultId);
            raceResult.RaceState = RaceState.Inactive;

            await UpdatePointsSummedAndPositions(raceResult);

            var raceDto = new SeasonEventRoundRaceDto(raceResult.SeasonEventRoundRace);
            if (_webSocket != null)
            {
                await _webSocket.Send(raceDto);
            }
        }

        public async Task SaveDistanceAndBonuses(Guid raceId, ResultData data)
        {
            await validateRoundChangeOrThrow(raceId);

            var raceResult = await GetRaceResult(raceId, data.RaceResultId);
            raceResult.Distance = data.Distance;
            raceResult.BonusesPoints = data.BonusesPoints;

            await UpdatePointsSummedAndPositions(raceResult);

            var raceDto = new SeasonEventRoundRaceDto(raceResult.SeasonEventRoundRace);
            if (_webSocket != null)
            {
                await _webSocket.Send(raceDto);
            }
        }

        // TODO: Notify for USB events as well, some method under the hood
        public async Task Notify(BoardEvent boardEvent)
        {
            if (!boardEvent.Broken)
            {
                return;
            }

            var checkpoints = _currentRound?.SeasonEvent.Circuit?.Checkpoints;
            if (checkpoints == null)
            {
                Console.WriteLine($"Cannot process board event: checkpoints not found");
                return;
            }

            var triggeredCheckpoint = checkpoints
                .Where(c => c.BreakBeamSensorId == boardEvent.SensorId)
                .FirstOrDefault();

            if (triggeredCheckpoint == null)
            {
                Console.WriteLine("Cannot process board event: triggered checkpoint not found");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var lookForRaceState = RaceState.Active;
            if (CheckpointType.Start == triggeredCheckpoint.Type)
            {
                lookForRaceState = RaceState.Pending;
            } else if (CheckpointType.Resume == triggeredCheckpoint.Type)
            {
                lookForRaceState = RaceState.Paused;
            }

            var possibleRaceResults = await db.SeasonEventRoundRaceResults
                .Where(r => r.RaceState == lookForRaceState)
                .Include(r => r.SeasonEventRoundRace).ThenInclude(r => r.Results).ThenInclude(r => r.Car)
                .ToListAsync();

            var possibleRaceResult = possibleRaceResults
                .Where(r => r.SectorTimestamps.Count() == triggeredCheckpoint.Position)
                .FirstOrDefault();

            if (possibleRaceResult == null)
            {
                Console.WriteLine($"Cannot process board event from [{triggeredCheckpoint.Name} ({boardEvent.Broken})]: there are no possible races for it");
                return;
            }


            var sensor = await db.BreakBeamSensors
                .Where(sensor => sensor.Id == boardEvent.SensorId)
                .FirstOrDefaultAsync();

            if (sensor == null)
            {
                Console.WriteLine($"Cannot process board event from [{triggeredCheckpoint.Name} ({boardEvent.Broken})]: sensor not found");
                return;
            }

            var boardId = sensor.BoardId;
            var picoTimestamp = new PicoTimestamp(boardId, boardEvent.PicoLocalTimestamp);
            var syncedTimestamp = await BoardSyncer.GetSyncedPicoTimestamp(picoTimestamp, db);

            if (syncedTimestamp == null)
            {
                Console.WriteLine($"Cannot process board event from [{triggeredCheckpoint.Name} ({boardEvent.Broken})]: cannot get synced timestamp");
                return;
            }

            var newSectorTimestamps = possibleRaceResult
                .SectorTimestamps
                .ToList();

            var doNotRegisterSectorTimeTypes = new List<CheckpointType> { CheckpointType.Start, CheckpointType.Resume };
            if (!doNotRegisterSectorTimeTypes.Contains(triggeredCheckpoint.Type))
            {
                var previousSectorTime = (syncedTimestamp - newSectorTimestamps.Last()) / 1000.0f;
                var newSectorTimes = possibleRaceResult.SectorTimes.ToList();
                newSectorTimes.Add(previousSectorTime ?? 0);
                possibleRaceResult.SectorTimes = newSectorTimes.ToArray();
            }

            // TODO: dont know why i have to escape null here
            newSectorTimestamps.Add(syncedTimestamp ?? 0);
            possibleRaceResult.SectorTimestamps = newSectorTimestamps.ToArray();

            if (CheckpointType.Start == triggeredCheckpoint.Type)
            {
                possibleRaceResult.RaceState = RaceState.Active;
            } 
            else if (CheckpointType.Pause == triggeredCheckpoint.Type)
            {
                possibleRaceResult.RaceState = RaceState.Paused;
            } 
            else if (CheckpointType.Resume == triggeredCheckpoint.Type)
            {
                possibleRaceResult.RaceState = RaceState.Active;
            }
            else if (CheckpointType.Stop == triggeredCheckpoint.Type)
            {
                possibleRaceResult.RaceState = RaceState.Inactive;
            }

            await UpdatePointsSummedAndPositions(possibleRaceResult);

            var raceDto = new SeasonEventRoundRaceDto(possibleRaceResult.SeasonEventRoundRace);
            if (_webSocket != null)
            {
                await _webSocket.Send(raceDto);
            }
        }

        private async Task UpdatePointsSummedAndPositions(SeasonEventRoundRaceResult raceResult)
        {
            if (raceResult.SeasonEventRoundRace == null || _currentRound == null)
            {
                throw new BadRequestException(string.Empty);
            }

            var scoreRules = _currentRound.SeasonEvent.ScoreRules;
            var totalTime = raceResult.SectorTimes.Sum();

            var excludeCheckpointTypes = new List<CheckpointType> { CheckpointType.Start, CheckpointType.Pause };
            var allSectorsCount = _currentRound.SeasonEvent.Circuit?.Checkpoints
                .Where(c => !excludeCheckpointTypes.Contains(c.Type))
                .Count();
            var unfinishedSectorsCount = allSectorsCount - raceResult.SectorTimes.Length;
            var unfinishedSectorsPoints = scoreRules?.UnfinishedSectorPenaltyPoints * unfinishedSectorsCount;

            var distancePoints = raceResult.Distance * scoreRules?.DistanceMultiplier;
            var timePoints = totalTime * scoreRules?.TimeMultiplier + unfinishedSectorsPoints;

            var totalPoints = (timePoints ?? 0) + (distancePoints ?? 0) + raceResult.BonusesPoints;
            raceResult.Points = totalPoints;

            var allRaceResults = raceResult.SeasonEventRoundRace.Results;

            // TODO: the more/less, the better
            allRaceResults.Sort((x, y) => x.Points.CompareTo(y.Points));

            var currentPosition = 1;
            foreach (SeasonEventRoundRaceResult result in allRaceResults)
            {
                if (result.Points.Equals(0))
                {
                    result.Position = 0;
                    result.RaceOutcome = RaceOutcome.Undetermined;
                    continue;
                }

                result.Position = currentPosition;
                currentPosition += 1;

                if (currentPosition <= raceResult.SeasonEventRoundRace.InstantAdvancements)
                {
                    result.RaceOutcome = RaceOutcome.Advanced;
                } else if (currentPosition <= raceResult.SeasonEventRoundRace.SecondChances)
                {
                    result.RaceOutcome = RaceOutcome.SecondChanceUndetermined;
                } else
                {
                    result.RaceOutcome = RaceOutcome.Dropped;
                }
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();
            
            db.SeasonEventRoundRaces.Update(raceResult.SeasonEventRoundRace);
            await db.SaveChangesAsync();
        }

        private async Task validateRoundChangeOrThrow(Guid raceId) 
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var race = await db.SeasonEventRoundRaces
                .Where(race => race.Id == raceId)
                .Include(race => race.Round).ThenInclude(round => round.SeasonEvent).ThenInclude(se => se.ScoreRules)           
                .Include(race => race.Round).ThenInclude(round => round.SeasonEvent).ThenInclude(se => se.Circuit).ThenInclude(c => c.Checkpoints)      
                .FirstOrDefaultAsync();

            if (race == null)
            {
                throw new NotFoundException($"Race [{raceId}] not found");
            }

            var hasRoundChanged = !(_currentRound == null || _currentRound.Id == race.RoundId);
            if (hasRoundChanged)
            {
                var anyRacesInProgress = await areAnyRacesInProgress();
                if (anyRacesInProgress)
                {
                    throw new BadRequestException("Cannot change rounds. There are some races in active state");
                }
            }

            _currentRound = race.Round;
        }

        private async Task<bool> areAnyRacesInProgress()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var racesInProgress = await db.SeasonEventRoundRaceResults
                .Where(race => !(RaceState.Inactive == race.RaceState))
                .ToListAsync();

            return racesInProgress.Count > 0;
        }

        private async Task<SeasonEventRoundRaceResult> GetRaceResult(Guid raceId, Guid raceResultId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var raceResult = await db.SeasonEventRoundRaceResults
                .Where(result => (result.SeasonEventRoundRaceId == raceId) && (result.Id == raceResultId))
                .Include(result => result.SeasonEventRoundRace).ThenInclude(r => r.Results).ThenInclude(r => r.Car)
                .Include(result => result.Car)
                .FirstOrDefaultAsync();

            if (raceResult == null)
            {
                throw new NotFoundException($"Race result [{raceResultId}] not found");
            }

            return raceResult;
        }

        private async Task StartListeningToEvents()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var boards = await db.PicoBoards
              .Include(pb => pb.BreakBeamSensors)
              .Where(b => PicoBoardType.WiFi.Equals(b.Type)).ToListAsync();

            var startListeningTasks = new List<Task>();

            foreach (var board in boards)
            {
                var dto = new PicoBoardDto(board);
                var task = _boardListener.StartListening(dto);
                startListeningTasks.Add(task);
            }

            await Task.WhenAll(startListeningTasks);
        }
    }
}
