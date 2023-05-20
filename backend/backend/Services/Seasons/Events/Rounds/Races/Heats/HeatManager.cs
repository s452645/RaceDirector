using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Hardware;
using backend.Repositories;
using backend.Services.Hardware.Comms;

namespace backend.Services.Seasons.Events.Rounds.Races.Heats
{


    public interface IHeatObserver
    {
        void Notify(SeasonEventRoundRaceHeatDto newHeatState);
    }

    public abstract class HeatManager : IBoardEventsObserver
    {
        protected readonly ISeasonEventRoundRaceHeatRepository _heatRespository;
        protected readonly ISyncBoardResultRepository _syncBoardResultsRepository;

        public SeasonEventRoundRaceHeatDto Heat { get; }
        protected CircuitDto _circuit;
        protected SeasonEventScoreRulesDto _scoreRules;

        protected readonly List<IHeatObserver> _observers = new();

        protected HeatManager(
            ISeasonEventRoundRaceHeatRepository heatRepository,
            ISyncBoardResultRepository syncBoardResultRepository,
            SeasonEventRoundRaceHeatDto heat,
            CircuitDto circuit,
            SeasonEventScoreRulesDto scoreRules
        )
        {
            _heatRespository = heatRepository;
            _syncBoardResultsRepository = syncBoardResultRepository;
            Heat = heat;
            _circuit = circuit;
            _scoreRules = scoreRules;
        }

        public void Register(IHeatObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unregister(IHeatObserver observer)
        {
            _observers.Remove(observer);
        }

        public abstract Task Notify(BoardEvent boardEvent);

        public abstract Task MoveToPendingState();

        public abstract Task MoveToInactiveState();

        public abstract Task UpdateBonuses(Guid heatResultId, List<float> bonuses);

        public abstract Task UpdateDistance(Guid heatResultId, float distance);

        protected long ConvertToSyncedTimestamp(long picoLocalTimestamp, Guid boardId)
        {
            var lastSyncBeforeTimestamp = _syncBoardResultsRepository
                .GetLastSyncBeforeLocalTimestamp(boardId, picoLocalTimestamp);

            if (lastSyncBeforeTimestamp.NewClockOffset == null)
            {
                throw new InvalidDataException();
            }

            return picoLocalTimestamp - (long)lastSyncBeforeTimestamp.NewClockOffset;
        }
    }
}
