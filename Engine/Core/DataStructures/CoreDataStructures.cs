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

        public FVector3 ToFVector3()
        {
            return new FVector3(x, y, z);
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

        public static FVector3 operator *(FVector3 a, double scalar)
        {
            return new FVector3(a.x * (float)scalar, a.y * (float)scalar, a.z * (float)scalar);
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

        public IntVector3 ToIntVector3()
        {
            return new IntVector3((int)x, (int)y, (int)z);
        }
    }

    public struct IntVector2 : IEquatable<IntVector2>
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(IntVector2 other)
        {
            return x == other.x && y == other.y;
        }

        public static IntVector2 operator *(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x * b.x, a.y * b.y);
        }
        public static IntVector2 operator *(IntVector2 a, int scalar)
        {
            return new IntVector2(a.x * scalar, a.y * scalar);
        }

        public static IntVector2 operator /(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x / b.x, a.y / b.y);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }
    }
}
