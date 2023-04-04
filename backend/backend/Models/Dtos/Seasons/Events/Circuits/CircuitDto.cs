using backend.Models.Seasons.Events.Circuits;

namespace backend.Models.Dtos.Seasons.Events.Circuits
{
    public class CircuitDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<CheckpointDto> Checkpoints { get; set; }

        public Guid? SeasonEventId { get; set; }

        public CircuitDto()
        {
            Id = Guid.NewGuid();
            Checkpoints = new List<CheckpointDto>();
        }

        public CircuitDto(Guid id, string? name, List<CheckpointDto> checkpoints, Guid? seasonEventId)
        {
            Id = id;
            Name = name;
            Checkpoints = checkpoints;
            SeasonEventId = seasonEventId;
        }

        public CircuitDto(Circuit circuit)
        {
            Id = circuit.Id;
            Name = circuit.Name;
            Checkpoints = circuit.Checkpoints.Select(checkpoint => new CheckpointDto(checkpoint)).ToList();
            SeasonEventId = circuit.SeasonEvent.Id;
        }

        public Circuit ToEntity()
        {
            // TODO: no season event id assignment, as circuit does not contain such information in db
            // (it cannot, since only one side of relationship should have a foreign key)
            // is it correct?

            var circuit = new Circuit();
            circuit.Id = Id;
            circuit.Name = Name ?? string.Empty;
            circuit.Checkpoints = Checkpoints.Select(checkpointDto => checkpointDto.ToEntity()).ToList();

            return circuit;
        }
    }
}
