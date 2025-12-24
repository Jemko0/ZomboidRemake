using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.DataStructures
{
    public struct IntVector3 : IEquatable<IntVector3>
    {
        public int x;
        public int y;
        public int z;

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(IntVector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public static IntVector3 operator *(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static IntVector3 operator *(IntVector3 a, int scalar)
        {
            return new IntVector3(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static IntVector3 operator /(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
    }

    public struct FVector3
    {
        public float x;
        public float y;
        public float z;

        public FVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(FVector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public static FVector3 operator *(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static FVector3 operator *(FVector3 a, int scalar)
        {
            return new FVector3(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static FVector3 operator /(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static FVector3 operator +(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static FVector3 operator -(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
    }
}
