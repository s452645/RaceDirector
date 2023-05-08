using backend.Models;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Hardware;
using backend.Models.Seasons.Events.Circuits;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Services.Hardware.Comms;
using Microsoft.EntityFrameworkCore;
using System.IO.Pipelines;

namespace backend.Services.Seasons.Events.Rounds.Races
{
    enum HeatState
    {
        NotStarted,
        Started,
        Finished,
    }

    public interface IHeatObserver
    {
        void Notify(SeasonEventRoundRaceHeatDto newHeatState);
    }

    public class HeatManager : IBoardEventsObserver
    {
        private readonly IServiceScopeFactory scopeFactory;

        private readonly SeasonEventRoundRaceHeatDto _heat;
        private readonly CircuitDto _circuit;
        private readonly SeasonEventScoreRulesDto _scoreRules;

        private readonly List<IHeatObserver> _observers = new();

        private readonly Guid StartSensorId;
        private readonly Guid FinishSensorId;

        private readonly List<long> _syncedCheckpointsTimestamps = new();

        private HeatState _state;

        public HeatManager(IServiceScopeFactory scopeFactory, SeasonEventRoundRaceHeatDto heat, CircuitDto circuit, SeasonEventScoreRulesDto scoreRules)
        {
            this.scopeFactory = scopeFactory;

            _heat = heat;
            _circuit = circuit;
            _scoreRules = scoreRules;

            var startCheckpoint = circuit.Checkpoints.Where(c => c.Type == CheckpointType.Start).FirstOrDefault();
            StartSensorId = startCheckpoint?.BreakBeamSensorId ?? Guid.Empty;

            var finishCheckpoint = circuit.Checkpoints.Where(c => c.Type == CheckpointType.Stop).FirstOrDefault();
            FinishSensorId = finishCheckpoint?.BreakBeamSensorId ?? Guid.Empty;

            _state = HeatState.NotStarted;
        }

        public void Register(IHeatObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unregister(IHeatObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(BoardEvent boardEvent)
        {
            if (HeatState.NotStarted == _state)
            {
                try
                {
                    ProcessStart(boardEvent);
                } 
                catch (InvalidDataException)
                {
                    Console.WriteLine("Processing start failed. InvalidDataException;");
                    return;
                }

            }

            if (HeatState.Started == _state)
            {
                try
                {
                    ProcessCheckpoint(boardEvent);
                } catch (InvalidDataException)
                {
                    Console.WriteLine("Processing checkpoint failed. InvalidDataException;");
                    return;
                }
            }
        }

        public void ProcessDistanceAndBonuses(float distance, List<float> bonuses)
        {
            var heatResult = _heat.Results.First();
            heatResult.DistancePoints = distance * _scoreRules.DistanceMultiplier;
            heatResult.Bonuses = bonuses.ToArray();

            HandleHeatChange();
        }

        public void EndHeat()
        {
            var heatResult = _heat.Results.First();

            if (HeatState.Finished != _state)
            {
              /*  while (heatResult.SectorTimes.Length < _circuit.Checkpoints.Count - 1)
                {
                    heatResult.SectorTimes = heatResult.SectorTimes.Append(_scoreRules.UnfinishedSectorPenaltyPoints).ToArray();
                }
*/
                CalculateTimePoints();
                _state = HeatState.Finished;
            }


            /*var pointsSummed =
                heatResult.TimePoints + 
                heatResult.AdvantagePoints + 
                heatResult.DistancePoints;
*/
  /*          heatResult.Bonuses.ToList().ForEach(b => pointsSummed += b);
            heatResult.PointsSummed = pointsSummed;
*/
            HandleHeatChange(true);
        }

        private void ProcessStart(BoardEvent boardEvent)
        {
            if (boardEvent.SensorId != StartSensorId || !boardEvent.Broken)
            {
                return;
            }

            var picoTimestamp = boardEvent.PicoLocalTimestamp;
            var syncedTimestamp = GetSyncedTimestamp(boardEvent.SensorId, picoTimestamp);

            _syncedCheckpointsTimestamps.Add(syncedTimestamp);
            _state = HeatState.Started;
        }

        private void ProcessCheckpoint(BoardEvent boardEvent)
        {
            var checkpoint = _circuit.Checkpoints.Where(c => c.BreakBeamSensorId == boardEvent.SensorId).FirstOrDefault();

            if (checkpoint == null)
            {
                throw new InvalidDataException();
            }

            if (checkpoint.Position != _syncedCheckpointsTimestamps.Count)
            {
                return;
            }

            var picoTimestamp = boardEvent.PicoLocalTimestamp;
            var syncedTimestamp = 
                checkpoint.Type == CheckpointType.Pause || 
                checkpoint.Type == CheckpointType.Resume ? 
                picoTimestamp :
                GetSyncedTimestamp(boardEvent.SensorId, picoTimestamp);

            var sectorTime = (syncedTimestamp - _syncedCheckpointsTimestamps.Last()) / 1000.0;
            _syncedCheckpointsTimestamps.Add(syncedTimestamp);

            var heatResult = _heat.Results.First();
            // heatResult.SectorTimes = heatResult.SectorTimes.Append((float)sectorTime).ToArray();

            var heatFinished = boardEvent.SensorId == FinishSensorId;
            if (heatFinished)
            {
                _state = HeatState.Finished;
                CalculateTimePoints();
            }

            HandleHeatChange();
        }

        private void CalculateTimePoints()
        {
            var heatResult = _heat.Results.First();

            var sectorIndex = 0;
            var paused = false;
            var fullTime = 0.0f;
            
            /*heatResult.SectorTimes.ToList().ForEach(sectorTime =>
            {
                if (paused && CheckpointType.Resume == _circuit.Checkpoints.Where(c => c.Position == sectorIndex + 1).First().Type)
                {
                    paused = false;
                    return;
                }

                if (!paused)
                {
                    fullTime += sectorTime;
                }

                if (CheckpointType.Pause == _circuit.Checkpoints.Where(c => c.Position == sectorIndex + 1).First().Type)
                {
                    paused = true;
                }

                sectorIndex++;
            });*/

            // heatResult.FullTime = fullTime;
            // heatResult.TimePoints = heatResult.FullTime * _scoreRules.TimeMultiplier;
        }


        private void HandleHeatChange(bool updateRace=false)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            if (updateRace)
            {
                var race = db.SeasonEventRoundRaces
                    .Where(r => r.Id == _heat.RaceId)
                    .Include(r => r.Results)
                    .Include(r => r.Round).ThenInclude(r => r.SeasonEvent).ThenInclude(e => e.ScoreRules)
                    .FirstOrDefault();

                if (race == null) 
                {
                    // TODO
                    return;
                }

                var raceResults = race.Results.Where(r => r.CarId == _heat.Results.First().CarId).FirstOrDefault();
                
                if (raceResults == null)
                {
                    raceResults = new SeasonEventRoundRaceResult();
                    raceResults.CarId = _heat.Results.First().CarId;
                }

                raceResults.Points = _heat.Results.First().PointsSummed;

                // FIXME
                var theMoreTheBetter = race.Round.SeasonEvent.ScoreRules?.TheMoreTheBetter ?? false;
                var sortedResults = new List<SeasonEventRoundRaceResult>();

                if (theMoreTheBetter) 
                {
                    sortedResults = race.Results.OrderByDescending(r => r.Points).ToList();
                } else
                {
                    sortedResults = race.Results.OrderBy(r => r.Points).ToList();
                }

                sortedResults.ForEach(result => {
                    result.Position = sortedResults.IndexOf(result);
                    db.SeasonEventRoundRaceResults.Update(result);
                });
            }

            db.SeasonEventRoundRaceHeats.Update(_heat.ToEntity());
            db.SaveChanges();

            _observers.ForEach(o => o.Notify(_heat));
        }


        private long GetSyncedTimestamp(Guid sensorId, long picoLocalTimestamp)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var sensor = db.BreakBeamSensors
                .Where(s => s.Id == sensorId)
                .FirstOrDefault();

            if (sensor == null) 
            {
                throw new InvalidDataException();
            }

            var boardId = sensor.BoardId;

            var lastSyncBeforeTimestamp = db.SyncBoardResults
                .ToList()
                .Where(sync =>
                    sync.PicoBoardId == boardId &&
                    sync.SyncResult == SyncResult.SYNC_SUCCESS &&
                    sync.ClockAdjustedPicoTimestamp != null &&
                    sync.NewClockOffset != null &&
                    sync.ClockAdjustedPicoTimestamp < picoLocalTimestamp)
                .MaxBy(sync => sync.SyncFinishedTimestamp);

            if (lastSyncBeforeTimestamp == null || lastSyncBeforeTimestamp.NewClockOffset == null)
            {
                throw new InvalidDataException();
            }

            return picoLocalTimestamp - (long)lastSyncBeforeTimestamp.NewClockOffset;
        }
    }
}
