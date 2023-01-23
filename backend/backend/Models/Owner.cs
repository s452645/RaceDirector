namespace backend.Models
{
    public class Owner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? PhotoId { get; set; }
        public Photo? Photo { get; set; }
        
        public DateTime? BirthDate { get; set; }
        public string Prefix { get; set; }


        public List<Car> Cars { get; set; }

/*        public Owner(string name, Photo? photo, DateTime? birthDate, string prefix)
        {
            Name = name;
            Photo = photo;
            BirthDate = birthDate;
            Prefix = prefix;
        }*/
    }
}
