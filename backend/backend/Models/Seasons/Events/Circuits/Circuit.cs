﻿using backend.Models.Seasons.Events;

namespace backend.Models.Seasons.Events.Circuits
{
    public class Circuit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Checkpoint> Checkpoints { get; set; }

        public SeasonEvent SeasonEvent { get; set; }

        // TODO: point calculation rules, bonuses, etc.
    }
}
