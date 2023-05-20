using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults;
using backend.Models.Hardware;
using backend.Models.Seasons.Events.Circuits;
using backend.Models.Seasons.Events.Rounds.Races.Heats;
using backend.Repositories;
using backend.Services.Seasons.Events.Rounds.Races.Heats;
using Moq;

namespace backend.Tests.Services.Seasons.Events.Rounds.Races.Heats
{
    public class RaceHeatManagerTest
    {
        private readonly Guid HEAT_ID = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly Guid RACE_ID = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private readonly Guid CIRCUIT_ID = Guid.Parse("00000000-0000-0000-0000-000000000003");

        private readonly Guid START_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private readonly Guid TRACK_A_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000005");
        private readonly Guid TRACK_B_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000006");
        private readonly Guid TRACK_C_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000007");
        private readonly Guid TRACK_D_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000008");
        private readonly Guid FINISH_CHECKPOINT_ID = Guid.Parse("00000000-0000-0000-0000-000000000009");

        private readonly Guid START_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000010");
        private readonly Guid TRACK_A_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000011");
        private readonly Guid TRACK_B_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000012");
        private readonly Guid TRACK_C_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000013");
        private readonly Guid TRACK_D_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000014");
        private readonly Guid FINISH_SENSOR_ID = Guid.Parse("00000000-0000-0000-0000-000000000015");
        
        private readonly Guid SEASON_EVENT_ID = Guid.Parse("00000000-0000-0000-0000-000000000016");
        private readonly Guid SCORE_RULES_ID = Guid.Parse("00000000-0000-0000-0000-000000000017");

        private readonly Guid HEAT_RESULT_1_ID = Guid.Parse("00000000-0000-0000-0000-000000000018");
        private readonly Guid HEAT_RESULT_2_ID = Guid.Parse("00000000-0000-0000-0000-000000000019");
        private readonly Guid HEAT_RESULT_3_ID = Guid.Parse("00000000-0000-0000-0000-000000000020");
        private readonly Guid HEAT_RESULT_4_ID = Guid.Parse("00000000-0000-0000-0000-000000000021");

        private readonly Guid CAR_1_ID = Guid.Parse("00000000-0000-0000-0000-000000000022");
        private readonly Guid CAR_2_ID = Guid.Parse("00000000-0000-0000-0000-000000000023");
        private readonly Guid CAR_3_ID = Guid.Parse("00000000-0000-0000-0000-000000000024");
        private readonly Guid CAR_4_ID = Guid.Parse("00000000-0000-0000-0000-000000000025");

        private readonly Guid START_BOARD_ID = Guid.Parse("00000000-0000-0000-0000-000000000026");
        private readonly Guid TRACK_END_BOARD_ID = Guid.Parse("00000000-0000-0000-0000-000000000027");
        private readonly Guid FINISH_BOARD_ID = Guid.Parse("00000000-0000-0000-0000-000000000028");

        private readonly long RECEIVED_TIMESTAMP = 1_000_000;

        private readonly Mock<ISeasonEventRoundRaceHeatRepository> _heatRepoMock;
        private readonly Mock<ISyncBoardResultRepository> _syncBoardRepoMock;

        private RaceHeatManager? _raceHeatManager;

        public RaceHeatManagerTest()
        {
            _heatRepoMock = new Mock<ISeasonEventRoundRaceHeatRepository>();
            _syncBoardRepoMock = new Mock<ISyncBoardResultRepository>();
        }

        [Fact]
        public void ShouldCreateRaceHeatManager()
        {
            _raceHeatManager = InitRaceHeatManager();

            Assert.Equal(START_SENSOR_ID, _raceHeatManager.StartSensorId);
            Assert.Equal(FINISH_SENSOR_ID, _raceHeatManager.FinishSensorId);
        }

        [Fact]
        public async Task ShouldClearResultsWhenMovingToPendingState()
        {
            _raceHeatManager = InitRaceHeatManager();

            await _raceHeatManager.MoveToPendingState();

            _raceHeatManager.Heat.Results.ForEach(result =>
            {
                Assert.Equal(0, result.Position);
                Assert.Equal(0, result.DistancePoints);
                Assert.Equal(0, result.PointsSummed);
                Assert.Empty(result.SectorResults);
                Assert.Empty(result.Bonuses);
            });
        }

        [Fact]
        public async Task ShouldSaveStartTimestampOnStart()
        {
            // given
            var localStartTimestamp = 100;
            var syncOffset = -40_300;
            var startTimestamp = localStartTimestamp - syncOffset;

            MockLastSync(syncOffset, START_BOARD_ID, localStartTimestamp);

            var startBoardEvent = GetBoardEvent(START_BOARD_ID, START_SENSOR_ID, true, localStartTimestamp, RECEIVED_TIMESTAMP);
            var notBrokenStartBoardEvent = GetBoardEvent(START_BOARD_ID, START_SENSOR_ID, false, localStartTimestamp, RECEIVED_TIMESTAMP);
            var otherBoardEvent = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_C_SENSOR_ID, true, localStartTimestamp, RECEIVED_TIMESTAMP);

            _raceHeatManager = InitRaceHeatManager();

            // when & then

            // no action when in inactive state
            await _raceHeatManager.Notify(startBoardEvent);
            Assert.Null(_raceHeatManager.StartTimestamp);
            Assert.Equal(HeatState.Inactive, _raceHeatManager.Heat.State);

            // move to pending state
            await _raceHeatManager.MoveToPendingState();

            // no action when not broken
            await _raceHeatManager.Notify(notBrokenStartBoardEvent);
            Assert.Null(_raceHeatManager.StartTimestamp);
            Assert.Equal(HeatState.Pending, _raceHeatManager.Heat.State);

            // no action when other event
            await _raceHeatManager.Notify(otherBoardEvent);
            Assert.Null(_raceHeatManager.StartTimestamp);
            Assert.Equal(HeatState.Pending, _raceHeatManager.Heat.State);

            // action if previously moved to pending state
            await _raceHeatManager.Notify(startBoardEvent);
            Assert.Equal(HeatState.Active, _raceHeatManager.Heat.State);
            Assert.Equal(startTimestamp, _raceHeatManager.StartTimestamp);
        }

        [Fact]
        public async Task ShouldHandleFirstSectorResults()
        {
            // given
            var allBoardsSyncOffset = -40_300;

            var localStartTimestamp = 100;
            var startTimestamp = localStartTimestamp - allBoardsSyncOffset;

            var local_Track_A_Timestamp = localStartTimestamp + 10_000;
            var track_A_Timestamp = local_Track_A_Timestamp - allBoardsSyncOffset;

            var local_Track_B_Timestamp = localStartTimestamp + 9_000;
            var track_B_Timestamp = local_Track_B_Timestamp - allBoardsSyncOffset;

            var local_Track_C_Timestamp = localStartTimestamp + 15_000;
            var track_C_Timestap = local_Track_C_Timestamp - allBoardsSyncOffset;

            var local_Track_D_Timestamp = localStartTimestamp + 6_000;
            var track_D_Timestamp = local_Track_D_Timestamp - allBoardsSyncOffset;

            var startEvent = GetBoardEvent(START_BOARD_ID, START_SENSOR_ID, true, localStartTimestamp, RECEIVED_TIMESTAMP);
            var track_A_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_A_SENSOR_ID, true, local_Track_A_Timestamp, RECEIVED_TIMESTAMP);
            var track_B_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_B_SENSOR_ID, true, local_Track_B_Timestamp, RECEIVED_TIMESTAMP);
            var track_C_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_C_SENSOR_ID, true, local_Track_C_Timestamp, RECEIVED_TIMESTAMP);
            var track_D_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_D_SENSOR_ID, true, local_Track_D_Timestamp, RECEIVED_TIMESTAMP);

            MockLastSync(allBoardsSyncOffset, null, null);

            // when 
            _raceHeatManager = InitRaceHeatManager();
            await _raceHeatManager.MoveToPendingState();
            await _raceHeatManager.Notify(startEvent);

            await _raceHeatManager.Notify(track_A_Event);
            await _raceHeatManager.Notify(track_B_Event);
            await _raceHeatManager.Notify(track_C_Event);
            await _raceHeatManager.Notify(track_D_Event);

            // then
            var D_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_D);
            var D_sectorResult = D_result?.SectorResults.FirstOrDefault();
            AssertHeatResult(D_result, 1, 9);
            AssertSectorResult(D_sectorResult, 6_000, 1, 5, 4);

            var B_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_B);
            var B_sectorResult = B_result?.SectorResults.FirstOrDefault();
            AssertHeatResult(B_result, 2, 5);
            AssertSectorResult(B_sectorResult, 9_000, 2, 3, 2);

            var A_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_A);
            var A_sectorResult = A_result?.SectorResults.FirstOrDefault();
            AssertHeatResult(A_result, 3, 3);
            AssertSectorResult(A_sectorResult, 10_000, 3, 2, 1);

            var C_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_C);
            var C_sectorResult = C_result?.SectorResults.FirstOrDefault();
            AssertHeatResult(C_result, 4, 1);
            AssertSectorResult(C_sectorResult, 15_000, 4, 1, 0);
        }

        [Fact]
        public async Task ShouldHandleAllSectorResults()
        {
            // given
            var allBoardsSyncOffset = -40_300;

            var localStartTimestamp = 100;
            var startTimestamp = localStartTimestamp - allBoardsSyncOffset;

            var local_Track_A_Timestamp = localStartTimestamp + 10_500;
            var track_A_Timestamp = local_Track_A_Timestamp - allBoardsSyncOffset;

            var local_Track_B_Timestamp = localStartTimestamp + 10_800;
            var track_B_Timestamp = local_Track_B_Timestamp - allBoardsSyncOffset;

            var local_Track_C_Timestamp = localStartTimestamp + 12_000;
            var track_C_Timestap = local_Track_C_Timestamp - allBoardsSyncOffset;

            var local_Track_D_Timestamp = localStartTimestamp + 10_000;
            var track_D_Timestamp = local_Track_D_Timestamp - allBoardsSyncOffset;

            // sector 1 advantages points:
            // 10% -> 1s
            // 0.5 * 2 = 1
            // 0.3 * 1 = 0.3
            // 1 * 1 = 1

            var local_Finish_1_Timestamp = localStartTimestamp + 20_000;
            var finish_1_Timestamp = local_Finish_1_Timestamp - allBoardsSyncOffset;

            var local_Finish_2_Timestamp = localStartTimestamp + 20_050;
            var finish_2_Timestamp = local_Finish_1_Timestamp - allBoardsSyncOffset;

            var local_Finish_3_Timestamp = localStartTimestamp + 20_100;
            var finish_3_Timestamp = local_Finish_1_Timestamp - allBoardsSyncOffset;

            var local_Finish_4_Timestamp = localStartTimestamp + 25_000;
            var finish_4_Timestamp = local_Finish_1_Timestamp - allBoardsSyncOffset;

            // sector 2 advantages points:
            // 10% -> 2s
            // 0.025 * 3 = 0.075
            // 0.025 * 2 = 0.05
            // 1 * 2 = 2

            var startEvent = GetBoardEvent(START_BOARD_ID, START_SENSOR_ID, true, localStartTimestamp, RECEIVED_TIMESTAMP);
            var track_A_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_A_SENSOR_ID, true, local_Track_A_Timestamp, RECEIVED_TIMESTAMP);
            var track_B_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_B_SENSOR_ID, true, local_Track_B_Timestamp, RECEIVED_TIMESTAMP);
            var track_C_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_C_SENSOR_ID, true, local_Track_C_Timestamp, RECEIVED_TIMESTAMP);
            var track_D_Event = GetBoardEvent(TRACK_END_BOARD_ID, TRACK_D_SENSOR_ID, true, local_Track_D_Timestamp, RECEIVED_TIMESTAMP);
            var finish_1_Event = GetBoardEvent(FINISH_BOARD_ID, FINISH_SENSOR_ID, true, local_Finish_1_Timestamp, RECEIVED_TIMESTAMP);
            var finish_2_Event = GetBoardEvent(FINISH_BOARD_ID, FINISH_SENSOR_ID, true, local_Finish_2_Timestamp, RECEIVED_TIMESTAMP);
            var finish_3_Event = GetBoardEvent(FINISH_BOARD_ID, FINISH_SENSOR_ID, true, local_Finish_3_Timestamp, RECEIVED_TIMESTAMP);
            var finish_4_Event = GetBoardEvent(FINISH_BOARD_ID, FINISH_SENSOR_ID, true, local_Finish_4_Timestamp, RECEIVED_TIMESTAMP);

            MockLastSync(allBoardsSyncOffset, null, null);

            // when 
            _raceHeatManager = InitRaceHeatManager();
            await _raceHeatManager.MoveToPendingState();
            await _raceHeatManager.Notify(startEvent);

            await _raceHeatManager.Notify(track_A_Event);
            await _raceHeatManager.Notify(track_B_Event);
            await _raceHeatManager.Notify(track_C_Event);
            await _raceHeatManager.Notify(track_D_Event);

            await _raceHeatManager.Notify(finish_1_Event);
            await _raceHeatManager.Notify(finish_2_Event);
            await _raceHeatManager.Notify(finish_3_Event);
            await _raceHeatManager.Notify(finish_4_Event);

            // then
            var D_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_D);
            AssertHeatResult(D_result, 1, 17.425f);
            AssertSectorResult(D_result?.SectorResults.ElementAtOrDefault(0), 10_000, 1, 5, 2.3f);
            AssertSectorResult(D_result?.SectorResults.ElementAtOrDefault(1), 20_000, 1, 8, 2.125f);

            var A_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_A);
            AssertHeatResult(A_result, 2, 11.35f);
            AssertSectorResult(A_result?.SectorResults.ElementAtOrDefault(0), 10_500, 2, 3, 1.3f);
            AssertSectorResult(A_result?.SectorResults.ElementAtOrDefault(1), 20_050, 2, 5, 2.05f);

            var B_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_B);
            AssertHeatResult(B_result, 3, 8f);
            AssertSectorResult(B_result?.SectorResults.ElementAtOrDefault(0), 10_800, 3, 2, 1f);
            AssertSectorResult(B_result?.SectorResults.ElementAtOrDefault(1), 20_100, 3, 3, 2f);

            var C_result = _raceHeatManager.Heat.Results.Find(r => r.Track == Track.TRACK_C);
            AssertHeatResult(C_result, 4, 2f);
            AssertSectorResult(C_result?.SectorResults.ElementAtOrDefault(0), 12_000, 4, 1, 0);
            AssertSectorResult(C_result?.SectorResults.ElementAtOrDefault(1), 25_000, 4, 1, 0);

            // add bonuses and distance points
            await _raceHeatManager.UpdateBonuses(HEAT_RESULT_4_ID, new() { -5f, -3f });
            await _raceHeatManager.UpdateDistance(HEAT_RESULT_3_ID, 6.02f);

            Assert.InRange(D_result?.PointsSummed ?? 0, 9.42f, 9.44f);
            Assert.InRange(C_result?.PointsSummed ?? 0, 8.01f, 8.03f);
            Assert.Equal(1, A_result?.Position);
            Assert.Equal(2, D_result?.Position);
            Assert.Equal(3, C_result?.Position);
            Assert.Equal(4, B_result?.Position);
        }

        private void AssertHeatResult(RaceHeatResultDto? heatResult, int position, float pointsSummed)
        {
            Assert.NotNull(heatResult);
            Assert.Equal(position, heatResult.Position);
            Assert.Equal(pointsSummed, heatResult.PointsSummed);
        }

        private void AssertSectorResult(RaceHeatSectorResultDto? sectorResult, float time, int position, float positionPoints, float advantagePoints)
        {
            Assert.NotNull(sectorResult);

            Assert.Equal(time, sectorResult.Time);
            Assert.Equal(position, sectorResult.Position);
            Assert.Equal(positionPoints, sectorResult.PositionPoints);
            Assert.Equal(advantagePoints, sectorResult.AdvantagePoints);
        }

        private RaceHeatManager InitRaceHeatManager()
        {
            return new RaceHeatManager(
                _heatRepoMock.Object,
                _syncBoardRepoMock.Object,
                GetHeatDto(),
                GetCircuitDto(),
                GetScoreRulesDto()
            );
        }

        private SeasonEventRoundRaceHeatDto GetHeatDto()
        {
            var id = HEAT_ID;
            var order = 0;
            var state = HeatState.Inactive;
            var raceId = RACE_ID;
            var results = GetRaceHeatResults();

            return new SeasonEventRoundRaceHeatDto(
                id, order, state, raceId, results
            );
        }

        private List<RaceHeatResultDto> GetRaceHeatResults()
        {
            var result1 = new RaceHeatResultDto(
                HEAT_RESULT_1_ID, CAR_1_ID, Track.TRACK_A, 0, Array.Empty<float>(), 0, 0, HEAT_ID, new()
            );

            var result2 = new RaceHeatResultDto(
                HEAT_RESULT_2_ID, CAR_2_ID, Track.TRACK_B, 0, Array.Empty<float>(), 0, 0, HEAT_ID, new()
            );

            var result3 = new RaceHeatResultDto(
                HEAT_RESULT_3_ID, CAR_3_ID, Track.TRACK_C, 0, Array.Empty<float>(), 0, 0, HEAT_ID, new()
            );

            var result4 = new RaceHeatResultDto(
                HEAT_RESULT_4_ID, CAR_4_ID, Track.TRACK_D, 0, Array.Empty<float>(), 0, 0, HEAT_ID, new()
            );

            return new() { result1, result2, result3, result4 };
        }

        private CircuitDto GetCircuitDto()
        {
            var id = CIRCUIT_ID;
            var name = "Test Circuit Name";
            var checkpoints = GetCheckpointsDtos();
            var seasonEventId = SEASON_EVENT_ID;

            return new CircuitDto(
                id, name, checkpoints, seasonEventId
            );
        }

        private List<CheckpointDto> GetCheckpointsDtos()
        {
            var startCheckpoint = new CheckpointDto(
                START_CHECKPOINT_ID, 
                "Test Start Checkpoint Name", 
                0, 
                START_SENSOR_ID, 
                CheckpointType.Start, 
                CIRCUIT_ID, 
                Track.ALL
            );

            var trackACheckpoint = new CheckpointDto(
                TRACK_A_CHECKPOINT_ID,
                "Test Track A Checkpoint ID",
                1,
                TRACK_A_SENSOR_ID,
                CheckpointType.Continue,
                CIRCUIT_ID,
                Track.TRACK_A
            );

            var trackBCheckpoint = new CheckpointDto(
                TRACK_B_CHECKPOINT_ID,
                "Test Track B Checkpoint ID",
                2,
                TRACK_B_SENSOR_ID,
                CheckpointType.Continue,
                CIRCUIT_ID,
                Track.TRACK_B
            );

            var trackCCheckpoint = new CheckpointDto(
                TRACK_C_CHECKPOINT_ID,
                "Test Track C Checkpoint ID",
                3,
                TRACK_C_SENSOR_ID,
                CheckpointType.Continue,
                CIRCUIT_ID,
                Track.TRACK_C
            );

            var trackDCheckpoint = new CheckpointDto(
                TRACK_D_CHECKPOINT_ID,
                "Test Track D Checkpoint ID",
                4,
                TRACK_D_SENSOR_ID,
                CheckpointType.Continue,
                CIRCUIT_ID,
                Track.TRACK_D
            );


            var finishCheckpoint = new CheckpointDto(
                FINISH_CHECKPOINT_ID,
                "Test Finish Checkpoint ID",
                5,
                FINISH_SENSOR_ID,
                CheckpointType.Stop,
                CIRCUIT_ID,
                Track.ALL
            );

            return new() { startCheckpoint, trackACheckpoint, trackBCheckpoint, trackCCheckpoint, trackDCheckpoint, finishCheckpoint };
        }

        private SeasonEventScoreRulesDto GetScoreRulesDto()
        {
            var id = SCORE_RULES_ID;
            var timeMultiplier = 2.0f;
            var distanceMultiplier = 3.0f;
            List<float> availableBonuses = new() { -5.0f, -3.0f, -1.0f, 1.0f, 3.0f, 5.0f };
            var unfinishedSectorPenaltyPoints = 0.0f;
            var theMoreTheBetter = true;
            var seasonEventId = SEASON_EVENT_ID;

            return new SeasonEventScoreRulesDto(
                id, timeMultiplier, distanceMultiplier, availableBonuses, unfinishedSectorPenaltyPoints, theMoreTheBetter, seasonEventId
            );
        }

        private BoardEvent GetBoardEvent(Guid boardId, Guid sensorId, bool broken, long picoLocalTimestamp, long receivedTimestamp)
        {
            var sensor = new BreakBeamSensor();
            sensor.Id = sensorId;
            sensor.BoardId = boardId;

            var boardEvent = new BoardEvent();
            boardEvent.SensorId = sensorId;
            boardEvent.Sensor = sensor;
            boardEvent.Broken = broken;
            boardEvent.PicoLocalTimestamp = picoLocalTimestamp;
            boardEvent.ReceivedTimestamp = receivedTimestamp;

            return boardEvent;
        }

        private SyncBoardResult GetSyncBoardResult(long syncOffset)
        {
            var syncResult = new SyncBoardResult();
            syncResult.CurrentSyncOffset = syncOffset;
            syncResult.NewClockOffset = syncOffset;

            return syncResult;
        }

        private void MockLastSync(long offset, Guid? boardId, long? timestamp)
        {
            _syncBoardRepoMock.Setup(m => m.GetLastSyncBeforeLocalTimestamp(
                It.Is<Guid>(v => boardId == null || boardId == v), 
                It.Is<long>(v => timestamp == null || timestamp == v)))
                .Returns(GetSyncBoardResult(offset));
        }
    }
}