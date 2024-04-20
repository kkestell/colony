namespace Colony;

internal class Genesis(int size)
{
    public Map CreateMap()
    {
        var map = new Map(size);
        
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                map.Tiles[x, y].TileNum = (Glyph)Rng.Next(256);
                map.Tiles[x, y].Foreground = Color.White;
                map.Tiles[x, y].Background = Color.Black;
            }
        }

        return map;
    }
}
