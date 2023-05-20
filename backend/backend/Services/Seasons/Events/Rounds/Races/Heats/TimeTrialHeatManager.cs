/*using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Seasons.Events.Circuits;
using backend.Models.Hardware;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models;

namespace backend.Services.Seasons.Events.Rounds.Races.Heats
{ 
    public class TimeTrialHeatManager : HeatManager
    {
        private readonly Guid _startSensorId;
        private readonly Guid _finishSensorId;


        public TimeTrialHeatManager(
            IServiceScopeFactory scopeFactory, 
            SeasonEventRoundRaceHeatDto heat,
            CircuitDto circuit, 
            SeasonEventScoreRulesDto scoreRules
        ) : base(scopeFactory, heat, circuit, scoreRules)
        {
            var startCheckpoint = circuit.Checkpoints.Where(c => c.Type == CheckpointType.Start).FirstOrDefault();
            _startSensorId = startCheckpoint?.BreakBeamSensorId ?? Guid.Empty;

            var finishCheckpoint = circuit.Checkpoints.Where(c => c.Type == CheckpointType.Stop).FirstOrDefault();
            _finishSensorId = finishCheckpoint?.BreakBeamSensorId ?? Guid.Empty;
        }


        // async?
        public override void Notify(BoardEvent boardEvent)
        {
            if (HeatState.NotStarted == _heatState)
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

            if (HeatState.Started == _heatState)
            {
                try
                {
                    ProcessCheckpoint(boardEvent);
                }
                catch (InvalidDataException)
                {
                    Console.WriteLine("Processing checkpoint failed. InvalidDataException;");
                    return;
                }
            }
        }

        private void ProcessStart(BoardEvent boardEvent)
        {
            if (boardEvent.SensorId != _startSensorId || !boardEvent.Broken)
            {
                return;
            }

            var picoTimestamp = boardEvent.PicoLocalTimestamp;
            var syncedTimestamp = ConvertToSyncedTimestamp(boardEvent.SensorId, picoTimestamp);

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
                ConvertToSyncedTimestamp(boardEvent.SensorId, picoTimestamp);

            var sectorTime = (syncedTimestamp - _syncedCheckpointsTimestamps.Last()) / 1000.0;
            _syncedCheckpointsTimestamps.Add(syncedTimestamp);

            var heatResult = _heat.Results.First();
            // heatResult.SectorTimes = heatResult.SectorTimes.Append((float)sectorTime).ToArray();

            var heatFinished = boardEvent.SensorId == _finishSensorId;
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

            *//*heatResult.SectorTimes.ToList().ForEach(sectorTime =>
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
            });*//*

            // heatResult.FullTime = fullTime;
            // heatResult.TimePoints = heatResult.FullTime * _scoreRules.TimeMultiplier;
        }


        private void HandleHeatChange(bool updateRace = false)
        {
            using var scope = _scopeFactory.CreateScope();
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
                }
                else
                {
                    sortedResults = race.Results.OrderBy(r => r.Points).ToList();
                }

                sortedResults.ForEach(result =>
                {
                    result.Position = sortedResults.IndexOf(result);
                    db.SeasonEventRoundRaceResults.Update(result);
                });
            }

            db.SeasonEventRoundRaceHeats.Update(_heat.ToEntity());
            db.SaveChanges();

            _observers.ForEach(o => o.Notify(_heat));
        }
    }
}
*/