namespace backend.Models.Dtos
{
    public class PicoBoardDto
    {
        public Guid Id { get; set; }
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

        public PicoBoardDto(Guid id, string name, string iPAddress, List<BreakBeamSensorDto> breakBeamSensors)
        {
            Id = id;
            Name = name;
            IPAddress = iPAddress;
            BreakBeamSensors = breakBeamSensors;
        }

        public PicoBoardDto(PicoBoard picoBoard)
        {
            Id = picoBoard.Id;
            Name = picoBoard.Name;
            IPAddress = picoBoard.IPAddress;
            BreakBeamSensors = picoBoard.BreakBeamSensors.Select(bbs => new BreakBeamSensorDto(bbs)).ToList();
        }

        public PicoBoard ToEntity()
        {
            var picoBoard = new PicoBoard();
            picoBoard.Id = Id;
            picoBoard.Name = Name;
            picoBoard.IPAddress = IPAddress;
            picoBoard.BreakBeamSensors = BreakBeamSensors.Select(bbs => bbs.ToEntity()).ToList();

            return picoBoard;
        }
    }
}
