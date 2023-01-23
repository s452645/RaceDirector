namespace backend.Models
{
    public class CarSize
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public UnitValue? Weight { get; set; }
        public UnitValue? Length { get; set; }
        public UnitValue? Width { get; set; }
        public UnitValue? Height { get; set; }

 /*       public CarSize(Car car, UnitValue weight, UnitValue length, UnitValue width, UnitValue height)
        {
            Car = car;
            Weight = weight;
            Length = length;
            Width = width;
            Height = height;
        }*/
    }
}
