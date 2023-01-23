namespace backend.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // set it up to be filled automatically on insert
        public DateTime UploadDate { get; set; }
        public byte[] File { get; set; }

        // not sure if it will be useful that way
        /*public UnitValue? Size { get; set; }*/

        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Note { get; set; }

/*        public Photo(
            string name, 
            DateTime uploadDate, 
            byte[] file, 
            UnitValue? size, 
            int width, 
            int height, 
            List<Note> notes
        )
        {
            Name = name;
            UploadDate = uploadDate;
            File = file;
            Size = size;
            Width = width;
            Height = height;
            Notes = notes;
        }*/
    }
}
