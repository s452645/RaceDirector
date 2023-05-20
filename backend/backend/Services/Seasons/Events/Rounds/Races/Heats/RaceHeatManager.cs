using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Hardware;
using backend.Models.Seasons.Events.Circuits;
using backend.Exceptions;
using backend.Models.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults;
using backend.Repositories;

namespace backend.Services.Seasons.Events.Rounds.Races.Heats
{
    public class RaceHeatManager : HeatManager
    {
        public Guid StartSensorId { get; }
        public Guid FinishSensorId { get; }

        public long? StartTimestamp { get; private set; }
        private int _finishCount = 0;

        public RaceHeatManager(
            ISeasonEventRoundRaceHeatRepository heatRepository,
            ISyncBoardResultRepository syncBoardResultRepository,
            SeasonEventRoundRaceHeatDto heat,
            CircuitDto circuit,
            SeasonEventScoreRulesDto scoreRules
        ) : base(heatRepository, syncBoardResultRepository, heat, circuit, scoreRules)
        {
            StartSensorId = FindStartSensorId();
            FinishSensorId = FindFinishSensorId();
        }

        public async override Task Notify(BoardEvent boardEvent)
        {
            if (!boardEvent.Broken)
            {
                return;
            }

            if (HeatState.Inactive == Heat.State)
            {
                return;
            }

            if (HeatState.Pending == Heat.State)
            {
                await HandlePending(boardEvent);
            }

            await HandleActive(boardEvent);
        }

        public async override Task MoveToPendingState()
        {
            if (HeatState.Inactive != Heat.State)
            {
                return;
            }

            Heat.Results.ForEach(heatResult =>
            {
                heatResult.DistancePoints = 0;
                heatResult.PointsSummed = 0;
                heatResult.Position = 0;
                Array.Clear(heatResult.Bonuses);
                heatResult.SectorResults.Clear();
            });

            Heat.State = HeatState.Pending;
            await HandleChanges();
        }

        public async override Task MoveToInactiveState()
        {
            Heat.State = HeatState.Inactive;
            await HandleChanges();
        }

        public override async Task UpdateBonuses(Guid heatResultId, List<float> bonuses)
        {
            var heatResult = Heat.Results.FirstOrDefault(r => r.Id == heatResultId);

            if (heatResult == null) 
            {
                return;
            }

            heatResult.Bonuses = bonuses.ToArray();
            await HandleChanges();
        }

        public override async Task UpdateDistance(Guid heatResultId, float distance)
        {
            var heatResult = Heat.Results.FirstOrDefault(r => r.Id == heatResultId);

            if (heatResult == null)
            {
                return;
            }

            heatResult.DistancePoints = distance * _scoreRules.DistanceMultiplier;
            await HandleChanges();
        }

        private async Task HandlePending(BoardEvent boardEvent)
        {
            if (boardEvent.SensorId != StartSensorId)
            {
                return;
            }
            
            // TODO: check if related Sensor is available at this point
            var startTimestamp = ConvertToSyncedTimestamp(boardEvent.PicoLocalTimestamp, boardEvent.Sensor.BoardId);
            StartTimestamp = startTimestamp;
            Heat.State = HeatState.Active;

            await HandleChanges();
        }

        private async Task HandleActive(BoardEvent boardEvent)
        {
            if (boardEvent.SensorId == FinishSensorId)
            {
                HandleFinishSector(boardEvent);
            } 
            else
            {
                HandleFirstSector(boardEvent);
            }

            await HandleChanges();
        }

        private void HandleFinishSector(BoardEvent boardEvent)
        {
            _finishCount++;

            var heatResult = Heat.Results.Find(result =>
            {
                var sectorResult = result.SectorResults.FirstOrDefault();
                return sectorResult != null && sectorResult.Position == _finishCount;
            });

            if (heatResult == null || StartTimestamp == null)
            {
                return;
            }

            var checkpointTimestamp = ConvertToSyncedTimestamp(boardEvent.PicoLocalTimestamp, boardEvent.Sensor.BoardId);
            var sectorTime = checkpointTimestamp - StartTimestamp;

            var sectorResult = new RaceHeatSectorResultDto();
            sectorResult.Order = 1;
            sectorResult.Time = (float)sectorTime;
            heatResult.SectorResults.Add(sectorResult);
        }

        private void HandleFirstSector(BoardEvent boardEvent)
        {
            // TODO:
            // not expected here: track is Track.ALL
            // not expected here: there is checkpoints configuration other than start - 4x track finish - finish for all
            var checkpoint = _circuit.Checkpoints.Find(checkpoint => checkpoint.BreakBeamSensorId == boardEvent.SensorId);

            if (checkpoint == null)
            {
                return;
            }

            var heatResult = Heat.Results.Find(result => result.Track == checkpoint.Track);
            if (heatResult is null || heatResult.SectorResults.Count > 0 || StartTimestamp is null)
            {
                return;
            }

            // TODO: check if related Sensor is available at this point
            var checkpointTimestamp = ConvertToSyncedTimestamp(boardEvent.PicoLocalTimestamp, boardEvent.Sensor.BoardId);
            var sectorTime = checkpointTimestamp - StartTimestamp;

            var sectorResult = new RaceHeatSectorResultDto();
            sectorResult.Order = 0;
            sectorResult.Time = (float)sectorTime;
            heatResult.SectorResults.Add(sectorResult);
        }

        private async Task HandleChanges()
        {
            Heat.processResultsChanges(_scoreRules);

            _heatRespository.UpdateHeat(Heat);
            await _heatRespository.SaveAsync();

            _observers.ForEach(o => o.Notify(Heat));
        }

        private Guid FindStartSensorId()
        {
            var startCheckpont = _circuit.Checkpoints.Find(
                checkpoint => CheckpointType.Start == checkpoint.Type
            );

            if (startCheckpont is null) 
            {
                throw new CustomHttpException($"Initializing HeatManager failed: no starting checkpoint in circuit [{_circuit.Id}]");
            }

            var startSensorId = startCheckpont.BreakBeamSensorId;

            if (startSensorId is null)
            {
                throw new CustomHttpException($"Initializing HeatManager failed: no sensor assigned to checkpoint [{startCheckpont.Id}]");
            }

            return startSensorId.Value;
        }

        private Guid FindFinishSensorId()
        {
            var finishCheckpoint = _circuit.Checkpoints.Find(
                checkpoint => CheckpointType.Stop == checkpoint.Type
            );

            if (finishCheckpoint is null)
            {
                throw new CustomHttpException($"Initializing HeatManager failed: no finish checkpoint in circuit [{_circuit.Id}]");
            }

            var finishCheckpointId = finishCheckpoint.BreakBeamSensorId;

            if (finishCheckpointId is null)
            {
                throw new CustomHttpException($"Initializing HeatManager failed: no sensor assigned to checkpoint [{finishCheckpoint.Id}]");
            }

            return finishCheckpointId.Value;
        }
    }
}
