using System;

namespace StreamInSync.Models
{
    public class Room
    {
        public Room(
            int id, 
            string name, 
            User owner, 
            string programmeName, 
            TimeSpan runtimeInSeconds,
            DateTime programmeStartTime)
        {
            Id = id;
            Name = name;
            Owner = owner;
            ProgrammeName = programmeName;
            Runtime = runtimeInSeconds;
            ProgrammeStartTime = programmeStartTime;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public User Owner { get; private set; }

        public string ProgrammeName { get; private set; }

        public TimeSpan Runtime { get; private set; }

        public DateTime ProgrammeStartTime { get; private set; }
    }
}