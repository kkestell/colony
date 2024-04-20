namespace Colony;

internal static class Rng
{
    private static readonly Random Random = new();
    
    public static int Next(int max)
    {
        return Random.Next(max);
    }
    
    public static int Next(int min, int max)
    {
        return Random.Next(min, max);
    }
    
    public static double NextDouble()
    {
        return Random.NextDouble();
    }
    
    public static double NextDouble(double min, double max)
    {
        return Random.NextDouble() * (max - min) + min;
    }
    
    public static float NextFloat()
    {
        return (float) Random.NextDouble();
    }
    
    public static float NextFloat(float min, float max)
    {
        return (float) Random.NextDouble() * (max - min) + min;
    }
}