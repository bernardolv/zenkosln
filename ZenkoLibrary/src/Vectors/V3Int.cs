using System;

//
// Summary:
//     Representation of 3D vectors and points using integers.
public struct V3Int : IEquatable<V3Int>, IComparable<V3Int>
{
    private int m_X;

    private int m_Y;

    private int m_Z;

    private static readonly V3Int s_Zero = new V3Int(0, 0, 0);

    private static readonly V3Int s_One = new V3Int(1, 1, 1);

    private static readonly V3Int s_Up = new V3Int(0, 1, 0);

    private static readonly V3Int s_Down = new V3Int(0, -1, 0);

    private static readonly V3Int s_Left = new V3Int(-1, 0, 0);

    private static readonly V3Int s_Right = new V3Int(1, 0, 0);

    private static readonly V3Int s_Forward = new V3Int(0, 0, 1);

    private static readonly V3Int s_Back = new V3Int(0, 0, -1);

    //
    // Summary:
    //     X component of the vector.
    public int x
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_X;
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_X = value;
        }
    }

    //
    // Summary:
    //     Y component of the vector.
    public int y
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Y;
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Y = value;
        }
    }

    //
    // Summary:
    //     Z component of the vector.
    public int z
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Z;
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Z = value;
        }
    }

    public int this[int index]
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return index switch
            {
                0 => x,
                1 => y,
                2 => z,
                _ => throw new IndexOutOfRangeException("Invalid V3Int index addressed: " + index + "!"),
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
                    throw new IndexOutOfRangeException("Invalid V3Int index addressed: " + index + "!");
            }
        }
    }


    //
    // Summary:
    //     Shorthand for writing V3Int(0, 0, 0).
    public static V3Int zero
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Zero;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(1, 1, 1).
    public static V3Int one
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_One;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(0, 1, 0).
    public static V3Int up
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Up;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(0, -1, 0).
    public static V3Int down
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Down;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(-1, 0, 0).
    public static V3Int left
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Left;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(1, 0, 0).
    public static V3Int right
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Right;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(0, 0, 1).
    public static V3Int forward
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Forward;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V3Int(0, 0, -1).
    public static V3Int back
    {
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return s_Back;
        }
    }

    //
    // Summary:
    //     Initializes and returns an instance of a new V3Int with x and y components
    //     and sets z to zero.
    //
    // Parameters:
    //   x:
    //     The X component of the V3Int.
    //
    //   y:
    //     The Y component of the V3Int.
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V3Int(int x, int y)
    {
        m_X = x;
        m_Y = y;
        m_Z = 0;
    }

    //
    // Summary:
    //     Initializes and returns an instance of a new V3Int with x, y, z components.
    //
    //
    // Parameters:
    //   x:
    //     The X component of the V3Int.
    //
    //   y:
    //     The Y component of the V3Int.
    //
    //   z:
    //     The Z component of the V3Int.
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V3Int(int x, int y, int z)
    {
        m_X = x;
        m_Y = y;
        m_Z = z;
    }

    //
    // Summary:
    //     Set x, y and z components of an existing V3Int.
    //
    // Parameters:
    //   x:
    //
    //   y:
    //
    //   z:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int x, int y, int z)
    {
        m_X = x;
        m_Y = y;
        m_Z = z;
    }


    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator V3(V3Int v)
    {
        return new V3(v.x, v.y, v.z);
    }
    //
    // Summary:
    //     Converts a V3 to a V3Int by doing a Round to each value.
    //
    // Parameters:
    //   v:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int RoundToInt(V3 v)
    {
        return new V3Int((int)v.x, (int)v.y, (int)v.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator +(V3Int a, V3Int b)
    {
        return new V3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator -(V3Int a, V3Int b)
    {
        return new V3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator *(V3Int a, V3Int b)
    {
        return new V3Int(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator -(V3Int a)
    {
        return new V3Int(-a.x, -a.y, -a.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator *(V3Int a, int b)
    {
        return new V3Int(a.x * b, a.y * b, a.z * b);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator *(int a, V3Int b)
    {
        return new V3Int(a * b.x, a * b.y, a * b.z);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V3Int operator /(V3Int a, int b)
    {
        return new V3Int(a.x / b, a.y / b, a.z / b);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(V3Int lhs, V3Int rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(V3Int lhs, V3Int rhs)
    {
        return !(lhs == rhs);
    }

    //
    // Summary:
    //     Returns true if the objects are equal.
    //
    // Parameters:
    //   other:
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object other)
    {
        if (!(other is V3Int))
        {
            return false;
        }

        return Equals((V3Int)other);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(V3Int other)
    {
        return this == other;
    }

    //
    // Summary:
    //     Gets the hash code for the V3Int.
    //
    // Returns:
    //     The hash code of the V3Int.
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        int hashCode = y.GetHashCode();
        int hashCode2 = z.GetHashCode();
        return x.GetHashCode() ^ (hashCode << 4) ^ (hashCode >> 28) ^ (hashCode2 >> 4) ^ (hashCode2 << 28);
    }

    public int CompareTo(V3Int other)
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

    public string ToStringV2()
    {
        return (x + 1).ToString() + (y + 1).ToString();
    }

    // //
    // // Summary:
    // //     Returns a formatted string for this vector.
    // //
    // // Parameters:
    // //   format:
    // //     A numeric format string.
    // //
    // //   formatProvider:
    // //     An object that specifies culture-specific formatting.
    // // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public override string ToString()
    // {
    //     return ToString(null, null);
    // }

    // //
    // // Summary:
    // //     Returns a formatted string for this vector.
    // //
    // // Parameters:
    // //   format:
    // //     A numeric format string.
    // //
    // //   formatProvider:
    // //     An object that specifies culture-specific formatting.
    // // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public string ToString(string format)
    // {
    //     return ToString(format, null);
    // }

    // //
    // // Summary:
    // //     Returns a formatted string for this vector.
    // //
    // // Parameters:
    // //   format:
    // //     A numeric format string.
    // //
    // //   formatProvider:
    // //     An object that specifies culture-specific formatting.
    // // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public string ToString(string format, IFormatProvider formatProvider)
    // {
    //     if (formatProvider == null)
    //     {
    //         formatProvider = CultureInfo.InvariantCulture.NumberFormat;
    //     }

    //     return "(" + x.ToString(format, formatProvider) + ", " + y.ToString(format, formatProvider) + ", " + z.ToString(format, formatProvider) + ")";
    // }
}