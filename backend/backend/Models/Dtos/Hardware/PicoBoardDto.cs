using backend.Models.Hardware;

namespace backend.Models.Dtos.Hardware
{
    public class PicoBoardDto
    {
        public Guid Id { get; set; }
        public PicoBoardType Type { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool Active { get; set; }
        public bool Connected { get; set; }
        public List<BreakBeamSensorDto> BreakBeamSensors { get; set; }

        public DateTime? LastSyncDateTime { get; set; }
        public long? LastSyncOffset { get; set; }
        public SyncResult? LastSyncResult { get; set; }

        public PicoBoardDto()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            IPAddress = string.Empty;
            Active = false;
            Connected = false;
            BreakBeamSensors = new List<BreakBeamSensorDto>();
        }

        public PicoBoardDto(Guid id, PicoBoardType type, string name, string iPAddress, bool active, bool connected, List<BreakBeamSensorDto> breakBeamSensors)
        {
            Id = id;
            Type = type;
            Name = name;
            IPAddress = iPAddress;
            Active = active;
            Connected = connected;
            BreakBeamSensors = breakBeamSensors;
        }

        public PicoBoardDto(PicoBoard picoBoard)
        {
            Id = picoBoard.Id;
            Type = picoBoard.Type;
            Name = picoBoard.Name;
            IPAddress = picoBoard.IPAddress;
            Active = picoBoard.Active;
            Connected = picoBoard.Connected;
            BreakBeamSensors = picoBoard.BreakBeamSensors.Select(bbs => new BreakBeamSensorDto(bbs)).ToList();
        }

        public PicoBoard ToEntity()
        {
            var picoBoard = new PicoBoard();
            picoBoard.Id = Id;
            picoBoard.Type = Type;
            picoBoard.Name = Name;
            picoBoard.IPAddress = IPAddress;
            picoBoard.Active = Active;
            picoBoard.Connected = Connected;
            picoBoard.BreakBeamSensors = BreakBeamSensors.Select(bbs => bbs.ToEntity()).ToList();

            return picoBoard;
        }
    }
}
