namespace Colony;

internal struct Color(float r, float g, float b)
{
    public float R { get; set; } = r;

    public float G { get; set; } = g;

    public float B { get; set; } = b;
    
    public static Color Black => new(0f, 0f, 0f);
    public static Color White => new(1f, 1f, 1f);
    
    public override bool Equals(object? obj)
    {
        if (obj is Color other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }

    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }
}