using backend.Models.Hardware;

namespace backend.Models.Dtos.Hardware
{
    public class PicoBoardDto
    {
        public Guid Id { get; set; }
        public PicoBoardType Type { get; set; }
        public string Name { get; set; }

        public string IPAddress { get; set; }
        public List<BreakBeamSensorDto> BreakBeamSensors { get; set; }

        public PicoBoardDto()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            IPAddress = string.Empty;
            BreakBeamSensors = new List<BreakBeamSensorDto>();
        }

        public PicoBoardDto(Guid id, PicoBoardType type, string name, string iPAddress, List<BreakBeamSensorDto> breakBeamSensors)
        {
            Id = id;
            Type = type;
            Name = name;
            IPAddress = iPAddress;
            BreakBeamSensors = breakBeamSensors;
        }

        public PicoBoardDto(PicoBoard picoBoard)
        {
            Id = picoBoard.Id;
            Type = picoBoard.Type;
            Name = picoBoard.Name;
            IPAddress = picoBoard.IPAddress;
            BreakBeamSensors = picoBoard.BreakBeamSensors.Select(bbs => new BreakBeamSensorDto(bbs)).ToList();
        }

        public PicoBoard ToEntity()
        {
            var picoBoard = new PicoBoard();
            picoBoard.Id = Id;
            picoBoard.Type = Type;
            picoBoard.Name = Name;
            picoBoard.IPAddress = IPAddress;
            picoBoard.BreakBeamSensors = BreakBeamSensors.Select(bbs => bbs.ToEntity()).ToList();

            return picoBoard;
        }
    }
}
