using System;

namespace MyMagicSpace
{
    [Serializable]
    public class TournamentInfo
    {
        public string name;
        public string description;
        public int id;
        public float entryFees;
        public DateTime startTime;
        public DateTime endTime;
        public bool isActive;
    }
}