using backend.Models.Cars;

namespace backend.Models.Dtos.Cars
{
    public class CarDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public CarDto()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
        }

        public CarDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public CarDto(Car car)
        {
            Id = car.Id;
            Name = car.Name;
        }
    }
}
