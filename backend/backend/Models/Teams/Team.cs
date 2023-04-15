using backend.Models.Seasons;

namespace backend.Models.Teams
{
    public class Team
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Prefix { get; set; }
        public List<Pot> Pots { get; set; }

        public Guid SeasonId { get; set; }
        public Season Season { get; set; }


        /*        public Team(string name, string prefix, List<Pot> pots)
                {
                    Name = name;
                    Prefix = prefix;
                    Pots = pots;
                }*/
    }
}
