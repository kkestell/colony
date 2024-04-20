namespace Colony;

internal class Cell
{
    public int Tile { get; set; }
    public Color Foreground { get; set; }
    public Color Background { get; set; }
    
    public Cell(int tile, Color foreground, Color background)
    {
        Tile = tile;
        Foreground = foreground;
        Background = background;
    }
}