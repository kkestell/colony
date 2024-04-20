namespace Colony;

internal struct Tile
{
    public Glyph TileNum { get; set; } = 0;

    public Color Foreground { get; set; } = Color.White;

    public Color Background { get; set; } = Color.Black;

    public Tile()
    {
    }
}