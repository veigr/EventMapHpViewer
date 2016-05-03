using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    public class RemainingCount
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public RemainingCount()
        {
            this.Min = 0;
            this.Max = 0;
        }
        public RemainingCount(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }
        public RemainingCount(int value)
        {
            this.Min = value;
            this.Max = value;
        }

        public bool IsSingleValue => this.Min == this.Max;

        public override bool Equals(object obj)
        {
            return obj is RemainingCount && this.Equals((RemainingCount)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Min.GetHashCode() * 397) ^ this.Max.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{this.Min}-{this.Max}";
        }

        public static bool operator ==(RemainingCount value1, RemainingCount value2)
        {
            if (ReferenceEquals(value1, value2)) return true;
            return value1?.Equals(value2) ?? false;
        }

        public static bool operator !=(RemainingCount id1, RemainingCount id2)
        {
            return !(id1 == id2);
        }

        private bool Equals(RemainingCount other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Min == other.Min
                && this.Max == other.Max;
        }

        public static readonly RemainingCount MaxValue = new RemainingCount(int.MaxValue);
        public static readonly RemainingCount Zero = new RemainingCount(0);
    }
}
