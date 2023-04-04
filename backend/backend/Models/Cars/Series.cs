namespace backend.Models.Cars
{
    public class Series
    {
        public Guid Id { get; set; }
        public string FirstLevelName { get; set; }
        public string? SecondLevelName { get; set; }

        public List<OfficialName> OfficialNames { get; set; }

        /*        public Series(string firstLevelName, string? secondLevelName)
                {
                    FirstLevelName = firstLevelName;
                    SecondLevelName = secondLevelName;
                }*/
    }
}
