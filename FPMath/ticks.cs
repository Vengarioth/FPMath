using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPMath
{
    [Serializable]
    public struct ticks : IComparable<ticks>, IEquatable<ticks>
    {
        public int IntValue;

        public ticks(int value)
        {
            IntValue = value;
        }

        public static implicit operator ticks(int value)
        {
            return new ticks(value);
        }

        public static explicit operator int(ticks ticks)
        {
            return ticks.IntValue;
        }

        public static ticks operator +(ticks lhs, ticks rhs)
        {
            return new ticks(lhs.IntValue + rhs.IntValue);
        }

        public static ticks operator -(ticks lhs, ticks rhs)
        {
            return new ticks(lhs.IntValue - rhs.IntValue);
        }

        public static ticks operator *(ticks lhs, ticks rhs)
        {
            return new ticks(lhs.IntValue * rhs.IntValue);
        }

        public static ticks operator /(ticks lhs, ticks rhs)
        {
            return new ticks(lhs.IntValue / rhs.IntValue);
        }

        public static ticks operator %(ticks lhs, ticks rhs)
        {
            return new ticks(lhs.IntValue % rhs.IntValue);
        }

        public static bool operator >(ticks lhs, ticks rhs)
        {
            return lhs.IntValue > rhs.IntValue;
        }

        public static bool operator <(ticks lhs, ticks rhs)
        {
            return lhs.IntValue < rhs.IntValue;
        }

        public static bool operator >=(ticks lhs, ticks rhs)
        {
            return lhs.IntValue <= rhs.IntValue;
        }

        public static bool operator <=(ticks lhs, ticks rhs)
        {
            return lhs.IntValue <= rhs.IntValue;
        }

        public static bool operator ==(ticks lhs, ticks rhs)
        {
            return lhs.IntValue == rhs.IntValue;
        }

        public static bool operator !=(ticks lhs, ticks rhs)
        {
            return lhs.IntValue != rhs.IntValue;
        }

        public override string ToString()
        {
            return string.Format("ticks({0})", IntValue);
        }

        public override int GetHashCode()
        {
            return IntValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ticks))
                return false;
            return Equals((ticks)obj);
        }

        public int CompareTo(ticks other)
        {
            return IntValue - other.IntValue;
        }

        public bool Equals(ticks other)
        {
            return IntValue == other.IntValue;
        }
    }
}
