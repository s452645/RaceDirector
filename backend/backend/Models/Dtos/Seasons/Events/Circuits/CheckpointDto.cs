using backend.Models.Seasons.Events.Circuits;

namespace backend.Models.Dtos.Seasons.Events.Circuits
{
    public class CheckpointDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Position { get; set; }
        public Guid? BreakBeamSensorId { get; set; }
        public CheckpointType Type { get; set; }
        public Guid CircuitId { get; set; }

        public CheckpointDto()
        {
            Id = Guid.NewGuid();
        }

        public CheckpointDto(
            Guid id,
            string? name,
            int position,
            Guid? breakBeamSensorId,
            CheckpointType type,
            Guid circuitId
        )
        {
            Id = id;
            Name = name;
            Position = position;
            BreakBeamSensorId = breakBeamSensorId;
            Type = type;
            CircuitId = circuitId;
        }

        public CheckpointDto(Checkpoint checkpoint)
        {
            Id = checkpoint.Id;
            Name = checkpoint.Name;
            Position = checkpoint.Position;
            BreakBeamSensorId = checkpoint.BreakBeamSensorId;
            Type = checkpoint.Type;
            CircuitId = checkpoint.CircuitId;
        }

        public Checkpoint ToEntity()
        {
            var checkpoint = new Checkpoint();
            checkpoint.Id = Id;
            checkpoint.Name = Name ?? "";
            checkpoint.Position = Position;
            checkpoint.BreakBeamSensorId = BreakBeamSensorId;
            checkpoint.Type = Type;
            checkpoint.CircuitId = CircuitId;

            return checkpoint;
        }
    }
}
