using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public enum Units 
    {
        Bytes,
        Meters,
        Grams,
        Seconds
    }

    public class UnitValue
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Units Unit { get; set; }
        public int Value { get; set; }

/*        public UnitValue(string name, Units unit, int value)
        {
            Name = name;
            Unit = unit;
            Value = value;
        }*/
    }
}
