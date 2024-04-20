namespace Colony;

internal struct Rectangle(int x, int y, int width, int height)
{
    public int X = x;
    public int Y = y;
    public int Width = width;
    public int Height = height;

    public bool Contains(Point point)
    {
        return point.X >= X && point.X < X + Width && point.Y >= Y && point.Y < Y + Height;
    }
    
    public bool Intersects(Rectangle other)
    {
        return X < other.X + other.Width && X + Width > other.X && Y < other.Y + other.Height && Y + Height > other.Y;
    }
    
    public Rectangle Intersection(Rectangle other)
    {
        var x = Math.Max(X, other.X);
        var y = Math.Max(Y, other.Y);
        var width = Math.Min(X + Width, other.X + other.Width) - x;
        var height = Math.Min(Y + Height, other.Y + other.Height) - y;
        return new Rectangle(x, y, width, height);
    }
    
    public Rectangle Union(Rectangle other)
    {
        var x = Math.Min(X, other.X);
        var y = Math.Min(Y, other.Y);
        var width = Math.Max(X + Width, other.X + other.Width) - x;
        var height = Math.Max(Y + Height, other.Y + other.Height) - y;
        return new Rectangle(x, y, width, height);
    }
    
    public static bool operator ==(Rectangle a, Rectangle b)
    {
        return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    }
    
    public static bool operator !=(Rectangle a, Rectangle b)
    {
        return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Rectangle rectangle && this == rectangle;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }
}