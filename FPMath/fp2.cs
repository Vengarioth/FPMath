using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPMath
{
    public struct fp2 : IEquatable<fp2>
    {
        public static fp2 Zero { get { return new fp2(0, 0); } }
        public static fp2 One { get { return new fp2(1, 1); } }
        public static fp2 Up { get { return new fp2(0, 1); } }
        public static fp2 Down { get { return new fp2(0, -1); } }
        public static fp2 Left { get { return new fp2(-1, 0); } }
        public static fp2 Right { get { return new fp2(1, 0); } }

        public fp x;
        public fp y;

        public fp2(fp x, fp y)
        {
            this.x = x;
            this.y = y;
        }

        public fp magnitude()
        {
            return fp.Sqrt((x * x) + (y * y));
        }

        public fp2 normalize()
        {
            var m = magnitude();
            if (m == fp.Zero)
                return fp2.Zero;
            return new fp2(x / m, y / m);
        }

        public static fp2 operator +(fp2 lhs, fp rhs)
        {
            return new fp2(lhs.x + rhs, lhs.y + rhs);
        }

        public static fp2 operator -(fp2 lhs, fp rhs)
        {
            return new fp2(lhs.x - rhs, lhs.y - rhs);
        }

        public static fp2 operator *(fp2 lhs, fp rhs)
        {
            return new fp2(lhs.x * rhs, lhs.y * rhs);
        }

        public static fp2 operator /(fp2 lhs, fp rhs)
        {
            return new fp2(lhs.x / rhs, lhs.y / rhs);
        }

        public static fp2 operator +(fp2 lhs, fp2 rhs)
        {
            return new fp2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static fp2 operator -(fp2 lhs, fp2 rhs)
        {
            return new fp2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static fp2 operator *(fp2 lhs, fp2 rhs)
        {
            return new fp2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static fp2 operator /(fp2 lhs, fp2 rhs)
        {
            return new fp2(lhs.x / rhs.x, lhs.y / rhs.y);
        }

        public override string ToString()
        {
            return string.Format("fp2({0}, {1})", x, y);
        }

        public bool Equals(fp2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }
    }
}
