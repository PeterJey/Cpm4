using System;

namespace Cpm.Core.Services.Diary
{
    public struct DiaryRange : IEquatable<DiaryRange>
    {
        public DateTime FirstDay { get; set; }
        public int NumberOfWeeks { get; set; }
        public int Position { get; set; }

        public bool Equals(DiaryRange other)
        {
            return FirstDay.Equals(other.FirstDay) && NumberOfWeeks == other.NumberOfWeeks && Position == other.Position;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DiaryRange && Equals((DiaryRange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FirstDay.GetHashCode();
                hashCode = (hashCode * 397) ^ NumberOfWeeks;
                hashCode = (hashCode * 397) ^ Position;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"First day: {FirstDay:d} weeks: {NumberOfWeeks} position: {Position}";
        }
    }
}