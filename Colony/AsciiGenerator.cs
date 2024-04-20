namespace Colony;

internal static class AsciiGenerator
{
    private static readonly Random Random = new Random();

    public static char GetRandomAsciiCharacter()
    {
        return (char)Random.Next(32, 127);
    }
}