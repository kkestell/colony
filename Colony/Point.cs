namespace Colony;

internal struct Point(int x, int y)
{
    public int X = x;
    public int Y = y;
    
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
    
    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }
    
    public static bool operator ==(Point a, Point b)
    {
        return a.X == b.X && a.Y == b.Y;
    }
    
    public static bool operator !=(Point a, Point b)
    {
        return a.X != b.X || a.Y != b.Y;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Point point && this == point;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}