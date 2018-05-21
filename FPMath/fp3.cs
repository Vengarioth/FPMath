using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPMath
{
    public struct fp3 : IEquatable<fp3>
    {
        public static fp3 Zero { get { return new fp3(0, 0, 0); } }
        public static fp3 One { get { return new fp3(1, 1, 1); } }
        public static fp3 Up { get { return new fp3(0, 1, 0); } }
        public static fp3 Down { get { return new fp3(0, -1, 0); } }
        public static fp3 Left { get { return new fp3(-1, 0, 0); } }
        public static fp3 Right { get { return new fp3(1, 0, 0); } }
        public static fp3 Forward { get { return new fp3(0, 0, 1); } }
        public static fp3 Backward { get { return new fp3(0, 0, -1); } }

        public fp2 xy { get { return new fp2(x, y); } }

        public fp x;
        public fp y;
        public fp z;

        public fp3(fp x, fp y, fp z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public fp magnitude()
        {
            return fp.Sqrt((x * x) + (y * y) + (z * z));
        }

        public fp3 normalize()
        {
            var m = magnitude();
            if (m == fp.Zero)
                return fp3.Zero;
            return new fp3(x / m, y / m, z / m);
        }

        public static fp3 operator +(fp3 lhs, fp rhs)
        {
            return new fp3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
        }

        public static fp3 operator -(fp3 lhs, fp rhs)
        {
            return new fp3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
        }

        public static fp3 operator *(fp3 lhs, fp rhs)
        {
            return new fp3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static fp3 operator /(fp3 lhs, fp rhs)
        {
            return new fp3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        }

        public static fp3 operator +(fp3 lhs, fp3 rhs)
        {
            return new fp3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static fp3 operator -(fp3 lhs, fp3 rhs)
        {
            return new fp3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static fp3 operator *(fp3 lhs, fp3 rhs)
        {
            return new fp3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public static fp3 operator /(fp3 lhs, fp3 rhs)
        {
            return new fp3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        }

        public override string ToString()
        {
            return string.Format("fp3({0}, {1}, {2})", x, y, z);
        }

        public bool Equals(fp3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }
    }
}
