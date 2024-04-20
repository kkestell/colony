namespace Colony;

internal class Map
{
    public int Size { get; init; }
    
    public Tile[,] Tiles { get; set; }

    public Map(int size)
    {
        Size = size;
        Tiles = new Tile[size, size];
    }
}