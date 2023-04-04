using backend.Models.Misc;
using backend.Models.Owners;
using backend.Models.Seasons.Events;
using backend.Models.Seasons.Events.Rounds;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Teams;

namespace backend.Models.Cars
{
    public class Car
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public OfficialName? OfficialName { get; set; }

        public Guid? OwnerId { get; set; }
        public Owner? Owner { get; set; }


        public Photo? MainPhoto { get; set; }
        public List<Photo> Photos { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public CarSize? Size { get; set; }
        public string? Note { get; set; }

        public List<SeasonEvent> SeasonEvents { get; set; }
        public List<SeasonEventRound> SeasonEventRounds { get; set; }
        public List<SeasonEventRoundRaceResult> RaceResults { get; set; }
        public List<SeasonEventRoundRaceHeatResult> HeatResults { get; set; }

        // multiple Pots, as in many seasons one car will be in different teams
        // each Pot is associated with one Team, and one Team with a single Season
        // maybe there should be some sort of validation that only one Pot from a single Season
        // can be assigned to a Car
        public List<Pot> Pots { get; set; }

        /*        public Car(
                    string name, 
                    OfficialName? officialName, 
                    Owner owner, 
                    Photo? mainPhoto, 
                    List<Photo> photos, 
                    DateTime? purchaseDate, 
                    CarSize? size, 
                    List<Note> notes
                )
                {
                    Name = name;
                    OfficialName = officialName;
                    Owner = owner;
                    MainPhoto = mainPhoto;
                    Photos = photos;
                    PurchaseDate = purchaseDate;
                    Size = size;
                    Notes = notes;
                }
        */
    }
}
