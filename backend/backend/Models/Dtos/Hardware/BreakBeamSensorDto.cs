using backend.Models.Hardware;

namespace backend.Models.Dtos.Hardware
{
    public class BreakBeamSensorDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Pin { get; set; }
        public Guid BoardId { get; set; }

        public BreakBeamSensorDto()
        {
            Id = Guid.NewGuid();
        }

        public BreakBeamSensorDto(Guid id, string name, int pin, Guid boardId)
        {
            Id = id;
            Name = name;
            Pin = pin;
            BoardId = boardId;
        }

        public BreakBeamSensorDto(BreakBeamSensor breakBeamSensor)
        {
            Id = breakBeamSensor.Id;
            Name = breakBeamSensor.Name;
            Pin = breakBeamSensor.Pin;
            BoardId = breakBeamSensor.BoardId;
        }

        public BreakBeamSensor ToEntity()
        {
            var breakBeamSensor = new BreakBeamSensor();
            breakBeamSensor.Id = Id;
            breakBeamSensor.Name = Name ?? "";
            breakBeamSensor.Pin = Pin;
            breakBeamSensor.BoardId = BoardId;

            return breakBeamSensor;
        }
    }
}
