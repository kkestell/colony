namespace Colony;

internal static class Configuration
{
    public const int WindowWidth = 1440;
    public const int WindowHeight = 960;
    public const int AtlasTilesX = 16;
    public const int AtlasTilesY = 16;
    public const int CellsX = WindowWidth / 16;
    public const int CellsY = WindowHeight / 16;
    public const int TotalCells = CellsX * CellsY;
    public const string Font = "ib8x8u.bdf";
}