namespace backend.Models
{
    public class SeasonCarStanding
    {
        public Guid Id { get; set; }
        
        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public Guid SeasonId { get; set; }
        public Season Season { get; set; }

        public double Points { get; set; }
        public int Place { get; set; }

    }
}
