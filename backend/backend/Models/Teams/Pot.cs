using backend.Models.Cars;

namespace backend.Models.Teams
{
    public enum Pots
    {
        FirstPot,
        SecondPot,
        ThirdPot,
        FourthPot,
        FifthPot,
    }
    public class Pot
    {
        public Guid Id { get; set; }
        public Pots Hierarchy { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }

        public List<Car> Cars { get; set; }

        /*       public Pot(Pots hierarchy, Team team, List<Car> cars)
               {
                   Hierarchy = hierarchy;
                   Team = team;
                   Cars = cars;
               }*/
    }
}
