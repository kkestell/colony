namespace Colony;

internal class MapView(Map map)
{
    private Point _camera = new(0, 0);

    public void MoveCamera(int deltaX, int deltaY)
    {
        _camera.X += deltaX;
        _camera.Y += deltaY;

        _camera.X = Math.Max(0, Math.Min(map.Size - Configuration.CellsX, _camera.X));
        _camera.Y = Math.Max(0, Math.Min(map.Size - Configuration.CellsY, _camera.Y));
    }

    public void Render(CellRenderer renderer)
    {
        for (var y = 0; y < Configuration.CellsY; y++)
        {
            for (var x = 0; x < Configuration.CellsX; x++)
            {
                var mapX = x + _camera.X;
                var mapY = y + _camera.Y;

                if (mapX < 0 || mapX >= map.Size || mapY < 0 || mapY >= map.Size)
                    continue;
                
                var tile = map.Tiles[mapX, mapY];
                renderer.UpdateCell(x, y, (int)tile.TileNum, tile.Foreground, tile.Background);
            }
        }
    }
}
