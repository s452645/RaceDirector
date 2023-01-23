namespace backend.Models
{
    public class OfficialName
    {
        public Guid Id { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }

        public Guid? SeriesId { get; set; }
        public Series? Series { get; set; }
        
        public string? WikiLink { get; set; }
        public string? Note { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

/*        public OfficialName(string modelName, int year, Series series, string wikiLink, List<Note> notes)
        {
            ModelName = modelName;
            Year = year;
            Series = series;
            WikiLink = wikiLink;
            Notes = notes;
        }*/
    }
}
