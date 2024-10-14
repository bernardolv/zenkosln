using System;

public struct V3 : IEquatable<V3>, IComparable<V3>
{
    public const float kEpsilon = 1E-05f;

    public const float kEpsilonNormalSqrt = 1E-15f;

    //
    // Summary:
    //     X component of the vector.
    public float x;

    //
    // Summary:
    //     Y component of the vector.
    public float y;

    //
    // Summary:
    //     Z component of the vector.
    public float z;

    private static readonly V3 zeroVector = new V3(0f, 0f, 0f);

    private static readonly V3 oneVector = new V3(1f, 1f, 1f);

    private static readonly V3 upVector = new V3(0f, 1f, 0f);

    private static readonly V3 downVector = new V3(0f, -1f, 0f);

    private static readonly V3 leftVector = new V3(-1f, 0f, 0f);

    private static readonly V3 rightVector = new V3(1f, 0f, 0f);

    private static readonly V3 forwardVector = new V3(0f, 0f, 1f);

    private static readonly V3 backVector = new V3(0f, 0f, -1f);

    public float this[int index]
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return index switch
            {
                0 => x,
                1 => y,
                2 => z,
                _ => throw new IndexOutOfRangeException("Invalid V3 index!"),
            };
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid V3 index!");
            }
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(0, 0, 0).
    public static V3 zero
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return zeroVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(1, 1, 1).
    public static V3 one
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return oneVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(0, 0, 1).
    public static V3 forward
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return forwardVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(0, 0, -1).
    public static V3 back
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return backVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(0, 1, 0).
    public static V3 up
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return upVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(0, -1, 0).
    public static V3 down
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return downVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(-1, 0, 0).
    public static V3 left
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return leftVector;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3(1, 0, 0).
    public static V3 right
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return rightVector;
        }
    }

    //
    // Summary:
    //     Creates a new vector with given x, y, z components.
    //
    // Parameters:
    //   x:
    //
    //   y:
    //
    //   z:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    //
    // Summary:
    //     Creates a new vector with given x, y components and sets z to zero.
    //
    // Parameters:
    //   x:
    //
    //   y:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V3(float x, float y)
    {
        this.x = x;
        this.y = y;
        z = 0f;
    }

    //
    // Summary:
    //     Set x, y and z components of an existing V3.
    //
    // Parameters:
    //   newX:
    //
    //   newY:
    //
    //   newZ:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(float newX, float newY, float newZ)
    {
        x = newX;
        y = newY;
        z = newZ;
    }

    //
    // Summary:
    //     Multiplies two vectors component-wise.
    //
    // Parameters:
    //   a:
    //
    //   b:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 Scale(V3 a, V3 b)
    {
        return new V3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    //
    // Summary:
    //     Multiplies every component of this vector by the same component of scale.
    //
    // Parameters:
    //   scale:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Scale(V3 scale)
    {
        x *= scale.x;
        y *= scale.y;
        z *= scale.z;
    }

    //
    // Summary:
    //     Cross Product of two vectors.
    //
    // Parameters:
    //   lhs:
    //
    //   rhs:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 Cross(V3 lhs, V3 rhs)
    {
        return new V3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
    }

    //
    // Summary:
    //     Returns true if the given vector is exactly equal to this vector.
    //
    // Parameters:
    //   other:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object other)
    {
        if (!(other is V3))
        {
            return false;
        }

        return Equals((V3)other);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(V3 other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public int CompareTo(V3 other)
    {
        if (x != other.x)
        {
            return x < other.x ? -1 : 1;
        }
        if (y != other.y)
        {
            return y < other.y ? -1 : 1;
        }
        if (z != other.z)
        {
            return z < other.z ? -1 : 1;
        }
        return 0;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator +(V3 a, V3 b)
    {
        return new V3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator -(V3 a, V3 b)
    {
        return new V3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator -(V3 a)
    {
        return new V3(0f - a.x, 0f - a.y, 0f - a.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator *(V3 a, float d)
    {
        return new V3(a.x * d, a.y * d, a.z * d);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator *(float d, V3 a)
    {
        return new V3(a.x * d, a.y * d, a.z * d);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3 operator /(V3 a, float d)
    {
        return new V3(a.x / d, a.y / d, a.z / d);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(V3 lhs, V3 rhs)
    {
        float num = lhs.x - rhs.x;
        float num2 = lhs.y - rhs.y;
        float num3 = lhs.z - rhs.z;
        float num4 = num * num + num2 * num2 + num3 * num3;
        return num4 < 9.9999994E-11f;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(V3 lhs, V3 rhs)
    {
        return !(lhs == rhs);
    }

    public string ToStringV2()
    {
        return (x + 1).ToString() + (y + 1).ToString();
    }

    public string ToString()
    {
        return (x + 1).ToString() + (y + 1).ToString() + (z + 1).ToString();
    }
}