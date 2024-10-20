using System;

// namespace Zenko;

//
// Summary:
//     Representation of 2D vectors and points using integers.
public struct V2Int : IEquatable<V2Int>
{
    private int m_X;

    private int m_Y;

    private static readonly V2Int s_Zero = new V2Int(0, 0);

    private static readonly V2Int s_One = new V2Int(1, 1);

    private static readonly V2Int s_Up = new V2Int(0, 1);

    private static readonly V2Int s_Down = new V2Int(0, -1);

    private static readonly V2Int s_Left = new V2Int(-1, 0);

    private static readonly V2Int s_Right = new V2Int(1, 0);

    //
    // Summary:
    //     X component of the vector.
    public int x
    {
        get
        {
            return m_X;
        }
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
        get
        {
            return m_Y;
        }
        set
        {
            m_Y = value;
        }
    }

    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException($"Invalid V2Int index addressed: {index}!"),
            };
        }
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
                default:
                    throw new IndexOutOfRangeException($"Invalid V2Int index addressed: {index}!");
            }
        }
    }

    //
    // Summary:
    //     Returns the length of this vector (Read Only).
    public float magnitude
    {
        get
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
    }

    //
    // Summary:
    //     Returns the squared length of this vector (Read Only).
    public int sqrMagnitude
    {
        get
        {
            return x * x + y * y;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(0, 0).
    public static V2Int zero
    {
        get
        {
            return s_Zero;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(1, 1).
    public static V2Int one
    {
        get
        {
            return s_One;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(0, 1).
    public static V2Int up
    {
        get
        {
            return s_Up;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(0, -1).
    public static V2Int down
    {
        get
        {
            return s_Down;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(-1, 0).
    public static V2Int left
    {
        get
        {
            return s_Left;
        }
    }

    //
    // Summary:
    //     Shorthand for writing V2Int(1, 0).
    public static V2Int right
    {
        get
        {
            return s_Right;
        }
    }

    public V2Int(int x, int y)
    {
        m_X = x;
        m_Y = y;
    }

    //
    // Summary:
    //     Set x and y components of an existing V2Int.
    //
    // Parameters:
    //   x:
    //
    //   y:
    public void Set(int x, int y)
    {
        m_X = x;
        m_Y = y;
    }

    //
    // Summary:
    //     Returns the distance between a and b.
    //
    // Parameters:
    //   a:
    //
    //   b:
    public static float Distance(V2Int a, V2Int b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        return (float)Math.Sqrt(num * num + num2 * num2);
    }

    //
    // Summary:
    //     Returns a vector that is made from the smallest components of two vectors.
    //
    // Parameters:
    //   lhs:
    //
    //   rhs:
    public static V2Int Min(V2Int lhs, V2Int rhs)
    {
        return new V2Int(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
    }

    //
    // Summary:
    //     Returns a vector that is made from the largest components of two vectors.
    //
    // Parameters:
    //   lhs:
    //
    //   rhs:
    public static V2Int Max(V2Int lhs, V2Int rhs)
    {
        return new V2Int(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
    }

    //
    // Summary:
    //     Multiplies two vectors component-wise.
    //
    // Parameters:
    //   a:
    //
    //   b:
    public static V2Int Scale(V2Int a, V2Int b)
    {
        return new V2Int(a.x * b.x, a.y * b.y);
    }

    //
    // Summary:
    //     Multiplies every component of this vector by the same component of scale.
    //
    // Parameters:
    //   scale:
    public void Scale(V2Int scale)
    {
        x *= scale.x;
        y *= scale.y;
    }

    //
    // Summary:
    //     Clamps the V2Int to the bounds given by min and max.
    //
    // Parameters:
    //   min:
    //
    //   max:
    public void Clamp(V2Int min, V2Int max)
    {
        x = Math.Max(min.x, x);
        x = Math.Min(max.x, x);
        y = Math.Max(min.y, y);
        y = Math.Min(max.y, y);
    }
    public static V2Int operator -(V2Int v)
    {
        return new V2Int(-v.x, -v.y);
    }

    public static V2Int operator +(V2Int a, V2Int b)
    {
        return new V2Int(a.x + b.x, a.y + b.y);
    }

    public static V2Int operator -(V2Int a, V2Int b)
    {
        return new V2Int(a.x - b.x, a.y - b.y);
    }

    public static V2Int operator *(V2Int a, V2Int b)
    {
        return new V2Int(a.x * b.x, a.y * b.y);
    }

    public static V2Int operator *(int a, V2Int b)
    {
        return new V2Int(a * b.x, a * b.y);
    }

    public static V2Int operator *(V2Int a, int b)
    {
        return new V2Int(a.x * b, a.y * b);
    }

    public static V2Int operator /(V2Int a, int b)
    {
        return new V2Int(a.x / b, a.y / b);
    }

    public static bool operator ==(V2Int lhs, V2Int rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(V2Int lhs, V2Int rhs)
    {
        return !(lhs == rhs);
    }

    //
    // Summary:
    //     Returns true if the objects are equal.
    //
    // Parameters:
    //   other:
    public override bool Equals(object other)
    {
        if (!(other is V2Int))
        {
            return false;
        }

        return Equals((V2Int)other);
    }

    public bool Equals(V2Int other)
    {
        return x == other.x && y == other.y;
    }

    //
    // Summary:
    //     Gets the hash code for the V2Int.
    //
    // Returns:
    //     The hash code of the V2Int.
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }

    public string ToString()
    {
        return x.ToString() + "," + y.ToString();
    }

    public int CompareTo(V2Int other)
    {
        if (x != other.x)
        {
            return x < other.x ? -1 : 1;
        }
        if (y != other.y)
        {
            return y < other.y ? -1 : 1;
        }
        return 0;
    }
}